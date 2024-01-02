using Newtonsoft.Json;
using OpenlabsInstaller.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsLauncher.Network
{
    internal static class CloudApi
    {
        static HttpClient CurrentClient
        {
            get;
        } = CreateClient("1.2.0", "https://openlabshubapi-prod-eastus-001.azurewebsites.net");

        static HttpClient CreateClient(string clientVersion, string baseAddress)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Origin", "Openlabs Hub (WPF UI)");
            client.DefaultRequestHeaders.Add("X-Origin-Version", clientVersion);

            client.BaseAddress = new Uri(baseAddress);

            return client;
        }

        public static async Task<bool> GetHealth()
        {
            var responseMessage = await CurrentClient.GetAsync("/api/v1/ping");
            if (!responseMessage.IsSuccessStatusCode) return false;

            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<PingResponse>(jsonString);

            return deserializedObject.Healthy;
        }

        public static async Task<List<OpenlabsBuild>> GetBuilds()
        {
            var responseMessage = await CurrentClient.GetAsync("/api/v1/builds");
            if (!responseMessage.IsSuccessStatusCode) return new List<OpenlabsBuild>();

            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<OpenlabsBuild>>(jsonString)!;
        }
    }
}
