using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using Newtonsoft.Json;

namespace ModManager.ModSystem
{
    public class Manifest
    {
        public Manifest(string path, string authorName, uint fileId, uint modId, string modName, string summary, string version, string changelogs, List<string> tags, bool removeModOnStartup)
        {
            Path = path;
            AuthorName = authorName;
            FileId = fileId;
            ModId = modId;
            ModName = modName;
            Summary = summary;
            Version = version;
            Changelogs = changelogs;
            Tags = tags;
            RemoveModOnStartup = removeModOnStartup;
        }

        [JsonIgnore] public string Path { get; private set; }

        public string AuthorName { get; private set; }

        public uint FileId { get; private set; }

        public uint ModId { get; private set; }

        public string ModName { get; private set; }

        public string Summary { get; private set; }

        public string Version { get; private set; }

        public string Changelogs { get; private set; }

        public List<string> Tags { get; private set; }

        public bool RemoveModOnStartup { get; private set; }

        public static Manifest Create(Mod mod)
        {
            string installationPath = System.IO.Path.Combine(Paths.ModInstallationFolder, mod.NameId!);

            return new Manifest(
                installationPath,
                mod.SubmittedBy!.Username!,
                mod.Modfile!.Id,
                mod.Id,
                mod.Name!,
                mod.Summary!,
                mod.Modfile.Version!,
                mod.Modfile.Changelog!,
                mod.Tags.Select(tag => tag.Name!).ToList(),
                false
            );
        }

        public void Update(Mod mod)
        {
            AuthorName = mod.SubmittedBy!.Username!;
            FileId = mod.Modfile!.Id;
            ModId = mod.Id;
            ModName = mod.Name!;
            Summary = mod.Summary!;
            Version = mod.Modfile.Version!;
            Changelogs = mod.Modfile.Changelog!;
            Tags = mod.Tags.Select(tag => tag.Name!).ToList();
        }

        public void ShouldRemoveOnNextStartup()
        {
            RemoveModOnStartup = true;
        }
    }
}