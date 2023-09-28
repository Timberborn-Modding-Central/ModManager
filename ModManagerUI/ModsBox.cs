using Modio.Filters;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.ManifestValidatorSystem;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using ModManagerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Timberborn.Core;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;
using TextField = UnityEngine.UIElements.TextField;

namespace Timberborn.ModsSystemUI
{
    enum InstalledOptions
    {
        All,
        Installed,
        Uninstalled,
        UpdateAvailable
    }
    enum EnabledOptions
    {
        Both,
        Enabled,
        NotEnabled
    }

    public class ModsBox : IPanelController
    {
        public static bool ModsWereChanged = false;
        public static Action OpenOptionsDelegate;

        private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            VisualElement visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            _visualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }

        private static readonly string AllLocKey = "Mods.Tags.All";
        private static readonly uint ModsPerPage = 25;
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
        private RadioButtonGroup _installedOptions;
        private RadioButtonGroup _enabledOptions;
        private Button _showMore;
        private Button _newest;
        private Button _lastUpdated;
        private Button _mostDownloaded;
        private Button _topRated;
        private Label _updateAllLabel;
        private VisualElement _updateAllWrapper;
        private Button _updateAllButton;
        private List<string> _tagOptions = new();
        private List<string> _installedOptionsOptions = new();
        private List<string> _enabledOptionsOptions = new();
        private uint _page;
        private readonly InstalledAddonRepository _installedAddonRepository;
        private readonly DialogBoxShower _dialogBoxShower;

        private Dictionary<uint, Mod> _updateAvailable = new();

        private string _activeSortButton = "MostDownloaded";

        private const string _bundleName = "modmanagerui.bundle";
        public static AssetBundle _bundle;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken _token = _cancellationTokenSource.Token;
        private static CancellationTokenSource _cancellationTokenSource2 = new CancellationTokenSource();
        private static CancellationToken _token2 = _cancellationTokenSource2.Token;

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
            _installedOptions = root.Q<RadioButtonGroup>("Options");
            _enabledOptions = root.Q<RadioButtonGroup>("EnabledOptions");
            _showMore = root.Q<Button>("ShowMore");
            _showMore.ToggleDisplayStyle(false);
            _newest = root.Q<Button>("Newest");
            _mostDownloaded = root.Q<Button>("MostDownloaded");
            _topRated = root.Q<Button>("TopRated");
            _lastUpdated = root.Q<Button>("LastUpdated");
            _updateAllLabel = root.Q<Label>("UpdateAllLabel");
            _updateAllWrapper = root.Q<VisualElement>("UpdateAllWrapper");
            _updateAllButton = root.Q<Button>("UpdateAll");

            _newest.clicked += () => SetActiveSortButton("Newest");
            _mostDownloaded.clicked += () => SetActiveSortButton("MostDownloaded");
            _topRated.clicked += () => SetActiveSortButton("TopRated");
            _lastUpdated.clicked += () => SetActiveSortButton("LastUpdated");
            _updateAllButton.clicked += async () => await UpdateUpdatableMods();

            SetUpdateAllVisibility(false);

            ShowModsAndTags();
            ShowTags();

            root.Q<Button>("Close").clicked += OnUICancelled;
            root.Q<Button>("SearchButton").clicked += UpdateMods;
            _showMore.clicked += ShowMoreMods;
            _search = root.Q<TextField>("Search");
            _search.isDelayed = true;
            _search.RegisterValueChangedCallback(_ => UpdateMods());
            PopulateInstalledOptions();
            PopulateEnabledOptions();

            return root;
        }

        private async void ShowTags()
        {
            Task<IReadOnlyList<TagOption>> getTagsTask = _addonService.GetTags().Get();

            OnTagsRetrieved(await getTagsTask);
        }

        private async Task UpdateUpdatableMods()
        {
            _updateAllButton.SetEnabled(false);
            var modList = new Dictionary<uint, Mod>(_updateAvailable);
            foreach (KeyValuePair<uint, Mod> updatableMod in modList)
            {
                try
                {
                    (string location, Mod Mod) mod = await _addonService.DownloadLatest(updatableMod.Value);
                    TryInstall(mod);
                    _updateAvailable.Remove(updatableMod.Key);
                }
                catch (MapException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning(ex.Message);
                }
                catch (AddonException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning(ex.Message);
                }
                catch (IOException ex)
                {
                    ModManagerUIPlugin.Log.LogError($"{ex.Message}");
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _updateAllButton.SetEnabled(true);
            SetUpdateAllVisibility();
        }

        private void SetUpdateAllVisibility()
        {
            bool updateAllVisible = _updateAvailable.Count > 0 
                ? true 
                : false;
            SetUpdateAllVisibility(updateAllVisible);
        }

        private void SetUpdateAllVisibility(bool visible)
        {
            _updateAllWrapper.ToggleDisplayStyle(visible);
            foreach (VisualElement child in _updateAllWrapper.Children())
            {
                child.ToggleDisplayStyle(visible);
            }
        }

        private async Task PopulateUpdatableMods(CancellationToken token)
        {
            _updateAvailable.Clear();
            var installedMods = _installedAddonRepository.All().ToList();
            foreach (Manifest manifest in installedMods)
            {
                Mod mod;
                try
                {
                    mod = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[manifest.ModId].Get();
                }
                catch (Exception)
                {
                    continue;
                }
                if (mod.Modfile.Version != manifest.Version &&
                    VersionComparer.IsVersionHigher(mod.Modfile.Version, manifest.Version))
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    _updateAvailable.Add(mod.Id, mod);
                }
            }

            _updateAllLabel.text = _loc.T("Mods.UpdateAllLabel", _updateAvailable.Count);
        }

        private void OpenOptionsPanel()
        {
            ManifestValidatorService.Instance.ValidateManifests();
            _panelStack.HideAndPush(this);
        }

        public bool OnUIConfirmed()
        {
            return false;
        }

        public void OnUICancelled()
        {
            if (ModsWereChanged)
            {
                _dialogBoxShower.Create()
                                .SetLocalizedMessage("Mods.ModsChanged")
                                .SetConfirmButton(GameQuitter.Quit, _loc.T("Mods.Quit"))
                                .SetCancelButton(() => _panelStack.Pop(this), _loc.T("Mods.Stay"))
                                .Show();
            }
            else
            {
                _panelStack.Pop(this);
            }
        }

        private Filter Filter => _filter.And(Filter.WithLimit(ModsPerPage).Offset(_page * ModsPerPage));

        private async void ShowModsAndTags()
        {
            _cancellationTokenSource2.Cancel();
            _cancellationTokenSource2 = new CancellationTokenSource();
            _token2 = _cancellationTokenSource2.Token;
            try
            {
                var populateModsTask = PopulateUpdatableMods(_token2);
                _mods.Clear();
                _page = 0;
                Task modsTask = ShowMods();
                await populateModsTask;
                SetUpdateAllVisibility();
            }
            catch (OperationCanceledException ex)
            {
                ModManagerUIPlugin.Log.LogWarning($"Async operation was cancelled: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

            switch (_activeSortButton)
            {
                case "MostDownloaded":
                    _filter = ModFilter.Downloads.Desc();
                    break;
                case "Newest":
                    _filter = ModFilter.DateAdded.Desc();
                    break;
                case "TopRated":
                    _filter = ModFilter.Rating.Desc();
                    break;
                case "LastUpdated":
                    _filter = ModFilter.DateUpdated.Desc();
                    break;
            }

            if (!string.IsNullOrEmpty(_search.value))
            {
                _filter = _filter.And(ModFilter.FullText.Eq(_search.value));
            }

            if (_tags.value > 0)
            {
                _filter = _filter.And(ModFilter.Tags.Eq(_tagOptions[_tags.value]));
            }


            if (_installedOptions.value >= 0)
            {
                switch (_installedOptionsOptions[_installedOptions.value])
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
                    case nameof(InstalledOptions.UpdateAvailable):
                        var updateAvailableNames = _updateAvailable.Select(x => x.Value.Name ?? "");
                        if (updateAvailableNames.Any())
                        {
                            var modFilter3 = ModFilter.Name.In(updateAvailableNames);
                            _filter = _filter.And(modFilter3);
                        }
                        else
                        {
                            // HACK: Dummy filter to ensure results is 0
                            _filter = _filter.And(ModFilter.Id.Eq(1));
                        }
                        break;
                    default:
                        break;
                }
            }
            if (_enabledOptions.value >= 0)
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

            ShowModsAndTags();
        }

        private void PopulateInstalledOptions()
        {
            _installedOptionsOptions.Clear();
            _installedOptionsOptions.AddRange(Enum.GetNames(typeof(InstalledOptions)));
            _installedOptions.choices = _installedOptionsOptions;

            _installedOptions.RegisterValueChangedCallback(_ => UpdateMods());
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
            catch (OperationCanceledException ex)
            {
                ModManagerUIPlugin.Log.LogWarning($"Async operation was cancelled: {ex.Message}");
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
                if (!_installedAddonRepository.Has(mod.Id) || mod.Name == "BepInExPack")
                {
                    uninstallButton.visible = false;
                }
                var downloadButton = item.Q<Button>("Download");
                downloadButton.clicked += async () => await DoDownloadAndExtract(mod, downloadButton, installedToggle, enabledToggle, uninstallButton);

                SetNumbers(mod, item);

                _mods.Add(item);
            }

            foreach (var mod in mods)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                await LoadImage(mod, _mods.Children().Where(x => x.Q<Label>("Name").text == mod.Name).First().Q<Image>("Logo"));
            }
        }

        private void ToggleEnabled(ChangeEvent<bool> changeEvent, Mod mod, Toggle enabledToggle)
        {
            if (mod.Name == "BepInExPack")
            {
                enabledToggle.SetValueWithoutNotify(true);
                ModManagerUIPlugin.Log.LogWarning("Disabling BepInEx is not allowed.");
                return;
            }
            ModsWereChanged = true;
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
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
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
            uninstallButton.SetEnabled(false);
            ModsWereChanged = true;
            try
            {
                _addonService.Uninstall(modInfo.Id);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
            isInstalledToggle.SetValueWithoutNotify(false);
            isEnabledToggle.SetValueWithoutNotify(false);
            uninstallButton.visible = false;
            uninstallButton.SetEnabled(true);
        }

        private async Task DoDownloadAndExtract(Mod modInfo, Button downloadButton, Toggle isInstalledToggle, Toggle isEnabledToggle, Button uninstallButton)
        {
            try
            {
                downloadButton.SetEnabled(false);
                ModsWereChanged = true;
                (string location, Mod Mod) mod = await _addonService.DownloadLatest(modInfo);
                TryInstall(mod, isInstalledToggle, isEnabledToggle, uninstallButton, downloadButton);
                await foreach ((string location, Mod Mod) dependency in _addonService.DownloadDependencies(modInfo))
                {
                    try
                    {
                        TryInstall(dependency, isInstalledToggle, isEnabledToggle, uninstallButton, downloadButton);
                        var depVisualElement = _mods.Children()
                                                    .Where(x => x.Q<Label>("Name").text == dependency.Mod.Name)
                                                    .FirstOrDefault();
                        if (depVisualElement != null)
                        {
                            depVisualElement.Q<Toggle>("Installed").SetValueWithoutNotify(true);
                            depVisualElement.Q<Toggle>("Enabled").SetValueWithoutNotify(true);
                        }
                    }
                    catch (MapException ex)
                    {
                        ModManagerUIPlugin.Log.LogWarning(ex.Message);
                    }
                    catch (AddonException ex)
                    {
                        ModManagerUIPlugin.Log.LogWarning(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        ModManagerUIPlugin.Log.LogError($"{ex.Message}");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (MapException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (IOException ex)
            {
                ModManagerUIPlugin.Log.LogError($"{ex.Message}");
            }
            catch (Exception)
            {
                throw;
            }
            downloadButton.SetEnabled(true);
        }

        private void TryInstall((string location, Mod Mod) mod)
        {
            var modVisualElement = _mods.Children()
                                         .Where(x => x.Q<Label>("Name").text == mod.Mod.Name)
                                         .FirstOrDefault();
            var installedToggle = modVisualElement?.Q<Toggle>("Installed");
            var enabledToggle = modVisualElement?.Q<Toggle>("Enabled");
            var uninstallButton = modVisualElement?.Q<Button>("Uninstall");
            var downloadButton = modVisualElement?.Q<Button>("Download");
            TryInstall(mod, installedToggle, enabledToggle, uninstallButton, downloadButton);
        }

        private void TryInstall((string location, Mod Mod) mod, Toggle isInstalledToggle, Toggle isEnabledToggle, Button uninstallButton, Button downloadButton)
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
                isInstalledToggle?.SetValueWithoutNotify(true);
                isEnabledToggle?.SetValueWithoutNotify(true);
                if (uninstallButton != null)
                {
                    uninstallButton.visible = true;
                }
                if (downloadButton != null)
                {
                    downloadButton.text = _loc.T("Mods.Download");
                }
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
                try
                {
                    var byteArray = await _addonService.GetImage(mod.Logo.Thumb320x180);
                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(byteArray);
                    root.image = texture;
                }
                catch (HttpRequestException ex)
                {
                    ModManagerUIPlugin.Log.LogWarning($"Error occured while fetching image: {ex.Message}");
                }
                catch (Exception)
                {
                    throw;
                }
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

        private void SetActiveSortButton(string activeButton)
        {
            _newest.EnableInClassList("mod-box__sort-button--selected", activeButton == "Newest" ? true : false);
            _mostDownloaded.EnableInClassList("mod-box__sort-button--selected", activeButton == "MostDownloaded" ? true : false);
            _topRated.EnableInClassList("mod-box__sort-button--selected", activeButton == "TopRated" ? true : false);
            _lastUpdated.EnableInClassList("mod-box__sort-button--selected", activeButton == "LastUpdated" ? true : false);

            if (_activeSortButton != activeButton)
            {
                _activeSortButton = activeButton;

                UpdateMods();
            }
        }
    }
}