using Timberborn.CoreUI;
using Timberborn.ExperimentalModeSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.CrashGuardSystem
{
  public class CrashScreenBox : IPanelController
  {
    private static bool showedAlready;

    private readonly PanelStack _panelStack;
    private readonly VisualElementLoader _visualElementLoader;
    private readonly ExperimentalMode _experimentalMode;

    public CrashScreenBox(
      PanelStack panelStack,
      VisualElementLoader visualElementLoader,
      ExperimentalMode experimentalMode)
    {
      _panelStack = panelStack;
      _visualElementLoader = visualElementLoader;
      _experimentalMode = experimentalMode;
    }

    public VisualElement GetPanel()
    {
      var e = _visualElementLoader.LoadVisualElement("MainMenu/WelcomeScreenBox");
      // e.Q<VisualElement>("ExperimentalInfo", (string) null).ToggleDisplayStyle(this._experimentalMode.IsExperimental);
      e.Q<Button>("Start", (string) null).RegisterCallback<ClickEvent>((EventCallback<ClickEvent>) (_ => RemoveAndShowMainMenu()));
      return e;
    }

    public bool OnUIConfirmed()
    {
      RemoveAndShowMainMenu();
      return true;
    }

    public void OnUICancelled() => RemoveAndShowMainMenu();

    public void Show()
    {
      _panelStack.Push((IPanelController) this);
    }

    private void RemoveAndShowMainMenu()
    {
      showedAlready = true;
      _panelStack.Pop((IPanelController) this);
    }
  }
}
