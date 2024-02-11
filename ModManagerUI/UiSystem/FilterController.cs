using System.Collections.Generic;
using System.Linq;
using Modio.Filters;
using ModManager.VersionSystem;
using ModManagerUI.Components.ModManagerPanel;
using UnityEngine.UIElements;
using TextField = UnityEngine.UIElements.TextField;

namespace ModManagerUI.UiSystem
{
    public abstract class FilterController
    {
        public static VersionStatus? VersionStatusFilter = null;
        
        public static Filter Create(TextField? search, VisualElement tagsWrapper)
        {
            var excludeNames = new List<uint>();
            
            var filter = SortingButtonsManager.GetFilter();

            if (search != null && !string.IsNullOrEmpty(search.value))
            {
                filter = filter.And(ModFilter.FullText.Eq(search.value));
            }
            
            filter = filter.And(CreateTagsFilter(tagsWrapper));

            var namesFilter = CreateIdsFilter(excludeNames);
            if (namesFilter != null) 
                filter = filter.And(namesFilter);

            filter = filter.And(ModFilter.Id.NotIn(excludeNames));

            filter = filter.And(Filter.WithLimit(ModManagerPanel.ModsPerPage));

            return filter;
        }

        private static Filter CreateTagsFilter(VisualElement tagsWrapper)
        {
            var tags = new List<string>();
            
            foreach (var tagRadioButtonGroup in RadioButtonGroupRegistry.All<TagRadioButtonGroup>())
            {
                if (!tagRadioButtonGroup.HasTagSelected()) 
                    continue;
                tags.Add(tagRadioButtonGroup.GetActiveTag());
            }
            
            var checkedToggles = tagsWrapper.Children().Where(x => x.GetType() == typeof(Toggle) && ((Toggle)x).value).ToList();

            foreach (Toggle toggle in checkedToggles)
            {
                tags.Add(toggle.text);
            }

            return ModFilter.Tags.In(tags);
        }
        
        private static Filter? CreateIdsFilter(List<uint> excludeNames)
        {
            var hasNameLists = new List<List<uint>>();
            foreach (var filterNamesProvider in RadioButtonGroupRegistry.All<IFilterIdsProvider>().Where(provider => provider.HasTagSelected()))
            {
                var filterNames = filterNamesProvider.ProvideFilterIds(out var isNotList);
                if (isNotList) 
                    excludeNames.AddRange(filterNames);
                else
                    hasNameLists.Add(filterNames);
            }
            
            if (!hasNameLists.Any())
                return null;

            var list = FindCommonValues(hasNameLists).ToArray();
            if (list.Any())
            {
                return ModFilter.Id.In(list);
            }

            // HACK: Dummy filter to ensure results is 0
            return ModFilter.Id.Eq(1);
        }
        
        private static List<uint> FindCommonValues(List<List<uint>> lists)
        {
            var result = new HashSet<uint>(lists[0]);

            for (var i = 1; i < lists.Count; i++)
            {
                result.IntersectWith(lists[i]);
            }

            return result.ToList();
        }
    }
}