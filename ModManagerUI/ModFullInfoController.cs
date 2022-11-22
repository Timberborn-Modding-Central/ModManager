using Modio.Models;
using ModManager.AddonSystem;
using System;
using System.Linq;
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

        private AssetBundle _bundle;

        public ModFullInfoController(PanelStack panelStack,
                                     IAddonService addonService,
                                     VisualElementInitializer visualElementInitializer)
        {
            _panelStack = panelStack;
            _addonService = addonService;
            _visualElementInitializer = visualElementInitializer;

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
                var byteArray = await _addonService.GetImage(mod.Logo.Thumb320x180);
                var texture = new Texture2D(1, 1);
                texture.LoadImage(byteArray);
                root.image = texture;
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

        private static string Format(uint number)
        {
            //return NumberFormatter.Format((int) number);
            return number.ToString();
        }

    }
}