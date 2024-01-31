using Modio.Filters;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class SortingButton
    {
        private readonly Button _root;

        public string Name { get; }        
        public Filter Filter { get; }

        public bool Active { get; private set; }

        public SortingButton(Button root, string name, Filter filter)
        {
            _root = root;
            Name = name;
            Filter = filter;
        }

        public void Initialize(bool initialState)
        {
            _root.RegisterCallback<ClickEvent>(_ => SortingButtonsManager.SetActive(Name));
            Active = initialState;
            Update();
        }

        public void SetState(bool state)
        {
            Active = state;
            Update();
        }

        private void Update()
        {
            _root.SetEnabled(!Active);
            // Turns text black, is currently set to false instead of _active as white might look better
            _root.EnableInClassList("mod-box__sort-button--selected", false);
        }
    }
}