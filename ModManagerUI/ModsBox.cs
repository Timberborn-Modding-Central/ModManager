using Modio.Filters;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManagerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timberborn.Core;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;
using TextField = UnityEngine.UIElements.TextField;

namespace Timberborn.ModsSystemUI
{
    enum InstalledOptions
    {
        Both,
        Installed,
        Uninstalled
    }
    enum EnabledOptions
    {
        Both,
        Enabled,
        NotEnabled
    }

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
        private RadioButtonGroup _options;
        private RadioButtonGroup _enabledOptions;
        private Button _showMore;
        private List<string> _tagOptions = new();
        private List<string> _optionsOptions = new();
        private List<string> _enabledOptionsOptions = new();
        private uint _page;
        private readonly InstalledAddonRepository _installedAddonRepository;

        private bool _modsWereChanged = false;


        private readonly DialogBoxShower _dialogBoxShower;

        private const string _bundleName = "modmanagerui.bundle";
        public static AssetBundle _bundle;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken _token = _cancellationTokenSource.Token;

        public ModsBox(VisualElementLoader visualElementLoader,
                       PanelStack panelStack,
                       IAddonService addonService,
                       VisualElementInitializer visualElementInitializer,
                       ModFullInfoController modFullInfoController,
                       ILoc loc,
                       InstalledAddonRepository installedAddonRepository,
                       DialogBoxShower dialogBoxShower)
        {
            _visualElementLoader = visualElementLoader;
            _panelStack = panelStack;
            _addonService = addonService;
            OpenOptionsDelegate = OpenOptionsPanel;
            _visualElementInitializer = visualElementInitializer;
            _modFullInfoController = modFullInfoController;
            _loc = loc;
            _installedAddonRepository = installedAddonRepository;
            _dialogBoxShower = dialogBoxShower;

            if (_bundle == null)
            {
                _bundle = AssetBundle.LoadFromFile($"{Path.Combine(UIPaths.ModManagerUI.Assets, _bundleName)}");
            }
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
            _options = root.Q<RadioButtonGroup>("Options");
            _enabledOptions = root.Q<RadioButtonGroup>("EnabledOptions");
            _showMore = root.Q<Button>("ShowMore");
            _showMore.ToggleDisplayStyle(false);

            ShowModsAndTags();

            root.Q<Button>("Close").clicked += OnUICancelled;
            root.Q<Button>("SearchButton").clicked += UpdateMods;
            _showMore.clicked += ShowMoreMods;
            _search = root.Q<TextField>("Search");
            _search.isDelayed = true;
            _search.RegisterValueChangedCallback(_ => UpdateMods());
            PopulateSpecialOptions();
            PopulateEnabledOptions();
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
            if (_modsWereChanged)
            {
                _dialogBoxShower.Show(_loc.T("Mods.ModsChanged"),
                                      GameQuitter.Quit, _loc.T("Mods.Quit"),
                                      () => _panelStack.Pop(this), _loc.T("Mods.Stay"));
            }
            else
            {
                _panelStack.Pop(this);
            }
        }

        private Filter Filter => _filter.And(Filter.WithLimit(ModsPerPage).Offset(_page * ModsPerPage));

        private async void ShowModsAndTags()
        {
            _mods.Clear();
            _page = 0;

            Task modsTask = ShowMods();

            Task<IReadOnlyList<TagOption>> getTagsTask = _addonService.GetTags().Get();
            OnTagsRetrieved(await getTagsTask);
        }

        private void ShowMoreMods()
        {
            _page++;
            Task modsTask = ShowMods();
        }

        private async Task ShowMods()
        {
            _loading.ToggleDisplayStyle(true);
            _error.ToggleDisplayStyle(false);

            var getModsTask = _addonService.GetMods().Search(Filter).FirstPage();
            Task modsRetrieved = OnModsRetrieved(await getModsTask);
        }

        private void UpdateMods()
        {
            _cancellationTokenSource.Cancel();
            _filter = ModFilter.Downloads.Desc();

            if (!string.IsNullOrEmpty(_search.value))
            {
                _filter = _filter.And(ModFilter.FullText.Eq(_search.value));
            }

            if (_tags.value > 0)
            {
                _filter = _filter.And(ModFilter.Tags.Eq(_tagOptions[_tags.value]));
            }


            if (_options.value >= 0)
            {
                switch (_optionsOptions[_options.value])
                {
                    case nameof(InstalledOptions.Installed):
                        var installedModNames = _installedAddonRepository.All()
                                                                         .Select(x => x.ModName)
                                                                         .ToArray();
                        var modFilter = ModFilter.Name.In(installedModNames);
                        _filter = _filter.And(modFilter);
                        break;
                    case nameof(InstalledOptions.Uninstalled):
                        var uninstalledModNames = _installedAddonRepository.All()
                                                                           .Select(x => x.ModName)
                                                                           .ToArray();
                        var modFilter2 = ModFilter.Name.NotIn(uninstalledModNames);
                        _filter = _filter.And(modFilter2);
                        break;
                    default:
                        break;
                }
                if(_enabledOptions.value >= 0)
                {
                    switch (_enabledOptionsOptions[_enabledOptions.value])
                    {
                        case nameof(EnabledOptions.Enabled):
                            var enabledModNames = _installedAddonRepository.All()
                                                                           .Where(x => x.Enabled)
                                                                           .Select(x => x.ModName)
                                                                           .ToArray();
                            var modFilter3 = ModFilter.Name.In(enabledModNames);
                            _filter = _filter.And(modFilter3);
                            break;
                        case nameof(EnabledOptions.NotEnabled):
                            var notEnabledModNames = _installedAddonRepository.All()
                                                                              .Where(x => x.Enabled)
                                                                              .Select(x => x.ModName)
                                                                              .ToArray();
                            var modFilter4 = ModFilter.Name.NotIn(notEnabledModNames);
                            _filter = _filter.And(modFilter4);
                            break;
                        default:
                            break;
                    }
                }
            }

            ShowModsAndTags();
        }

        private void PopulateSpecialOptions()
        {
            _optionsOptions.Clear();
            _optionsOptions.AddRange(Enum.GetNames(typeof(InstalledOptions)));
            _options.choices = _optionsOptions;

            _options.RegisterValueChangedCallback(_ => UpdateMods());
        }

        private void PopulateEnabledOptions()
        {
            _enabledOptionsOptions.Clear();
            _enabledOptionsOptions.AddRange(Enum.GetNames(typeof(EnabledOptions)));
            _enabledOptions.choices = _enabledOptionsOptions;

            _enabledOptions.RegisterValueChangedCallback(_ => UpdateMods());
        }

        private void OnTagsRetrieved(IReadOnlyList<TagOption> task)
        {
            _tagOptions.Clear();
            _tagOptions.Add(_loc.T(AllLocKey));
            _tagOptions.AddRange(task.SelectMany(tagGroup => tagGroup.Tags));
            _tags.choices = _tagOptions;

            _tags.RegisterValueChangedCallback(_ => UpdateMods());
        }

        private async Task OnModsRetrieved(IReadOnlyList<Mod> task)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            try
            {
                await FillTheWrapper(task, _token);
                if (task.Count < ModsPerPage)
                {
                    _showMore.ToggleDisplayStyle(false);
                }
            }
            catch(OperationCanceledException ex)
            {
                ModManagerUIPlugin.Log.LogWarning($"{ex.Message}");
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private async Task FillTheWrapper(IReadOnlyCollection<Mod> mods, CancellationToken token)
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

                var installedToggle = item.Q<Toggle>("Installed");
                bool isInstalled = _installedAddonRepository.Has(mod.Id)
                    ? true
                    : false;
                installedToggle.SetValueWithoutNotify(isInstalled);
                // HACK: using SetEnabled(false) greys the Toggle and dunno how to fix that ;_;
                installedToggle.RegisterValueChangedCallback((changeEvent) => installedToggle.SetValueWithoutNotify(changeEvent.previousValue));

                var enabledToggle = item.Q<Toggle>("Enabled");
                bool modIsEnabled = false;
                if (_installedAddonRepository.TryGet(mod.Id, out Manifest manifest))
                {
                    modIsEnabled = manifest.Enabled;
                    var latestVersion = mod.Modfile.Version;
                    if (VersionComparer.IsVersionHigher(latestVersion, manifest.Version))
                    {
                        item.Q<Button>("Download").text = "Update";
                    }
                }
                enabledToggle.value = modIsEnabled;
                enabledToggle.RegisterValueChangedCallback((changeEvent) => ToggleEnabled(changeEvent, mod, enabledToggle));

                var uninstallButton = item.Q<Button>("Uninstall");
                uninstallButton.clicked += () => DoUninstall(mod, installedToggle, enabledToggle, uninstallButton);
                if(!_installedAddonRepository.Has(mod.Id))
                {
                    uninstallButton.visible = false;
                }
                item.Q<Button>("Download").clicked += async () => await DoDownloadAndExtract(mod, installedToggle, uninstallButton);

                SetNumbers(mod, item);

                _mods.Add(item);
            }

            foreach (var mod in mods)
            {
                if(token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                await LoadImage(mod, _mods.Children().Where(x => x.Q<Label>("Name").text == mod.Name).First().Q<Image>("Logo"));
            }
        }

        private void ToggleEnabled(ChangeEvent<bool> changeEvent, Mod mod, Toggle enabledToggle)
        {
            _modsWereChanged = true;
            try
            {
                if (changeEvent.newValue == true)
                {
                    _addonService.Enable(mod.Id);
                }
                else
                {
                    _addonService.Disable(mod.Id);
                }
            }
            catch (AddonException ex)
            {
                enabledToggle.SetValueWithoutNotify(changeEvent.previousValue);
                Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ShowFullInfo(Mod mod)
        {
            _panelStack.HideAndPush(_modFullInfoController);
            _modFullInfoController.SetMod(mod);
        }

        private void DoUninstall(Mod modInfo, Toggle isInstalledToggle, Toggle isEnabledToggle, Button uninstallButton)
        {
            _modsWereChanged = true;
            _addonService.Uninstall(modInfo.Id);
            isInstalledToggle.SetValueWithoutNotify(false);
            isEnabledToggle.SetValueWithoutNotify(false);
            uninstallButton.visible = false;
        }

        private async Task DoDownloadAndExtract(Mod modInfo, Toggle isInstalledToggle, Button uninstallButton)
        {
            _modsWereChanged = true;
            (string location, Mod Mod) mod = await _addonService.DownloadLatest(modInfo);
            TryInstall(mod, isInstalledToggle, uninstallButton);

            await foreach ((string location, Mod Mod) dependency in _addonService.DownloadDependencies(modInfo))
            {
                TryInstall(dependency, isInstalledToggle, uninstallButton);
            }
        }

        private void TryInstall((string location, Mod Mod) mod, Toggle isInstalledToggle, Button uninstallButton)
        {
            try
            {
                if (_installedAddonRepository.TryGet(mod.Mod.Id, out Manifest manifest) &&
                   manifest.Version != mod.Mod.Modfile.Version)
                {
                    _addonService.ChangeVersion(mod.Mod, mod.Mod.Modfile, mod.location);
                }
                else
                {
                    _addonService.Install(mod.Mod, mod.location);
                }
                isInstalledToggle.SetValueWithoutNotify(true);
                uninstallButton.visible = true;
            }
            catch (MapException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (Exception)
            {
                throw;
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