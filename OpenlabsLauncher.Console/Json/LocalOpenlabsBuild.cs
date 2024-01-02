using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Json
{
    internal class LocalOpenlabsBuild
    {
        public static LocalOpenlabsBuild Create(OpenlabsBuild build, string path)
        {
            return new LocalOpenlabsBuild
            {
                BuildId = build.BuildId,
                Hash = build.Hash,
                Version = build.Version,
                FriendlyVersion = build.FriendlyVersion,
                GameName = build.GameName,
                PlatformProvider = build.PlatformProvider,
                Manifest = build.Manifest,
                BuildPath = path,
                Installed = false
            };
        }

        [JsonProperty("buildId")]
        public int BuildId { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("friendlyVersion")]
        public string FriendlyVersion { get; set; }

        [JsonProperty("gameName")]
        public string GameName { get; set; }

        [JsonProperty("platformProvider")]
        public string PlatformProvider { get; set; }

        [JsonProperty("manifest")]
        public string Manifest { get; set; }

        [JsonProperty("buildPath")]
        public string BuildPath { get; set; }

        [JsonProperty("installed")]
        public bool Installed { get; set; }
    }
}
