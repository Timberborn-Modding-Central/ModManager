using Timberborn.CoreUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModManagerUI.CrashGuardSystem
{
    public class StaticVisualElementLoader
    {
        public static VisualElementInitializer VisualElementInitializer = null!;

        public StaticVisualElementLoader(VisualElementInitializer visualElementInitializer)
        {
            VisualElementInitializer = visualElementInitializer;
        }
        
        public static VisualElement LoadVisualElement(string elementName)
        {
            return LoadVisualElement(LoadVisualTreeAsset(elementName));
        }

        private static VisualTreeAsset LoadVisualTreeAsset(string elementName)
        {
            return Resources.Load<VisualTreeAsset>(VisualElementLoader.ViewsDirectory + "/" + elementName);
        }

        private static VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
        {
            var visualElement = visualTreeAsset.CloneTree().ElementAt(0);
            VisualElementInitializer.InitializeVisualElement(visualElement);
            return visualElement;
        }
    }
}