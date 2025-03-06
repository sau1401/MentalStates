using System;
using System.Windows.Forms;

namespace MentalStates
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //check if screensaver is launched with a command-line argument; checks for screensaver bootup, settings, or preview
            if (args.Length > 0)
            {
                string arg = args[0].ToLower().Trim();

                // /c for configuration/settings
                if (arg.StartsWith("/c"))
                {
                    Application.Run(new SettingsForm());
                    return;
                }
                // /p for preview mode (if implemented)
                if (arg.StartsWith("/p"))
                {
                    return;
                }
                // /s runs the screensaver normally
            }
            Application.Run(new ScreensaverForm());
        }
    }
}
