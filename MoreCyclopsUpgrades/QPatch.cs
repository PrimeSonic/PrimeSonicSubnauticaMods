namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.AuxConsole;

    /// <summary>
    /// Entry point class for patching. For use by QModManager only.
    /// </summary>
    public class QPatch
    {
        /// <summary>
        /// Main patching method. For use by QModManager only.
        /// </summary>
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                QuickLogger.DebugLogsEnabled = ModConfig.Main.DebugLogsEnabled;
                QuickLogger.Info($"Debug logging is {(QuickLogger.DebugLogsEnabled ? "en" : "dis")}abled");

                RemoveOldConfigs();

                PatchAuxUpgradeConsole();

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");

            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void RemoveOldConfigs()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string oldConfig1 = Path.Combine(executingLocation, $"CyclopsNuclearChargerConfig.txt");
            string oldConfig2 = Path.Combine(executingLocation, $"MoreCyclopsUpgradesConfig.txt");

            if (File.Exists(oldConfig1))
            {
                QuickLogger.Info("Deleted old config file 'CyclopsNuclearChargerConfig.txt'");
                File.Delete(oldConfig1);
            }

            if (File.Exists(oldConfig2))
            {
                QuickLogger.Info("Deleted old config file 'MoreCyclopsUpgradesConfig.txt'");
                File.Delete(oldConfig2);
            }
        }

        private static void PatchAuxUpgradeConsole()
        {
            if (ModConfig.Main.AuxConsoleEnabled)
                QuickLogger.Debug("Patching Auxiliary Upgrade Console");
            else
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

            var console = new AuxCyUpgradeConsole();
            console.Patch(ModConfig.Main.AuxConsoleEnabled);
        }
    }
}
