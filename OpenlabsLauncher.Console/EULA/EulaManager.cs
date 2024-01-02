using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsInstaller.EULA
{
    public enum EulaState
    {
        Unsigned,
        Signed
    }

    internal class EulaFile
    {
        static EulaFile()
        {
            if (!File.Exists(EulaFilePath))
            {
                File.WriteAllText(EulaFilePath, @"# This is the EULA file. (End User License Agreement)
#
# Below, is a value that indicates that you agree to all of our terms and conditions
# for using this application. If you do, replace 'Unsigned' to 'Signed' or '1'.

eula_state=Unsigned");
            }
        }

        static string EulaFilePath => Environment.CurrentDirectory + "/__eula.txt";

        private string[] GetAllEulaLines() => File.ReadAllLines(EulaFilePath);
        private string GetEulaStateLine()
        {
            string[] lines = (from x in GetAllEulaLines()
                              where !x.StartsWith("#")
                              select x).ToArray();

            foreach (var line in lines)
            {
                if (line.StartsWith("eula_state"))
                {
                    return line;
                }
            }
            throw new InvalidDataException("Invalid EULA file data");
        }

        public EulaState ReadState()
        {
            var stateLine = GetEulaStateLine();
            var value = stateLine.Split('=')[1];

            return Enum.Parse<EulaState>(value);
        }

        public void WriteState(EulaState state)
        {
            var eulaLines = (from x in GetAllEulaLines()
                             select (x.StartsWith("eula_state") ? "eula_state=" + state.ToString() : x));
            File.WriteAllLines(EulaFilePath, eulaLines);
        }
    }

    internal static class EulaManager
    {
        public static readonly EulaFile CurrentEulaFile = new EulaFile();

        public static void CheckEula()
        {
            if (CurrentEulaFile.ReadState() == EulaState.Unsigned)
            {
                Console.Clear();
                Console.WriteLine("To use this application, you must agree to our EULA.");
                Console.WriteLine("Please read the EULA that will appear.");

                Thread.Sleep(500);
                var process = Process.Start("notepad.exe", Environment.CurrentDirectory + "/__terms.txt");
                Thread.Sleep(85);
                PInvoke.User32.ShowWindow(process.MainWindowHandle, PInvoke.User32.WindowShowStyle.SW_MAXIMIZE);

                Console.WriteLine();
                Console.Write("Do you agree to the terms? (yes, no) > ");

                var str = Console.ReadLine().ToLower();
                EulaState choice = str.StartsWith("y") ? EulaState.Signed : str.StartsWith("n") ? EulaState.Unsigned : EulaState.Unsigned;
                CurrentEulaFile.WriteState(choice);

                if (choice == EulaState.Unsigned)
                {
                    Environment.Exit(0);
                    return;
                }

                Console.Clear();
            }
        }
    }
}
