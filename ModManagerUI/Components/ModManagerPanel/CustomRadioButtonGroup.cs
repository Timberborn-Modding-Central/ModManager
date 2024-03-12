using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modio.Models;
using ModManagerUI.EventSystem;
using ModManagerUI.UiSystem;
using UnityEngine.UIElements;

namespace ModManagerUI.Components.ModManagerPanel
{
    public abstract class CustomRadioButtonGroup
    {
        private readonly VisualElement _root;
        private readonly TagOption _tagOption;
        
        private int _tagsLastValue = -1;

        public RadioButtonGroup RadioButtonGroup { get; private set; }
        
        public List<string> TagOptions => _tagOption.Tags;
        
        public CustomRadioButtonGroup(VisualElement root, TagOption tagOption)
        {
            _root = root;
            _tagOption = tagOption;
        }
        
        public void Initialize(bool formatTags = false)
        {
            RadioButtonGroupRegistry.RadioButtonGroups.Add(this);
            
            var header = new Label();
            header.name = $"{_tagOption.Name}Header";
            header.text = _tagOption.Name;
            header.AddToClassList("text--default");
            header.AddToClassList("mods-box__tags-label");
            _root.Add(header);

            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.name = $"{_tagOption.Name}TagRadioButtonGroup";
            radioButtonGroup.value = -1;
            radioButtonGroup.AddToClassList("mods-box__tags");
            
            radioButtonGroup.choices = formatTags ? _tagOption.Tags.Select(FormatTag) : _tagOption.Tags;
            radioButtonGroup.RegisterValueChangedCallback(_ => OnValueChanged());

            foreach (var radioButton in radioButtonGroup.Children())
            {
                radioButton.RegisterCallback((ClickEvent @event) => ClickTagRadioButton(@event));
            }

            RadioButtonGroup = radioButtonGroup;
            
            _root.Add(radioButtonGroup);
        }

        public bool HasTagSelected()
        {
            return RadioButtonGroup.value != -1;
        }

        public string GetActiveTag()
        {
            return TagOptions[RadioButtonGroup.value];
        }

        private static string FormatTag(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');

                newText.Append(text[i]);
            }

            return newText.ToString();
        }
        
        protected virtual void OnValueChanged()
        {
            EventBus.Instance.PostEvent(new ModManagerPanelRefreshEvent());
        }

        protected virtual void ClickTagRadioButton(ClickEvent clickEvent)
        {
            if (clickEvent.currentTarget.GetType() != typeof(RadioButton))
                return;

            var target = (RadioButton)clickEvent.currentTarget;
            // Hacky way to get parent TagRadioButtonGroup
            var parent = target.parent;
            var secondParent = parent.parent;
            var thirdParent = (RadioButtonGroup)secondParent.parent;

            if (_tagsLastValue == thirdParent.value)
            {
                thirdParent.value = -1;
                _tagsLastValue = -1;
                return;
            }
            _tagsLastValue = thirdParent.value;
        }
    }
}