using OpenlabsInstaller.Json;
using OpenlabsInstaller.Utilities;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller
{
    internal static class BepInExMod
    {
        public static void InstallToGame(LocalOpenlabsBuild build, Action onFinished)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Openlabs HTTP Client (v1 like WebClient)");
            var downloadUrl = $"http://downloads.openlabs.live/{build.Manifest}/patcher.dll";

            var filePath = Path.Combine(Environment.CurrentDirectory, "patcher.dll");
            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using (var pbar = new ProgressBar(100, "Downloading Rec Room Patcher", new ProgressBarOptions
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

            var pluginsFolder = Path.Combine(build.BuildPath, "BepInEx", "plugins");
            if (Directory.Exists(pluginsFolder))
                Directory.CreateDirectory(pluginsFolder);

            file.Close();
            File.Move(filePath, Path.Combine(pluginsFolder, "patcher.dll"));
            Console.WriteLine("WARNING: mod successfully added to BepInEx");

            // after finished
            File.Delete(filePath);
            onFinished();
        }
    }
}
