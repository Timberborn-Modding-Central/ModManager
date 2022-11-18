using Modio.Filters;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManagerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using Timberborn.GameExitSystem;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;
using TextField = UnityEngine.UIElements.TextField;

namespace Timberborn.ModsSystemUI
{
    public class ModsBox : IPanelController
    {
        private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            VisualElement visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            _visualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }

        private static readonly string AllLocKey = "Mods.Tags.All";
        private static readonly uint ModsPerPage = 25;
        public static Action OpenOptionsDelegate;
        private readonly VisualElementLoader _visualElementLoader;
        private readonly PanelStack _panelStack;
        private readonly IAddonService _addonService;
        private readonly ModFullInfoController _modFullInfoController;
        private readonly ILoc _loc;
        private readonly VisualElementInitializer _visualElementInitializer;
        private VisualElement _mods;
        private Filter _filter = ModFilter.Downloads.Desc();
        private Label _loading;
        private Label _error;
        private TextField _search;
        private RadioButtonGroup _tags;
        private Button _showMore;
        private List<string> _tagOptions = new();
        private uint _page;

        private GoodbyeBoxFactory _goodbyeBoxFactory;

        private const string _bundleName = "what.bundle";
        public static AssetBundle _bundle;

        public ModsBox(VisualElementLoader visualElementLoader,
                       PanelStack panelStack,
                       IAddonService addonService,
                       VisualElementInitializer visualElementInitializer,
                       GoodbyeBoxFactory goodbyeBoxFactory,
                       ModFullInfoController modFullInfoController,
                       ILoc loc)
        {
            _visualElementLoader = visualElementLoader;
            _panelStack = panelStack;
            _addonService = addonService;
            OpenOptionsDelegate = OpenOptionsPanel;
            _visualElementInitializer = visualElementInitializer;
            _goodbyeBoxFactory = goodbyeBoxFactory;
            _modFullInfoController = modFullInfoController;
            _loc = loc;

            _bundle = AssetBundle.LoadFromFile($"{Path.Combine(UIPaths.ModManagerUI.Assets, _bundleName)}");
        }

        public VisualElement GetPanel()
        {
            string assetName = "assets/resources/ui/views/mods/modsbox.uxml";
            var asset = _bundle.LoadAsset<VisualTreeAsset>(assetName);
            var root = LoadVisualElement(asset);

            _mods = root.Q<ScrollView>("Mods");
            _loading = root.Q<Label>("Loading");
            _error = root.Q<Label>("Error");
            _tags = root.Q<RadioButtonGroup>("Tags");
            _showMore = root.Q<Button>("ShowMore");
            _showMore.ToggleDisplayStyle(false);

            ShowModsAndTags();

            root.Q<Button>("Close").clicked += OnUICancelled;
            root.Q<Button>("SearchButton").clicked += UpdateMods;
            _showMore.clicked += ShowMoreMods;
            _search = root.Q<TextField>("Search");
            _search.isDelayed = true;
            _search.RegisterValueChangedCallback(_ => UpdateMods());
            return root;
        }

        private void OpenOptionsPanel()
        {
            _panelStack.HideAndPush(this);
        }

        public bool OnUIConfirmed()
        {
            return false;
        }

        public void OnUICancelled()
        {
            _panelStack.Pop(this);


            var box = _goodbyeBoxFactory.ShowExitToDesktop();
            _panelStack.HideAndPush(box);
        }

        private Filter Filter => _filter.And(Filter.WithLimit(ModsPerPage).Offset(_page * ModsPerPage));

        private void ShowModsAndTags()
        {
            _mods.Clear();
            _page = 0;

            ShowMods();

            var getTagsTask = _addonService.GetTags().Get();
            getTagsTask.ConfigureAwait(true).GetAwaiter()
                .OnCompleted(() => OnTagsRetrieved(getTagsTask));
        }

        private void ShowMoreMods()
        {
            _page++;
            ShowMods();
        }

        private void ShowMods()
        {
            _loading.ToggleDisplayStyle(true);
            _error.ToggleDisplayStyle(false);
            var getModsTask = _addonService.GetMods().Search(Filter).FirstPage();
            getModsTask.ConfigureAwait(true).GetAwaiter()
                .OnCompleted(() => OnModsRetrieved(getModsTask));
        }

        private void UpdateMods()
        {
            _filter = ModFilter.Downloads.Desc();

            if (!string.IsNullOrEmpty(_search.value))
            {
                _filter = _filter.And(ModFilter.FullText.Eq(_search.value));
            }

            if (_tags.value > 0)
            {
                _filter = _filter.And(ModFilter.Tags.Eq(_tagOptions[_tags.value]));
            }

            ShowModsAndTags();
        }

        private void OnTagsRetrieved(Task<IReadOnlyList<TagOption>> task)
        {
            _tagOptions.Clear();
            _tagOptions.Add(_loc.T(AllLocKey));
            _tagOptions.AddRange(task.Result.SelectMany(tagGroup => tagGroup.Tags));
            _tags.choices = _tagOptions;

            _tags.RegisterValueChangedCallback(_ => UpdateMods());
        }

        private void OnModsRetrieved(Task<IReadOnlyList<Mod>> task)
        {
            try
            {
                FillTheWrapper(task.Result);
                if (task.Result.Count < ModsPerPage)
                {
                    _showMore.ToggleDisplayStyle(false);
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private async void FillTheWrapper(IReadOnlyCollection<Mod> mods)
        {
            _loading.ToggleDisplayStyle(false);
            _showMore.ToggleDisplayStyle(true);
            foreach (var mod in mods)
            {
                string assetName = "assets/resources/ui/views/mods/modsboxitem.uxml";
                var asset = _bundle.LoadAsset<VisualTreeAsset>(assetName);
                var item = _visualElementLoader.LoadVisualElement(asset);
                item.Q<Label>("Name").text = mod.Name;
                item.Q<Button>("ModsBoxItem").clicked += () => ShowFullInfo(mod);
                item.Q<Button>("Download").clicked += async () => await DoDownloadAndExtract(mod);

                SetNumbers(mod, item);
                await LoadImage(mod, item.Q<Image>("Logo"));

                _mods.Add(item);
            }
        }

        private void ShowFullInfo(Mod mod)
        {
            _panelStack.HideAndPush(_modFullInfoController);
            _modFullInfoController.SetMod(mod);
        }

        private async Task DoDownloadAndExtract(Mod modInfo)
        {
            IAsyncEnumerable<(string location, Mod Mod)> dependencies;
            try
            {
                (string location, Mod Mod) mod = await _addonService.DownloadLatest(modInfo);

                dependencies = _addonService.DownloadDependencies(modInfo);
                _addonService.Install(mod.Mod, mod.location);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            await foreach (var foo in _addonService.DownloadDependencies(modInfo))
            {
                try
                {
                    _addonService.Install(foo.Mod, foo.location);
                }
                catch (AddonException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning(ex.Message);
                }
            }
        }

        private static void SetNumbers(Mod mod, VisualElement item)
        {
            if (mod.Stats != null)
            {
                item.Q<Label>("UpCount").text = Format(mod.Stats.PositiveRatings);
                item.Q<Label>("DownCount").text = Format(mod.Stats.NegativeRatings);
                item.Q<Label>("DownloadCount").text = Format(mod.Stats.TotalDownloads);
            }
        }

        private async Task LoadImage(Mod mod, Image root)
        {
            if (mod.Logo != null)
            {
                var byteArray = await _addonService.GetImage(mod.Logo.Thumb320x180);
                var texture = new Texture2D(1, 1);
                texture.LoadImage(byteArray);
                root.image = texture;
            }
        }

        private void ShowError(Exception e)
        {
            _loading.ToggleDisplayStyle(false);
            _error.ToggleDisplayStyle(true);
            _error.text = e.Message;
        }

        private static string Format(uint number)
        {
            //return NumberFormatter.Format((int) number);
            return number.ToString();
        }

    }
}