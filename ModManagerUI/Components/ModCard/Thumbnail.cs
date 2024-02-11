using System.Net.Http;
using Modio.Models;
using ModManager.AddonSystem;
using UnityEngine;
using Image = UnityEngine.UIElements.Image;

namespace ModManagerUI.Components.ModCard
{
    public class Thumbnail
    {
        private readonly Image _root;
        private readonly Mod _mod;

        public Thumbnail(Image root, Mod mod)
        {
            _root = root;
            _mod = mod;
        }
        
        public void Initialize()
        {
            LoadImage();
        }
        
        private async void LoadImage()
        {
            if (_mod.Logo == null || _mod.Logo.Thumb320x180 == null) 
                return;
            try
            {
                var bytes = await AddonService.Instance.GetImage(_mod.Logo.Thumb320x180);
                var texture = new Texture2D(0, 0);
                texture.LoadImage(bytes);
                _root.image = texture;
            }
            catch (HttpRequestException exception)
            {
                ModManagerUIPlugin.Log.LogWarning($"Error occured while fetching image: {exception.Message}");
            }
        }
    }
}