using System.Collections.Generic;
using System.Linq;
using Modio.Models;
using Newtonsoft.Json;

namespace ModManager.ModSystem
{
    public class Manifest
    {
        public static readonly string FileName = "manifest.json";

        public Manifest()
        {
        }

        public Manifest(Mod mod, File file, string installationRootPath)
        {
            RootPath = installationRootPath;
            AuthorName = mod.SubmittedBy!.Username!;
            FileId = file.Id;
            ModId = mod.Id;
            ModName = mod.Name!;
            Summary = mod.Summary!;
            Version = file.Version!;
            Changelogs = file.Changelog!;
            Tags = mod.Tags.Select(tag => tag.Name!).ToList();
        }

        public Manifest Update(Mod mod, File file)
        {
            AuthorName = mod.SubmittedBy!.Username!;
            FileId = file.Id;
            ModId = mod.Id;
            ModName = mod.Name!;
            Summary = mod.Summary!;
            Version = file.Version!;
            Changelogs = file.Changelog!;
            Tags = mod.Tags.Select(tag => tag.Name!).ToList();

            return this;
        }

        [JsonIgnore]
        public string RootPath { get; set; } = null!;

        [JsonIgnore]
        public bool Enabled { get; set; } = true;

        public string AuthorName { get; set; } = null!;

        public uint FileId { get; set; }

        public uint ModId { get; set; }

        public string ModName { get; set; } = null!;

        [JsonProperty(Required = Required.AllowNull)]
        public string Summary { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Version { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string? Changelogs { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public List<string>? Tags { get; set; }
    }
}
