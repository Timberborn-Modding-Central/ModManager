using System;
using System.Collections.Generic;
using ModManager.AddonSystem;
using ModManager.VersionSystem;
using ModManagerUI.UiSystem;
using Timberborn.CoreUI;
using Timberborn.DistributionSystemBatchControl;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.StockpilePriorityUISystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModManagerUI.CrashGuardSystem
{
    public class CrashScreenBox
    {
        public static CrashScreenBox Instance = null!;

        private readonly IAddonService _addonService = AddonService.Instance;

        private static SliderToggleFactory _sliderToggleFactory;
        private static ILoc _loc;

        private VisualElement? _root;
        private static SliderToggle[]? _sliderToggles;
        private bool _isFirst = true;


        public CrashScreenBox(
            SliderToggleFactory sliderToggleFactory,
            ILoc loc)
        {
            _sliderToggleFactory = sliderToggleFactory;
            _loc = loc;

            Instance = this;
        }

        public VisualElement Create(List<uint> modIds)
        {
            var builder = CreateBuilder();
            builder.SetMessage(_loc.T("Mods.CrashInfo"));

            var buttonsContainer = new VisualElement
            {
                style =
                {
                    marginTop = new Length(20, LengthUnit.Pixel),
                    justifyContent = new StyleEnum<Justify>(Justify.Center),
                    alignItems = new StyleEnum<Align>(Align.Center),
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                }
            };
            var enableAll = new NineSliceButton
            {
                classList = { "menu-button" },
                text = _loc.T("Mods.EnableAll")
            };
            enableAll.RegisterCallback<ClickEvent>(_ => modIds.ForEach(id => _addonService.Enable(id)));
            buttonsContainer.Add(enableAll);
            var disableAll = new NineSliceButton
            {
                classList = { "menu-button" },
                text = _loc.T("Mods.DisableAll")
            };
            disableAll.RegisterCallback<ClickEvent>(_ => modIds.ForEach(id => _addonService.Disable(id)));
            buttonsContainer.Add(disableAll);
            builder.AddContent(buttonsContainer);

            var container = new ScrollView
            {
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    height = new Length(500, LengthUnit.Pixel),

                    justifyContent = new StyleEnum<Justify>(Justify.Center),
                    alignItems = new StyleEnum<Align>(Align.Center),

                    marginTop = new Length(20, LengthUnit.Pixel)
                }
            };
            container.AddToClassList("scroll--green-decorated");

            var sliderToggles = new List<SliderToggle>();
            foreach (var modId in modIds)
            {
                if (!InstalledAddonRepository.Instance.TryGet(modId, out var manifest))
                    continue;
                var modContainer = new VisualElement
                {
                    style =
                    {
                        paddingBottom = new Length(10, Length.Unit.Pixel),
                        paddingLeft = new Length(10, Length.Unit.Pixel),
                        paddingRight = new Length(10, Length.Unit.Pixel),
                        paddingTop = new Length(10, Length.Unit.Pixel),

                        justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween),
                        alignItems = new StyleEnum<Align>(Align.Center),
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
                };
                if (_isFirst)
                {
                    _isFirst = false;
                }
                else
                {
                    modContainer.style.borderTopColor = new StyleColor(new Color32(8, 20, 20, 255));
                    modContainer.style.borderTopWidth = 2;
                }

                var compatibleImage = new Image
                {
                    style = { marginRight = new Length(10, Length.Unit.Pixel) }
                };
                switch (VersionStatusService.GetVersionStatus(manifest.ModId, manifest.Version))
                {
                    case VersionStatus.Unknown:
                        compatibleImage.AddToClassList("mods-box-item__status-unknown");
                        break;
                    case VersionStatus.Compatible:
                        compatibleImage.AddToClassList("mods-box-item__status-compatible");
                        break;
                    case VersionStatus.Incompatible:
                        compatibleImage.AddToClassList("mods-box-item__status-incompatible");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                modContainer.Add(compatibleImage);

                var label = new Label($"{manifest.ModName} ({manifest.Version})");
                label.AddToClassList("text--default");
                label.AddToClassList("mods-box-item__name");
                modContainer.Add(label);
                var buttonContainer = new VisualElement();
                var sliderToggleItem1 = SliderToggleItem.Create(GetImportDisabledTooltip, StockpilePriorityToggle.AcceptClass,
                    ImportToggleFactory.ImportForcedBackgroundClass, () => _addonService.Enable(manifest.ModId), () => manifest.Enabled);
                var sliderToggleItem2 = SliderToggleItem.Create(GetImportAutoTooltip, ImportToggleFactory.ImportDisabledIconClass,
                    ImportToggleFactory.ImportDisabledBackgroundClass, () => _addonService.Disable(manifest.ModId), () => !manifest.Enabled);
                sliderToggles.Add(_sliderToggleFactory.Create(buttonContainer, sliderToggleItem1, sliderToggleItem2));
                modContainer.Add(buttonContainer);
                container.Add(modContainer);
            }

            _sliderToggles = sliderToggles.ToArray();
            builder.SetConfirmButton(GameRestarter.QuitOrRestart, _loc.T(SteamChecker.IsRestartCompatible() ? "Mods.Restart" : "Mods.Quit"));
            builder.SetCancelButton(GoToCrashReport, _loc.T("Mods.GoToCrashReport"));
            builder.AddContent(container);
            _root = builder.Create();
            return _root;
        }

        public static void UpdateSingleton()
        {
            if (_sliderToggles == null)
                return;

            foreach (var sliderToggle in _sliderToggles)
                sliderToggle.Update();
        }

        private void GoToCrashReport()
        {
            _root.ToggleDisplayStyle(false);
            CrashScreenPanelPatch.ShowCrashScreenPanel();
        }

        public BoxBuilder CreateBuilder()
        {
            var root = StaticVisualElementLoader.LoadVisualElement("Core/DialogBox");
            foreach (var styleSheet in Resources.LoadAll<StyleSheet>($"UI"))
                root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(AssetBundleLoader.AssetBundle.LoadAsset<StyleSheet>("assets/resources/ui/views/mods/ModsBoxStyle.uss"));
            root.style.position = new StyleEnum<Position>(Position.Absolute);
            root.style.height = new Length(100, Length.Unit.Percent);
            root.style.width = new Length(100, Length.Unit.Percent);
            // root.style.backgroundColor = new StyleColor(new Color32(144, 238, 144, 1));
            return new BoxBuilder(_loc, root);
        }

        private VisualElement GetImportDisabledTooltip()
        {
            return GetTooltip(ImportToggleFactory.ImportDisabledLocKey,
                ImportToggleFactory.ImportDisabledDescriptionLocKey, false);
        }

        private VisualElement GetImportAutoTooltip()
        {
            return GetTooltip(ImportToggleFactory.ImportAutoLocKey, ImportToggleFactory.ImportAutoDescriptionLocKey, true);
        }

        private VisualElement GetTooltip(string title, string description, bool withBalanceInfo)
        {
            var e = StaticVisualElementLoader.LoadVisualElement("Game/ImportToggleTooltip");
            e.Q<Label>("Title").text = _loc.T(title);
            e.Q<Label>("Description").text =
                withBalanceInfo ? _loc.T(description) + "\n" + _loc.T(ImportToggleFactory.BalanceInfoLocKey) : _loc.T(description);
            return e;
        }
    }
}