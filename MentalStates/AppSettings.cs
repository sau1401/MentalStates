using System;
using System.Globalization;

namespace MentalStates
{
    public static class AppSettings
    {
        public static string[] PsychologicalStates { get; private set; }
        public static int SphereSize { get; private set; }
        public static string RippleColorHex { get; private set; }
        public static int RippleSize { get; private set; }

        public static string ShockwaveColorHex { get; private set; }
        public static float GlobalSpeedMultiplier { get; private set; }
        public static int BackgroundIntensity { get; private set; }
        public static string SphereColorSetting { get; private set; }

        public static void LoadSettings()
        {
            PsychologicalStates = SettingsManager.GetSetting("PsychologicalStates", "Calm, Focus").Split(',');
            SphereSize = int.Parse(SettingsManager.GetSetting("SphereSize", "10"));
            RippleColorHex = SettingsManager.GetSetting("RippleColor", "#FFFFFF");
            RippleSize = int.Parse(SettingsManager.GetSetting("RippleSize", "5"));
            ShockwaveColorHex = SettingsManager.GetSetting("ShockwaveColor", "#FF0000");
            GlobalSpeedMultiplier = float.Parse(SettingsManager.GetSetting("SpeedMultiplier", "1.0"), CultureInfo.InvariantCulture);
            BackgroundIntensity = int.Parse(SettingsManager.GetSetting("BackgroundIntensity", "0"));
            SphereColorSetting = SettingsManager.GetSetting("SphereColor", "Random");
        }
    }
}
