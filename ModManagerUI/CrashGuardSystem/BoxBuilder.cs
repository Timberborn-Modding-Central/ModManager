using System;
using Timberborn.Core;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModManagerUI.CrashGuardSystem
{
    public class BoxBuilder
    {
        private readonly ILoc _loc;
        private readonly VisualElement _root;
        private Action? _confirmAction = EmptyCallback;
        private Action? _cancelAction;
        private Action? _infoAction;
        private string? _confirmText;
        private string? _cancelText;
        private string? _infoText;

        public BoxBuilder(
            ILoc loc,
            VisualElement root)
        {
            _loc = loc;
            _root = root;
        }

        public BoxBuilder SetMessage(string text)
        {
            _root.Q<Label>("Message").text = text;
            return this;
        }

        public BoxBuilder SetLocalizedMessage(string locKey) => SetMessage(_loc.T(locKey));

        public BoxBuilder SetConfirmButton(Action confirmAction)
        {
            _confirmAction = confirmAction;
            return this;
        }

        public BoxBuilder SetConfirmButton(Action confirmAction, string confirmText)
        {
            _confirmText = confirmText;
            return SetConfirmButton(confirmAction);
        }

        public BoxBuilder SetCancelButton(Action cancelAction)
        {
            _cancelAction = cancelAction;
            return this;
        }

        public BoxBuilder SetCancelButton(Action cancelAction, string cancelText)
        {
            _cancelText = cancelText;
            return SetCancelButton(cancelAction);
        }

        public BoxBuilder SetDefaultCancelButton()
        {
            return SetCancelButton(EmptyCallback);
        }

        public BoxBuilder SetDefaultCancelButton(string cancelText)
        {
            return SetCancelButton(EmptyCallback, cancelText);
        }

        public BoxBuilder SetInfoButton(Action infoAction, string infoText)
        {
            _infoAction = infoAction;
            _infoText = infoText;
            return this;
        }

        public BoxBuilder SetOffset(Vector2Int offsetInPixels)
        {
            OffsetBox(_root, offsetInPixels);
            return this;
        }

        public BoxBuilder AddContent(VisualElement content)
        {
            _root.Q<VisualElement>("Content").Add(content);
            return this;
        }

        public VisualElement Create()
        {
            SetupButtons();
            StaticVisualElementLoader.VisualElementInitializer.InitializeVisualElement(_root);
            return _root;
        }

        private void SetupButtons()
        {
            var confirmButton = _root.Q<Button>("ConfirmButton");
            var cancelButton = _root.Q<Button>("CancelButton");
            var infoButton = _root.Q<Button>("InfoButton");
            SetupButtonsText(confirmButton, cancelButton, infoButton);
            SetupButtonsActions(confirmButton, cancelButton, infoButton);
        }

        private void SetupButtonsText(Button confirmButton, Button cancelButton, Button infoButton)
        {
            if (_confirmText == null)
                _confirmText = _loc.T(_cancelAction == null ? CoreLocKeys.OKKey : CoreLocKeys.YesKey);
            confirmButton.text = _confirmText;
            if (_cancelAction != null)
            {
                if (_cancelText == null)
                    _cancelText = _loc.T(CoreLocKeys.NoKey);
                cancelButton.text = _cancelText;
            }

            if (string.IsNullOrEmpty(_infoText))
                return;
            infoButton.text = _infoText;
        }

        private void SetupButtonsActions(
            Button confirmButton,
            Button cancelButton,
            Button infoButton)
        {
            if (_confirmAction != null)
                confirmButton.RegisterCallback((EventCallback<ClickEvent>)(_ => _confirmAction()));
            else
                confirmButton.ToggleDisplayStyle(false);
            if (_cancelAction != null)
                cancelButton.RegisterCallback((EventCallback<ClickEvent>)(_ => _cancelAction()));
            else
                cancelButton.ToggleDisplayStyle(false);
            if (_infoAction != null)
                infoButton.RegisterCallback((EventCallback<ClickEvent>)(_ => _infoAction()));
            else
                infoButton.ToggleDisplayStyle(false);
        }

        private static void EmptyCallback()
        {
        }

        private static void OffsetBox(VisualElement box, Vector2Int offsetInPixels)
        {
            var style = box.style;
            var x = offsetInPixels.x;
            if (x < 0)
                style.right = (float)Math.Abs(x);
            else if (x > 0)
                style.left = (float)x;
            var y = offsetInPixels.y;
            if (y < 0)
            {
                style.top = (float)Math.Abs(y);
            }
            else
            {
                if (y <= 0)
                    return;
                style.bottom = (float)y;
            }
        }
    }
}