using System.Collections.Generic;
using Modio.Models;
using ModManager.ModIoSystem;
using ModManagerUI.EventSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;
using EventBus = ModManagerUI.EventSystem.EventBus;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class TagsWrapper
    {
        public VisualElement Root { get; }

        public TagsWrapper(VisualElement root)
        {
            Root = root;
        }

        public void Initialize()
        {
            EventBus.Instance.Register(this);
        }

        [OnEvent]
        public void OnModManagerPanelOpenedEvent(ModManagerPanelRefreshEvent modManagerPanelRefreshEvent)
        {
            ShowTags();
        }

        private async void ShowTags()
        {
            var getTagsTask = ModIo.GameTagsClient.Get();
            OnTagsRetrieved(await getTagsTask);
            EventBus.Instance.Unregister(this);
        }
        
        private void OnTagsRetrieved(IEnumerable<TagOption> tags)
        {
            foreach (var tag in tags)
            {
                switch (tag.Type)
                {
                    case "dropdown":
                        var tagRadioButtonGroup = new TagRadioButtonGroup(Root, tag);
                        tagRadioButtonGroup.Initialize();
                        break;
                    case "checkboxes":
                        CreateCheckboxGroup(tag);
                        break;
                }
            }
        }
        
        private void CreateCheckboxGroup(TagOption tagOption)
        {
            var header = new Label();
            header.name = $"{tagOption.Name}Header";
            header.text = tagOption.Name;
            header.AddToClassList("text--default");
            header.AddToClassList("mods-box__tags-label");
            Root.Add(header);

            foreach (var tag in tagOption.Tags)
            {
                var toggle = new Toggle();
                toggle.name = $"{tagOption.Name}.{tag}";
                toggle.text = tag;
                toggle.AddToClassList("text--default");
                toggle.AddToClassList("mods-box-toggle");
                toggle.RegisterValueChangedCallback(_ => EventBus.Instance.PostEvent(new ModManagerPanelRefreshEvent()));
                Root.Add(toggle);
            }
        }
    }
}