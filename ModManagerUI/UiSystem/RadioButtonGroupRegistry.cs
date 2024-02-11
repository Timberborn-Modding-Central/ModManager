using System.Collections.Generic;
using System.Linq;
using ModManagerUI.Components.ModManagerPanel;

namespace ModManagerUI.UiSystem
{
    public abstract class RadioButtonGroupRegistry
    {
        public static readonly List<CustomRadioButtonGroup> RadioButtonGroups = new();

        public static IEnumerable<T> All<T>()
        {
            return RadioButtonGroups.OfType<T>();
        }
        
        public static T Single<T>()
        {
            return RadioButtonGroups.OfType<T>().First();
        }
    }
}