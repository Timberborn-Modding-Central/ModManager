using Modio.Filters;
using Modio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;
using TextField = UnityEngine.UIElements.TextField;

namespace Timberborn.ModsSystemUI
{
    public class ModsBox : IPanelController
    {


        private VisualElement LoadVisualElement(string elementName) => LoadVisualElement(LoadVisualTreeAsset(elementName));

        //private VisualTreeAsset LoadVisualTreeAsset(string elementName) => _resourceAssetLoader.Load<VisualTreeAsset>($"ModManager/what/{elementName}");
        private VisualTreeAsset LoadVisualTreeAsset(string elementName) => _resourceAssetLoader.Load<VisualTreeAsset>($"{elementName}");

        private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            VisualElement visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            _visualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }


        public static Action OpenOptionsDelegate;
        private readonly VisualElementLoader _visualElementLoader;
        private readonly PanelStack _panelStack;
        private readonly IModService _modService;
        private readonly VisualElementInitializer _visualElementInitializer;
        private readonly IResourceAssetLoader _resourceAssetLoader;
        private VisualElement _mods;
        private Filter _filter = ModFilter.Downloads.Desc();
        private Label _loading;
        private Label _error;
        private TextField _search;
        private RadioButtonGroup _tags;
        private List<string> _tagOptions;

        private AssetBundle _bundle;

        public ModsBox(VisualElementLoader visualElementLoader,
                       PanelStack panelStack,
                       IModService modService,
                       VisualElementInitializer visualElementInitializer,
                       IResourceAssetLoader resourceAssetLoader)
        {
            _visualElementLoader = visualElementLoader;
            _panelStack = panelStack;
            _modService = modService;
            OpenOptionsDelegate = OpenOptionsPanel;
            _visualElementInitializer = visualElementInitializer;
            _resourceAssetLoader = resourceAssetLoader;

            _bundle = AssetBundle.LoadFromFile(@"D:\Ohjelmat\Steam\steamapps\common\Timberborn\BepInEx\plugins\ModManager\assets\what.bundle");
        }

        public VisualElement GetPanel()
        {
            //var root = _visualElementLoader.LoadVisualElement("Mods/ModsBox");

            //var root2 = _resourceAssetLoader.Load<GameObject>($"ModManager/what/Cube");
            //Console.WriteLine($"loaded Cube");

            foreach (var item in _bundle.GetAllAssetNames())
            {
                Console.WriteLine($"\titem: {item}");
                //_bundle.LoadAsset(item);
            }

            string assName = "assets/resources/ui/views/mods/modsbox.uxml";
            Console.WriteLine($"loading asset \"{assName}\"");
            var ass = _bundle.LoadAsset<VisualTreeAsset>(assName);
            Console.WriteLine($"ass: {ass.name}");

            var root = LoadVisualElement(ass);
            Console.WriteLine($"root: {root.name}");

            Console.WriteLine("finish bundle");



            //var root3 = LoadVisualElement("ui/views/mods/newuxmltemplate.uxml");


            //var root = LoadVisualElement("ModsBox");
            //Console.WriteLine($"FOO2");
            //Console.WriteLine($"root: {root}");
            _mods = root.Q<ScrollView>("Mods");
            _loading = root.Q<Label>("Loading");
            _error = root.Q<Label>("Error");
            _tags = root.Q<RadioButtonGroup>("Tags");

            ShowModsAndTags();

            root.Q<Button>("Close").clicked += OnUICancelled;
            root.Q<Button>("SearchButton").clicked += UpdateMods;
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
        }

        private void ShowModsAndTags()
        {
            _loading.ToggleDisplayStyle(true);
            _error.ToggleDisplayStyle(false);
            _mods.Clear();

            var getModsTask = _modService.GetMods().Search(_filter).ToList();
            getModsTask.ConfigureAwait(true).GetAwaiter()
                .OnCompleted(() => OnModsRetrieved(getModsTask));

            var getTagsTask = _modService.GetTags().Get();
            getTagsTask.ConfigureAwait(true).GetAwaiter()
                .OnCompleted(() => OnTagsRetrieved(getTagsTask));
        }

        private void UpdateMods()
        {
            _filter = ModFilter.Downloads.Desc();

            if (!string.IsNullOrEmpty(_search.value))
            {
                _filter = _filter.And(ModFilter.FullText.Eq(_search.value));
            }

            if (_tags.value != -1)
            {
                _filter = _filter.And(ModFilter.Tags.Eq(_tagOptions[_tags.value]));
            }

            ShowModsAndTags();
        }

        private void OnTagsRetrieved(Task<IReadOnlyList<TagOption>> task)
        {
            _tagOptions = task.Result.SelectMany(tagGroup => tagGroup.Tags).ToList();
            _tags.choices = _tagOptions;

            _tags.RegisterValueChangedCallback(_ => UpdateMods());
        }

        private void OnModsRetrieved(Task<IReadOnlyList<Mod>> task)
        {
            try
            {
                FillTheWrapper(task.Result);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private void FillTheWrapper(IReadOnlyCollection<Mod> mods)
        {
            _loading.ToggleDisplayStyle(false);
            foreach (var mod in mods)
            {

                string assName = "assets/resources/ui/views/mods/modsboxitem.uxml";
                //Console.WriteLine($"loading asset \"{assName}\"");
                var ass = _bundle.LoadAsset<VisualTreeAsset>(assName);
                //Console.WriteLine($"ass: {ass.name}");

                //var item = _visualElementLoader.LoadVisualElement("Mods/ModsBoxItem");

                var item = _visualElementLoader.LoadVisualElement(ass);
                item.Q<Label>("Name").text = mod.Name;
                item.Q<Button>("Download").clicked += () => _modService.DownloadLatestMod(mod.Id);

                SetNumbers(mod, item);
                LoadImage(mod, item.Q<Image>("Logo"));

                _mods.Add(item);
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

        private void LoadImage(Mod mod, Image root)
        {
            if (mod.Logo != null)
            {
                root.image = _modService.GetImage(mod.Logo.Thumb320x180, 320, 180);
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