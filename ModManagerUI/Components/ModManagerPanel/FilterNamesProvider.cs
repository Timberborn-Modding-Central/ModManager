using System.Collections.Generic;

namespace ModManagerUI.Components.ModManagerPanel
{
    public interface IFilterIdsProvider
    {
        List<uint> ProvideFilterIds(out bool isNotList);

        bool HasTagSelected();
    }
}