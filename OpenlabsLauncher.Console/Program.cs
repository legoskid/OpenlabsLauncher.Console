using System.ComponentModel;
using System;
using OpenlabsInstaller.EULA;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using OpenlabsLauncher.ConsoleUI;
using OpenlabsLauncher.Network;
using OpenlabsInstaller.Settings;

namespace OpenlabsInstaller
{
    internal static class Program
    {
        static InstallsConsoleView _installsView
            = new InstallsConsoleView();

        #region Parameters

        public static T GetParameter<T>(string[] args, string paramName, T defaultValue = default!)
        {
            var index = IndexOfParameter(args, paramName);
            if (index == -1) return defaultValue;

            string value = args[index + 1];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null)
                return (T)converter.ConvertFromString(value)!;
            return defaultValue;
        }

        private static bool HasParameter(string[] args, string param)
            => Program.IndexOfParameter(args, param) > -1;

        private static int IndexOfParameter(string[] args, string param)
        {
            for (int i = 0; i < args.Length; i++)
                if (args[i].Equals(param, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        #endregion

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e)
                => File.WriteAllText("crash.log", ((Exception)e.ExceptionObject).ToString());

            SettingsManager.Initialize();
            EulaManager.CheckEula();

            Console.Title = "Openlabs Auto Installer";
            DepotDownloader.InstallLatest();

            if (!(await CloudApi.GetHealth()))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Server connected returned unexpected error");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            Console.Clear();
            Console.WriteLine("Welcome to the Openlabs Auto Installer!");
            Console.WriteLine("Press any key to continue...");

            const float DELTA = 0.0333333f;

            await _installsView.OnLoad();
            while (_installsView != null)
            {
                await _installsView.OnUpdate(DELTA);
                await Task.Delay((int)(DELTA * 500));
            }
            await _installsView.OnClose();
            return;
        }

        /*private static void PostInstall()
        {
            MelonLoader.InstallToGame(async () =>
            {
                Console.Clear();
                Console.WriteLine("INFO: Successfully installed MelonLoader");
                await Task.Delay(500);

                Directory.CreateDirectory(Path.Combine(DepotDownloader.Latest, "Mods"));
                Console.WriteLine("INFO: Created Mods");

                Directory.CreateDirectory(Path.Combine(DepotDownloader.Latest, "Plugins"));
                Console.WriteLine("INFO: Created Plugins");

                Directory.CreateDirectory(Path.Combine(DepotDownloader.Latest, "UserData"));
                Console.WriteLine("INFO: Created UserData");

                Directory.CreateDirectory(Path.Combine(DepotDownloader.Latest, "UserLibs"));
                Console.WriteLine("INFO: Created UserLibs");

                MelonLoaderMod.InstallToGame(async () =>
                {
                    Console.Clear();
                    Console.WriteLine("INFO: Successfully installed patcher");
                    await Task.Delay(1000);

                    File.WriteAllText(Path.Combine(DepotDownloader.Latest, "config.json"), JsonConvert.SerializeObject(new
                    {
                        Branch = "Cobalt",
                        GameVersion = "20200204",
                        NameServer = "http://openlabsprodeu001.zapto.org:7000/",
                        IsDeveloper = false
                    }));
                    Console.WriteLine("INFO: Created config");

                    File.WriteAllText(Path.Combine(DepotDownloader.Latest, "steam_appid.txt"), "471710");
                    Console.WriteLine("INFO: Created steam_appid");

                    if (!VerifyBuild())
                        Console.WriteLine("ERROR: The build failed to install correctly. Please retry installation and try again.");
                    else
                        Console.WriteLine("INFO: The build installed successfully!");
                });
            });
        }

        private static bool VerifyBuild()
        {
            var mods = Path.Combine(DepotDownloader.Latest, "Mods");
            var plugins = Path.Combine(DepotDownloader.Latest, "Plugins");
            var userData = Path.Combine(DepotDownloader.Latest, "UserData");
            var userLibs = Path.Combine(DepotDownloader.Latest, "UserLibs");
            var config = Path.Combine(DepotDownloader.Latest, "config.json");
            var steamAppId = Path.Combine(DepotDownloader.Latest, "steam_appid.txt");

            if (!Directory.Exists(mods))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The mods folder does not exist!\n\tPath: " + mods);
                return false;
            }
            if (!Directory.Exists(plugins))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The plugins folder does not exist!\n\tPath: " + plugins);
                return false;
            }
            if (!Directory.Exists(userData))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The user data folder does not exist!\n\tPath: " + userData);
                return false;
            }
            if (!Directory.Exists(userLibs))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The user libs folder does not exist!\n\tPath: " + userLibs);
                return false;
            }
            if (!File.Exists(config))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The config file does not exist!\n\tPath: " + config);
                return false;
            }
            if (!File.Exists(steamAppId))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The steam_appid file does not exist!\n\tPath: " + steamAppId);
                return false;
            }

            return true;
        }*/
    }
}