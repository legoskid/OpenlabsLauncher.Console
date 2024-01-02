using OpenlabsInstaller;
using OpenlabsInstaller.Json;
using OpenlabsInstaller.Settings;
using OpenlabsLauncher.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsLauncher.ConsoleUI
{
    internal class InstallsConsoleView : BaseCompactConsoleView
    {
        public override string Name => "hub.installs";

        public override async Task OnLoad()
        {
            await InitializeOptionMenu();
        }

        private async Task InitializeOptionMenu()
        {
            _builds = await CloudApi.GetBuilds();
            _optionView = new ConsoleOptionView("Available Builds",
                                                (from x in _builds
                                                 select new ConsoleOption
                                                 {
                                                     name = string.Format("'{0}' Openlabs Release ({1})", x.GameName, x.FriendlyVersion),
                                                     action = () => OnBuildOptionClicked(x)
                                                 }).ToArray());

            _optionView.Update();
        }

        private bool IsBuildInstalled(OpenlabsBuild x)
        {
            SettingsManager.ReloadSettings();
            var localBuild = SettingsManager.LocalBuilds.FirstOrDefault(
                x => x.Manifest == x.Manifest, null);

            if (localBuild == null) return false;

            var unityDataPath = Path.Combine(localBuild.BuildPath, "Recroom_Release_Data");
            var unityAppPath = Path.Combine(localBuild.BuildPath, "Recroom_Release.exe");

            return File.Exists(unityAppPath) &&
                    Directory.Exists(unityDataPath);
        }

        private bool IsBuildInSettings(OpenlabsBuild x)
        {
            SettingsManager.ReloadSettings();
            var localBuild = SettingsManager.LocalBuilds.FirstOrDefault(
                x => x.Manifest == x.Manifest, null);

            if (localBuild == null) return false;
            return true;
        }

        private void OnBuildOptionClicked(OpenlabsBuild build)
        {
            if (!IsBuildInstalled(build))
            {
                _optionView.Teardown();
                _optionView = null;

                DepotDownloader.Initialize();
                Console.Clear();

                if (!IsBuildInSettings(build))
                {
                    Console.WriteLine("Select your folder (if you cancel the build will download in the installer directory)");

                    string initialFolder = Path.Combine(Environment.CurrentDirectory, "Installs");
                    Directory.CreateDirectory(initialFolder);

                    initialFolder = Path.Combine(initialFolder, build.Manifest);
                    Directory.CreateDirectory(initialFolder);

                    SettingsManager.LocalBuilds.Add(
                        LocalOpenlabsBuild.Create(build, initialFolder));
                    SettingsManager.SaveSettings();
                }

                LocalOpenlabsBuild localBuild = SettingsManager.LocalBuilds
                    .FirstOrDefault(x => x.Manifest == build.Manifest);

                DepotDownloader.OnApplicationClose += () =>
                { OnDepotDownloaderApplicationClose(localBuild); };
                DepotDownloader.StartDepotDownloader("471710", "471711", localBuild.Manifest, localBuild.BuildPath);
            }
            else
            {

            }
        }

        public static void OnDepotDownloaderApplicationClose(LocalOpenlabsBuild localBuild)
        {
            BepInEx.InstallToGame(localBuild, async () =>
            {
                Console.Clear();
                Console.WriteLine("INFO: Successfully installed BepInEx");
                await Task.Delay(500);

                Directory.CreateDirectory(Path.Combine(localBuild.BuildPath, "BepInEx", "plugins"));
                Console.WriteLine("INFO: Created Plugins");

                BepInExMod.InstallToGame(localBuild, async () =>
                {
                    Console.Clear();
                    Console.WriteLine("INFO: Successfully installed patcher");
                    await Task.Delay(1000);

                    File.WriteAllLines(Path.Combine(localBuild.BuildPath, "zinc.cfg"), new string[] 
                    {
                        "Branch=Zinc",
                        "ServerHost=zinc.openlabs.live"
                    });
                    Console.WriteLine("INFO: Created config");

                    File.WriteAllText(Path.Combine(localBuild.BuildPath, "steam_appid.txt"), "471710");
                    Console.WriteLine("INFO: Created steam_appid");

                    if (!VerifyBuild(localBuild))
                        Console.WriteLine("ERROR: The build failed to install correctly. Please retry installation and try again.");
                    else
                        Console.WriteLine("INFO: The build installed successfully!");
                });
            });
        }

        private static bool VerifyBuild(LocalOpenlabsBuild localBuild)
        {
            var plugins = Path.Combine(localBuild.BuildPath, "BepInEx", "Plugins");
            var config = Path.Combine(localBuild.BuildPath, "zinc.cfg");
            var steamAppId = Path.Combine(localBuild.BuildPath, "steam_appid.txt");

            if (!Directory.Exists(plugins))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: The plugins folder does not exist!\n\tPath: " + plugins);
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
        }

        public override async Task OnUpdate(float delta)
        {
            if (_optionView != null)
                _optionView.Update();
        }

        public override async Task OnClose()
        {
            _optionView.Teardown();
            _optionView = null;
        }

        private ConsoleOptionView _optionView;
        private List<OpenlabsBuild> _builds;
    }
}
