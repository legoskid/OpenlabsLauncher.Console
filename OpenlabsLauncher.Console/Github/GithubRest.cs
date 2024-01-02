using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Github
{
    internal static class GithubRest
    {
        public static Release[] GetReleases(string user, string repository)
        {
            string url = string.Format("https://api.github.com/repos/{0}/{1}/releases", user, repository);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Openlabs HTTP Client (v1 like WebClient)");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

            var responseMessage = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
            if (!responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("INFO: " + responseMessage.StatusCode + ", " + responseMessage.Content.ReadAsStringAsync().Result);
                return Array.Empty<Release>();
            }

            return JsonConvert.DeserializeObject<Release[]>(responseMessage.Content.ReadAsStringAsync().Result)!;
        }

        public static Release GetLatestRelease(string user, string repository)
        {
            string url = string.Format("https://api.github.com/repos/{0}/{1}/releases/latest", user, repository);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Openlabs HTTP Client (v1 like WebClient)");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

            var responseMessage = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
            if (!responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("INFO: " + responseMessage.StatusCode + ", " + responseMessage.Content.ReadAsStringAsync().Result);
                return null;
            }

            return JsonConvert.DeserializeObject<Release>(responseMessage.Content.ReadAsStringAsync().Result)!;
        }
    }
}
