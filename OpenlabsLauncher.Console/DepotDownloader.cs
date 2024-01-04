using Newtonsoft.Json;
using OpenlabsInstaller.Encryption;
using OpenlabsInstaller.Github;
using OpenlabsInstaller.Utilities;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenlabsInstaller
{
    internal static class DepotDownloader
    {
        public static LoginInformation Credentials { get; set; } = null;
        public static Action OnApplicationClose { get; set; } = new Action(() => { });

        public static string App { get; set; } = "471710";
        public static string Depot { get; set; } = "471711";
        public static string Manifest { get; set; } = "0";

        public static string Latest => Path.Combine(Environment.CurrentDirectory, "Latest Install");

        public static StreamWriter ProcessInput;
        public static StreamReader ProcessOutput;

        public static string UserInfoPath => Environment.CurrentDirectory + "/do_not_share_this_file.userinfo";

        public static void Initialize()
        {
            Credentials = GetLoginInformation();
            if (Credentials == null) PromptLogin();
        }

        public static string ReadLatest() => File.ReadAllText(Environment.CurrentDirectory + "/latest.dp.txt");
        public static void WriteLatest(int id) => File.WriteAllText(Environment.CurrentDirectory + "/latest.dp.txt", id.ToString());
        public static bool IsLatest(Release release) => ReadLatest() == release.Id.ToString();

        public static void InstallLatest()
        {
            var depotDownloaderPath = Environment.CurrentDirectory + "/DepotDownloader";
            var latestRelease = GithubRest.GetLatestRelease("SteamRE", "DepotDownloader");
            if (latestRelease == null)
            {
                Console.WriteLine("ERROR: Failed to download SteamRE/DepotDownloader. The repo does not exist or the Github servers are down.");
                return;
            }

            if (latestRelease != null && (!IsLatest(latestRelease) || !Directory.Exists(depotDownloaderPath)))
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Openlabs HTTP Client (v1 like WebClient)");
                var downloadUrl = latestRelease.Assets.First().BrowserDownloadUrl;

                var filePath = Environment.CurrentDirectory + "/latest.dp.zip";
                var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using (var pbar = new ProgressBar(100, "Downloading SteamAPI Depot Downloader", new ProgressBarOptions
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
                WriteLatest(latestRelease.Id);

                ZipArchive archive = new ZipArchive(file);
                archive.ExtractToDirectory(depotDownloaderPath);
                file.Close(); // dispose zip archive file stream

                archive.Dispose();
                File.Delete(filePath);
            }
        }

        public static LoginInformation GetLoginInformation()
        {
            if (!File.Exists(UserInfoPath))
                return null;

            var encryptedJson = File.ReadAllText(UserInfoPath);
            var decryptedLogin = AES.Decrypt(encryptedJson, "usidu89");
            return JsonConvert.DeserializeObject<LoginInformation>(decryptedLogin)!;
        }

        public static void SaveLoginInformation(string username, string password)
        {
            var credentials = new LoginInformation
            {
                Username = username,
                Password = password,
                CreationTime = DateTime.Now
            };

            var loginJson = JsonConvert.SerializeObject(credentials);
            var loginEncryption = AES.Encrypt(loginJson, "usidu89");
            File.WriteAllText(UserInfoPath, loginEncryption);

            Credentials = credentials;
        }

        public static void SetDepotDownloaderWindowTitle()
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
                if (process.ProcessName.Contains("Depot") && process.ProcessName.Contains("Downloader"))
                    PInvoke.User32.SetWindowText(process.MainWindowHandle, "Depot Downloader [data hidden by Openlabs Installer]");
        }

        public static void StartDepotDownloader(string appId, string depotId, string manifestId, string directory)
        {
            Console.Clear();

            Manifest = manifestId;
            var loginInformation = GetLoginInformation();
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + "/DepotDownloader/DepotDownloader.exe",

                Arguments = string.Format("-app {0} -depot {1} -manifest {2} -username {3} -password {4} -dir {5}", 
                                appId, 
                                depotId, 
                                manifestId, 
                                loginInformation.Username, 
                                loginInformation.Password,
                                directory),

                WindowStyle = ProcessWindowStyle.Normal,
                CreateNoWindow = false,
                UseShellExecute = true
            });

            Thread.Sleep(250);
            SetDepotDownloaderWindowTitle();

            Console.WriteLine("INFO: Waiting for application close...");
            process!.WaitForExit();

            Console.WriteLine("WARNING: Application closed");

            // fix install path

            OnApplicationClose();
        }

        public static void PromptLogin()
        {
            if (GetLoginInformation() != null) return;

            var usernameText = "Account Name: ";
            var passwordText = "Password (masked): ";

            Console.Clear();
            Console.WriteLine("You are currently not logged in -- please login to your Steam account to access SteamAPI.");
            Console.WriteLine("Please note that your account information IS NOT saved on our cloud, or shared anywhere.");
            Console.WriteLine(usernameText);
            Console.WriteLine(passwordText);
            Console.WriteLine();
            Console.WriteLine("Once you press enter, your credentials will be stored in a user info file. ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("DON'T SHARE THE FILE TO ANYBODY even if they're helping you as it has YOUR STEAM PASSWORD!!!");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("This file is encrypted!");
            Console.ResetColor();

            Console.SetCursorPosition(usernameText.Length, 2);
            var usernameValue = Console.ReadLine()!;

            Console.SetCursorPosition(passwordText.Length, 3);
            var passwordValue = ConsoleUtilities.MaskedInput('*');

            SaveLoginInformation(usernameValue, passwordValue);
        }

        public static void PromptManifest()
        {
            Console.Clear();
            Console.Write("manifest: ");
            Manifest = ulong.Parse(Console.ReadLine()!).ToString();
        }
    }
}
