using System.Collections.Generic;
using System.Linq;
using Modio.Filters;
using ModManager;
using ModManagerUI.EventSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class SortingButtonsManager : Singleton<SortingButtonsManager>
    {
        private static readonly string InitiallyActiveName = "MostDownloaded";
        private static readonly List<SortingButton> SortingButtons = new();

        public static void AddNew(Button button, string name, Filter filterGetter)
        {
            var sortingButton = new SortingButton(button, name, filterGetter);
            sortingButton.Initialize(name == InitiallyActiveName);
            SortingButtons.Add(sortingButton);
        }

        public static void SetActive(string buttonName)
        {
            foreach (var sortingButton in SortingButtons)
            {
                sortingButton.SetState(sortingButton.Name == buttonName);
            }

            EventBus.Instance.PostEvent(new ModManagerPanelRefreshEvent());
        }

        public static Filter GetFilter()
        {
            return SortingButtons.First(button => button.Active).Filter;
        }
    }
}