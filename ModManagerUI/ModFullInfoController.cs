using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.ModIoSystem;
using ModManagerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Timberborn.ModsSystemUI
{
    public class ModFullInfoController : IPanelController
    {
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

        private AssetBundle _bundle;

        public ModFullInfoController(PanelStack panelStack,
                                     IAddonService addonService,
                                     VisualElementInitializer visualElementInitializer,
                                     InstalledAddonRepository installedAddonRepository)
        {
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

        public void SetMod(Mod mod)
        {
            string assetName = "assets/resources/ui/views/mods/ModsBoxFullItem.uxml";
            var asset = ModsBox._bundle.LoadAsset<VisualTreeAsset>(assetName);
            var item = LoadVisualElement(asset);

            item.Q<Button>("Close").clicked += OnUICancelled;
            item.Q<Label>("Name").text = mod.Name;
            item.Q<Label>("AuthorName").text = mod.SubmittedBy?.Username;
            item.Q<Label>("Tags").text = string.Join(", ", mod.Tags.Select(tag => tag.Name));
            item.Q<Label>("Description").text = string.IsNullOrEmpty(mod.DescriptionPlaintext)
                ? mod.Summary
                : mod.DescriptionPlaintext;
            item.Q<Label>("InstalledVersion").text = _installedAddonRepository.Has(mod.Id)
                ? _installedAddonRepository.Get(mod.Id).Version
                : "-";
            item.Q<Label>("LiveVersion").text = mod?.Modfile?.Version ?? "";

            var depTask = SetDependencies(item, mod);

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

    }
}