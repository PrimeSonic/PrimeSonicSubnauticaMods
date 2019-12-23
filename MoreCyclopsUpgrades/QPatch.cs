namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Config;
    using QModManager.API.ModLoading;

    /// <summary>
    /// Entry point class for patching. For use by QModManager only.
    /// </summary>
    [QModCore]
    public static class QPatch
    {
        /// <summary>
        /// For use by QModManager only.
        /// </summary>
        [QModPrePatch("5294E9BE8C4062684ACAAE0B514E91E2")]
        public static void PrePatch()
        {
            ModConfig.LoadOnDemand();

            RemoveOldConfigs();
        }

        /// <summary>
        /// Main patching method. For use by QModManager only.
        /// </summary>
        [QModPatch]
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

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
