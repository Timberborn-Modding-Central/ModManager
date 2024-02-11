using Modio.Models;
using ModManager.VersionSystem;
using ModManagerUI.Components.ModCard;
using ModManagerUI.EventSystem;
using Timberborn.CoreUI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace ModManagerUI.UiSystem
{
    public class ModCard
    {
        private readonly InstalledToggle _installedToggle;
        private readonly EnabledToggle _enabledToggle;
        private readonly DownloadButton _downloadButton;
        private readonly UninstallButton _uninstallButton;
        private readonly VersionStatusIcon _versionStatusIcon;

        public VisualElement Root { get; }
        public Mod Mod { get; }

        private ModCard(
            VisualElement root, 
            InstalledToggle installedToggle, 
            EnabledToggle enabledToggle, 
            DownloadButton downloadButton, 
            UninstallButton uninstallButton, 
            VersionStatusIcon versionStatusIcon,
            Mod mod)
        {
            Root = root;
            _installedToggle = installedToggle;
            _enabledToggle = enabledToggle;
            _downloadButton = downloadButton;
            _uninstallButton = uninstallButton;
            _versionStatusIcon = versionStatusIcon;
            Mod = mod;
        }

        public static ModCard Create(Mod mod)
        {
            var assetName = "assets/resources/ui/views/mods/modsboxitem.uxml";
            var asset = AssetBundleLoader.AssetBundle.LoadAsset<VisualTreeAsset>(assetName);
            var root = ModManagerPanel.VisualElementLoader.LoadVisualElement(asset);
            
            root.Q<Label>("Name").text = mod.Name;
            root.Q<Button>("ModsBoxItem").clicked += () => OpenModPage(mod);
            // item.Q<Button>("ModsBoxItem").RegisterCallback<ClickEvent>(_ =>  OpenModPage(mod));
            
            var installedToggle = new InstalledToggle(root.Q<Toggle>("Installed"));
            installedToggle.Initialize(mod);
            
            var enabledToggle = new EnabledToggle(root.Q<Toggle>("Enabled"), mod);
            enabledToggle.TryInitializing();
            enabledToggle.Refresh();
            
            var downloadButton = new DownloadButton(root.Q<Button>("Download"), mod);
            downloadButton.Initialize();

            var uninstallButton = new UninstallButton(root.Q<Button>("Uninstall"), mod);
            uninstallButton.Initialize();
            
            var statusIcon = new VersionStatusIcon(root.Q<VisualElement>("VersionCompatibility"), mod);
            statusIcon.Initialize();
            
            var thumbnail = new Thumbnail(root.Q<Image>("Logo"), mod);
            thumbnail.Initialize();
            
            SetNumbers(mod, root);

            var modCard = new ModCard(root, installedToggle, enabledToggle, downloadButton, uninstallButton, statusIcon, mod);
            ModCardRegistry.ModCards.Add(modCard);
            EventBus.Instance.Register(modCard);
            modCard.Refresh();
            return modCard;
        }

        public void ModActionStarted()
        {
            ModManagerPanel.ModsWereChanged = true;
            _downloadButton.Disable();
        }

        public void ModActionStopped()
        {
            _downloadButton.Enable();
            Refresh();
        }
        
        public void Refresh()
        {
            Root.ToggleDisplayStyle(FilterController.VersionStatusFilter == null || FilterController.VersionStatusFilter == VersionStatusService.GetVersionStatus(Mod.Modfile));
            
            _installedToggle.Refresh();
            _enabledToggle.Refresh();
            _downloadButton.Refresh();
            _uninstallButton.Refresh();
            _versionStatusIcon.Refresh();
        }
        
        private static void OpenModPage(Mod mod)
        {
            ModManagerPanel.PanelStack.PushOverlay(ModManagerPanel.ModFullInfoController);
            ModManagerPanel.ModFullInfoController.SetMod(mod);
        }
        
        private static void SetNumbers(Mod mod, VisualElement item)
        {
            if (mod.Stats == null) 
                return;
            item.Q<Label>("UpCount").text = Format(mod.Stats.PositiveRatings);
            item.Q<Label>("DownCount").text = Format(mod.Stats.NegativeRatings);
            var flag = mod.Stats.TotalDownloads < 100000;
            item.Q<Label>("DownloadCount").text = Format(flag ? mod.Stats.TotalDownloads : mod.Stats.TotalDownloads / 1000.0) + (flag ? "" : "k");
        }

        private static string Format(double number)
        {
            return $"{number:0.#}";
        }
    }
}