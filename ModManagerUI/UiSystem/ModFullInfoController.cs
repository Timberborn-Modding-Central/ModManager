using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManagerUI.Components.ModFullInfo;
using ModManagerUI.EventSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace ModManagerUI.UiSystem
{
    public class ModFullInfoController : IPanelController
    {
        private static readonly string ImageClass = "mods-box-full-item__image";
        
        private readonly VisualElementInitializer _visualElementInitializer;
        private readonly InstalledAddonRepository _installedAddonRepository;
        private readonly DropdownItemsSetter _dropdownOptionsSetter;
        private readonly IAddonService _addonService;
        private readonly PanelStack _panelStack;
        
        private readonly VisualElement _item = new();
        
        private Mod? _currentMod;
        private Dropdown? _versionsDropdown;
        private DownloadButton? _downloadButton;
        
        public File? CurrentFile;

        public ModFullInfoController(
            VisualElementInitializer visualElementInitializer,
            InstalledAddonRepository installedAddonRepository,
            DropdownItemsSetter dropdownOptionsSetter,
            IAddonService addonService,
            PanelStack panelStack)
        {
            _visualElementInitializer = visualElementInitializer;
            _installedAddonRepository = installedAddonRepository;
            _dropdownOptionsSetter = dropdownOptionsSetter;
            _addonService = addonService;
            _panelStack = panelStack;
        }

        public VisualElement GetPanel()
        {
            EventBus.Instance.PostEvent(new ModsBoxFullInfoOpenedEvent());
            _item.AddToClassList("content-row-centered");
            return _item;
        }

        public bool OnUIConfirmed()
        {
            EventBus.Instance.PostEvent(new ModsBoxFullInfoClosedEvent());
            return false;
        }

        public void OnUICancelled()
        {
            _currentMod = null;
            CurrentFile = null;
            _item.Clear();
            _panelStack.Pop(this);
            EventBus.Instance.PostEvent(new ModsBoxFullInfoClosedEvent());
        }
        
        public async void SetMod(Mod mod)
        {
            _currentMod = mod;
            CurrentFile = mod.Modfile;

            var assetName = "assets/resources/ui/views/mods/ModsBoxFullItem.uxml";
            var asset = AssetBundleLoader.AssetBundle.LoadAsset<VisualTreeAsset>(assetName);
            var item = LoadVisualElement(asset);

            _versionsDropdown = item.Q<Dropdown>("Versions");
            await SetVersions(_versionsDropdown);

            item.Q<Button>("Close").clicked += OnUICancelled;
            item.Q<Label>("Name").text = mod.Name;
            item.Q<Label>("AuthorName").text = mod.SubmittedBy?.Username;
            item.Q<Label>("Tags").text = string.Join(", ", mod.Tags.Select(tag => tag.Name));
            item.Q<Label>("Description").text = string.IsNullOrEmpty(mod.DescriptionPlaintext)
                ? mod.Summary
                : mod.DescriptionPlaintext;
            var installedVersion = item.Q<Label>("InstalledVersion");
            installedVersion.text = _installedAddonRepository.Has(mod.Id)
                ? _installedAddonRepository.Get(mod.Id).Version
                : "-";
            item.Q<Label>("LiveVersion").text = mod.Modfile?.Version ?? "";

            _downloadButton = DownloadButton.Create(item.Q<Button>("Download"), mod, this);

            LoadLogo(mod, item.Q<Image>("Logo"));
            SetNumbers(mod, item);
            AddImages(mod, item.Q<ScrollView>("Description"));
            _item.Add(item);
            
            Refresh();
        }

        public void Refresh()
        {
            _downloadButton?.Refresh();
        }
        
        private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            var visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            _visualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }

        private async Task SetVersions(Dropdown dropdown)
        {
            if (_currentMod == null)
                return;
            var versionList = await ModIoModFilesRegistry.GetDescAsync(_currentMod.Id);
            var dropdownProvider = new VersionDropdownProvider(this, versionList.ToList());
            _dropdownOptionsSetter.SetItems(dropdown, dropdownProvider);
            _versionsDropdown?.RefreshContent();
        }

        private async void LoadLogo(Mod mod, Image root)
        {
            if (mod.Logo == null || mod.Logo.Thumb320x180 == null) 
                return;
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
        }

        private static void SetNumbers(Mod mod, VisualElement item)
        {
            if (mod.Stats == null) 
                return;
            item.Q<Label>("UpCount").text = Format(mod.Stats.PositiveRatings);
            item.Q<Label>("DownCount").text = Format(mod.Stats.NegativeRatings);
            item.Q<Label>("DownloadCount").text = Format(mod.Stats.TotalDownloads);
        }

        private async void AddImages(Mod mod, VisualElement root)
        {
            var images = mod.Media.Images;
            if (images.Count > 0)
            {
                foreach (var image in images)
                {
                    if (image == null || image.Original == null)
                        continue;

                    await AddImage(image.Original, root);
                }
            }
            else if (mod.Logo != null)
            {
                if (mod.Logo == null || mod.Logo.Original == null)
                    return;
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
        }

        private static string Format(uint number)
        {
            //return NumberFormatter.Format((int) number);
            return number.ToString();
        }

        private async Task SetDependencies(VisualElement item, Mod mod)
        {
            var dependencies = await ModIo.Client.Games[ModIoGameInfo.GameId].Mods[mod.Id].Dependencies.Get();

            List<string> dependencyNames = new();
            foreach (var dependency in dependencies)
            {
                dependencyNames.Add(ModIoModRegistry.Get(dependency.ModId).Name);
            }

            item.Q<Label>("Dependencies").text = dependencyNames.Any() ? string.Join(Environment.NewLine, dependencyNames) : "-";
        }
    }
}