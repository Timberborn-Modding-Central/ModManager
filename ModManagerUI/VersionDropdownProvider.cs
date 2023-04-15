using Modio.Models;
using System.Collections.Generic;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.ModsSystemUI;

namespace ModManagerUI
{
    public class VersionDropdownProvider : IDropdownProvider
    {

        public VersionDropdownProvider(ModFullInfoController infoController,
                                   List<File> versions)
        {
            _infoController = infoController;
            _versions = versions;
        }

        private ModFullInfoController _infoController;
        private List<File> _versions;


        public IReadOnlyList<string> Items => _versions.Select(x => x.Version ?? "").ToList();

        public string GetValue()
        {
            return _infoController.CurrentFile.Version ?? "";
        }

        public void SetValue(string value)
        {
            File currFile = _versions.Where(x => (x.Version ?? "") == value).SingleOrDefault();
            _infoController.CurrentFile = currFile;
        }
    }
}
