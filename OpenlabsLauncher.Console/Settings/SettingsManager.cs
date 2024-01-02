using Newtonsoft.Json;
using OpenlabsInstaller.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.Settings
{
    internal static class SettingsManager
    {
        public static string AppDataPath
            => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string AppDataDirectory
            => Path.Combine(AppDataPath, "Openlabs Installer Console");

        public static string LocalBuildsPath
            => Path.Combine(AppDataDirectory, "localBuilds.json");

        public static void Initialize()
        {
            Directory.CreateDirectory(AppDataDirectory);

            if (!File.Exists(LocalBuildsPath))
                File.WriteAllText(LocalBuildsPath, JsonConvert.SerializeObject(LocalBuilds));
            else ReloadSettings();
        }

        public static void ReloadSettings()
        {
            LocalBuilds = JsonConvert.DeserializeObject<List<LocalOpenlabsBuild>>(
                File.ReadAllText(LocalBuildsPath));
        }

        public static void SaveSettings()
        {
            File.WriteAllText(LocalBuildsPath, JsonConvert.SerializeObject(LocalBuilds));
        }

        public static List<LocalOpenlabsBuild> LocalBuilds
        {
            get;
            private set;
        } = new List<LocalOpenlabsBuild>();
    }
}
