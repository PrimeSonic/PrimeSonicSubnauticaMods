namespace MoreCyclopsUpgrades
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Config;
    using QModManager.API.ModLoading;

    /// <summary>
    /// Entry point class for patching.For use by QModManager only.
    /// </summary>
    [QModCore]
    public static class QPatch
    {
        /// <summary>
        /// Setting up the mod config. For use by QModManager only.
        /// </summary>
        [QModPrePatch]
        public static void PrePatch()
        {
            ModConfig.LoadOnDemand();
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

                var console = new AuxCyUpgradeConsole();
                console.Patch(ModConfig.Main.AuxConsoleEnabled);

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Critical error in patching");
                throw; // Rethrow for QModManager to catch and report
            }
        }
    }
}
