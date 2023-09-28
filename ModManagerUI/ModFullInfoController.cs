using Modio;
using Modio.Filters;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using ModManagerUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Timberborn.ModsSystemUI
{
    public class ModFullInfoController : IPanelController
    {
        public ImmutableArray<string> VersionsString;
        public ImmutableArray<File> Versions;
        public File CurrentFile;

        private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            VisualElement visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            _visualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }

        private static readonly string ImageClass = "mods-box-full-item__image";
        private readonly VisualElementInitializer _visualElementInitializer;
        private readonly PanelStack _panelStack;
        private readonly IAddonService _addonService;
        private readonly VisualElement _item = new();
        private readonly InstalledAddonRepository _installedAddonRepository;

        private Mod _currentMod;
        private Dropdown _versionsDropdown;
        private readonly DropdownItemsSetter _dropdownOptionsSetter;

        private AssetBundle _bundle;

        public ModFullInfoController(PanelStack panelStack,
                                     IAddonService addonService,
                                     VisualElementInitializer visualElementInitializer,
                                     InstalledAddonRepository installedAddonRepository,
                                     DropdownItemsSetter dropdownOptionsSetter)
        {
            _dropdownOptionsSetter = dropdownOptionsSetter;
            _panelStack = panelStack;
            _addonService = addonService;
            _visualElementInitializer = visualElementInitializer;
            _installedAddonRepository = installedAddonRepository;
        }

        public VisualElement GetPanel()
        {
            _item.AddToClassList("content-row-centered");
            return _item;
        }

        public async Task SetVersions(Dropdown dropdown)
        {
            FilesClient filesCLient = _addonService.GetFiles(_currentMod);
            var versions = filesCLient.Search(FileFilter.Version.Desc()).ToEnumerable();

            var foo = new List<File>();
            await foreach (var version in versions)
            {
                foo.Add(version);
            }
            Versions = foo.ToImmutableArray();

            _dropdownOptionsSetter.SetItems(
                dropdown,
                Versions.Select(x => x.Version ?? ""),
                () => CurrentFile.Version ?? "",
                delegate (string value)
                {
                    SetVersion(value);
                });
        }

        private void SetVersion(string value)
        {
            File currFile = Versions.Where(x => (x.Version ?? "") == value).SingleOrDefault();
            CurrentFile = currFile;
            _versionsDropdown.RefreshContent();
        }

        public void SetMod(Mod mod)
        {
            _currentMod = mod;
            CurrentFile = mod.Modfile;

            string assetName = "assets/resources/ui/views/mods/ModsBoxFullItem.uxml";
            var asset = ModsBox._bundle.LoadAsset<VisualTreeAsset>(assetName);
            var item = LoadVisualElement(asset);

            _versionsDropdown = item.Q<Dropdown>("Versions");
            SetVersions(_versionsDropdown);

            item.Q<Button>("Close").clicked += OnUICancelled;
            item.Q<Label>("Name").text = mod.Name;
            item.Q<Label>("AuthorName").text = mod.SubmittedBy?.Username;
            item.Q<Label>("Tags").text = string.Join(", ", mod.Tags.Select(tag => tag.Name));
            item.Q<Label>("Description").text = string.IsNullOrEmpty(mod.DescriptionPlaintext)
                ? mod.Summary
                : mod.DescriptionPlaintext;
            Label installedVersion = item.Q<Label>("InstalledVersion");
            installedVersion.text = _installedAddonRepository.Has(mod.Id)
                ? _installedAddonRepository.Get(mod.Id).Version
                : "-";
            item.Q<Label>("LiveVersion").text = mod?.Modfile?.Version ?? "";

            var depTask = SetDependencies(item, mod);

            var downloadButton = item.Q<Button>("Download");
            downloadButton.clicked += async () => await DoDownloadAndExtract(mod, downloadButton, installedVersion);

            LoadLogo(mod, item.Q<Image>("Logo"));
            SetNumbers(mod, item);
            AddImages(mod, item.Q<ScrollView>("Description"));
            _item.Add(item);
        }

        public bool OnUIConfirmed()
        {
            return false;
        }

        public void OnUICancelled()
        {
            _currentMod = null;
            _item.Clear();
            _panelStack.Pop(this);
        }

        private async void LoadLogo(Mod mod, Image root)
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
                catch (Exception ex)
                {
                    throw;
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

        private async void AddImages(Mod mod, VisualElement root)
        {
            var images = mod.Media.Images;
            if (images.Count > 0)
            {
                foreach (var image in images)
                {
                    await AddImage(image.Original, root);
                }
            }
            else if (mod.Logo != null)
            {
                await AddImage(mod.Logo.Original, root);
            }
        }


        private async Task AddImage(Uri image, VisualElement root)
        {
            try
            {
                var byteArray = await _addonService.GetImage(image);
                var texture = new Texture2D(1, 1);
                texture.LoadImage(byteArray);
                var imageElement = new Image
                {
                    image = texture
                };
                imageElement.AddToClassList(ImageClass);
                const float maxWidth = 1000;
                if (texture.width > maxWidth)
                {
                    imageElement.style.width = maxWidth;
                    var scaleFactor = maxWidth / texture.width;
                    imageElement.style.height = texture.height * scaleFactor;
                }
                root.Add(imageElement);
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

        private static string Format(uint number)
        {
            //return NumberFormatter.Format((int) number);
            return number.ToString();
        }

        private async Task SetDependencies(VisualElement item, Mod mod)
        {
            var deps = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[mod.Id].Dependencies.Get();

            List<string> dependencyNames = new();
            foreach (var dep in deps)
            {
                dependencyNames.Add((await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[dep.ModId].Get()).Name);
            }

            item.Q<Label>("Dependencies").text = dependencyNames.Count() > 0
                ? string.Join(Environment.NewLine, dependencyNames)
                : "-";
        }

        private async Task DoDownloadAndExtract(Mod modInfo, Button downloadButton, Label installedVersion)
        {
            downloadButton.SetEnabled(false);
            ModsBox.ModsWereChanged = true;
            try
            {
                (string location, Mod Mod) mod = await _addonService.Download(modInfo, CurrentFile);
                TryInstall(mod, installedVersion);
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
            await foreach ((string location, Mod Mod) dependency in _addonService.DownloadDependencies(modInfo))
            {
                try
                {
                    TryInstall(dependency, installedVersion);
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
            downloadButton.SetEnabled(true);
        }

        private void TryInstall((string location, Mod Mod) mod, Label installedVersion)
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
                if (_item.Q<Label>("Name").text == mod.Mod.Name)
                {
                    installedVersion.text = mod.Mod.Modfile.Version;
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

    }
}