using OpenlabsInstaller.Github;
using OpenlabsInstaller.Json;
using OpenlabsInstaller.Utilities;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller
{
    internal static class BepInEx
    {
        public static void InstallToGame(LocalOpenlabsBuild build, Action onFinished)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Openlabs HTTP Client (v1 like WebClient)");
            var releases = GithubRest.GetReleases("BepInEx", "BepInEx");
            var downloadUrl = releases.FirstOrDefault(x => x.Name.Contains("5.4.21"), null)!.Assets
                                      .FirstOrDefault(x => x.Name == "BepInEx_x64_5.4.21.0.zip")!.BrowserDownloadUrl;

            var filePath = Environment.CurrentDirectory + "/bepinex.zip";
            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using (var pbar = new ProgressBar(100, "Downloading BepInEx", new ProgressBarOptions
            {
                ProgressCharacter = '█',
                ProgressBarOnBottom = true,
                ForegroundColor = ConsoleColor.Blue
            }))
            {
                client.DownloadAsync(downloadUrl, file, new Progress<float>((progress) =>
                {
                    pbar.Tick();
                    Thread.Sleep(Random.Shared.Next(40, 250));
                })).Wait();
            }

            ZipArchive archive = new ZipArchive(file);
            archive.ExtractToDirectory(build.BuildPath, true);
            file.Close(); // dispose zip archive file stream

            archive.Dispose();
            File.Delete(filePath);
            onFinished();
        }
    }
}
