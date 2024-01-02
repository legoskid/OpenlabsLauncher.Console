using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Json
{
    internal class PingResponse
    {
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("healthy")]
        public bool Healthy { get; set; }

        [JsonProperty("healthStatus")]
        public string HealthStatus { get; set; }

        [JsonProperty("connectedSince")]
        public long ConnectedSince { get; set; }

        [JsonProperty("pingMs")]
        public int PingMs { get; set; }
    }
}
