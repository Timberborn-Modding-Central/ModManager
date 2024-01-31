using ModManagerUI.EventSystem;
using Timberborn.CoreUI;
using UnityEngine.UIElements;
using EventBus = ModManagerUI.EventSystem.EventBus;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class ShowMoreButton
    {
        private readonly Button _root;

        private ShowMoreButton(Button root)
        {
            _root = root;
        }

        public static ShowMoreButton Create(Button root)
        {
            var showMoreButton = new ShowMoreButton(root);
            EventBus.Instance.Register(showMoreButton);
            root.ToggleDisplayStyle(false);
            root.RegisterCallback<ClickEvent>(_ => EventBus.Instance.PostEvent(new ShowMoreModsEvent()));
            return showMoreButton;
        }

        // [OnEvent]
        // private void OnShowMoreMods(OnShowMoreMods onShowMoreMods)
        // {
        //     _root.SetEnabled(false);
        // }
        //
        // [OnEvent]
        // private void OnModsRetrieved(OnModsRetrieved onModsRetrieved)
        // {
        //     _root.ToggleDisplayStyle(onModsRetrieved.Mods.Count >= _modsPerPage);
        //     _root.SetEnabled(true);
        // }
    }
}