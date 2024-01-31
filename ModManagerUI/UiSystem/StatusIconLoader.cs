using ModManager;
using UnityEngine;

namespace ModManagerUI.UiSystem
{
    public class StatusIconLoader : Singleton<StatusIconLoader>
    {
        private Sprite? _unknownSprite;
        private Sprite? _compatibleSprite;
        private Sprite? _incompatibleSprite;

        public Sprite UnknownSprite => _unknownSprite ??= LoadSprite("assets/resources/ui/images/mods/status-unknown.png");
        public Sprite CompatibleSprite => _compatibleSprite ??= LoadSprite("assets/resources/ui/images/mods/status-compatible.png");
        public Sprite IncompatibleSprite => _incompatibleSprite ??= LoadSprite("assets/resources/ui/images/mods/status-incompatible.png");

        private static readonly float ImageSizeMultiplier = 0.6f;

        private static Sprite LoadSprite(string path)
        {
            var texture = AssetBundleLoader.AssetBundle.LoadAsset<Texture2D>(path);
            // Scaling doesnt work ¯\_(ツ)_/¯
            // texture.Reinitialize(Mathf.RoundToInt(texture.width * ImageSizeMultiplier), Mathf.RoundToInt(texture.height * ImageSizeMultiplier));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}