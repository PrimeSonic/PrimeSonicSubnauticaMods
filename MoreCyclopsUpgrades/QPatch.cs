namespace MoreCyclopsUpgrades
{
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
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // If enabled, patch the Auxiliary Upgrade Console as a new buildable.
            if (ModConfig.Main.AuxConsoleEnabled)
            {
                var console = new AuxCyUpgradeConsole();
                console.Patch();
            }
            else
            {
                // SMLHelper now handles previously used but now disabled TechTypes
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");
            }

            var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }
    }
}
