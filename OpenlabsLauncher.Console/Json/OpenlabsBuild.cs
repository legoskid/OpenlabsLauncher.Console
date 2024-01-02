using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Json
{
    internal class OpenlabsBuild
    {
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

        [JsonProperty("downloadProvider")]
        public string DownloadProvider { get; set; }

        [JsonProperty("downloadProviderUrl")]
        public string DownloadProviderUrl { get; set; }

        [JsonProperty("downloadProviderVersion")]
        public string DownloadProviderVersion { get; set; }

        [JsonProperty("manifest")]
        public string Manifest { get; set; }
    }
}
