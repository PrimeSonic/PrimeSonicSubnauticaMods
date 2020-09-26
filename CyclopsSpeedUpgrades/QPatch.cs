namespace CyclopsSpeedUpgrades
{
    using System.Reflection;
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var speedUpgrade = new CyclopsSpeedModule();
            speedUpgrade.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(speedUpgrade.CreateSpeedUpgradeHandler);
            MCUServices.Register.PdaIconOverlay(speedUpgrade.TechType, speedUpgrade.CreateSpeedIconOverlay);

            var harmony = new Harmony("com.cyclopsspeedupgrades.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info($"Finished patching.");
        }
    }

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch(nameof(CyclopsHelmHUDManager.PlayCavitationWarningAfterSeconds))]
    internal class NoiseAlertPatch
    {
        [HarmonyPrefix]
        internal static bool Prefix(CyclopsHelmHUDManager __instance)
        {
            // Ensure that the alert only plays when in Flank mode
            return __instance.motorMode.cyclopsMotorMode == CyclopsMotorMode.CyclopsMotorModes.Flank;
        }
    }
}
