using System;
using System.Linq;
using Modio.Models;

namespace ModManagerUI.UiSystem
{
    public static class RadioButtonTagOption
    {
        public static TagOption Create(Type enumType)
        {
            return new TagOption
            {
                Name = enumType.Name,
                Type = enumType.Name.ToLower(),
                Tags = Enum.GetNames(enumType).ToList()
            };
        }
    }
}