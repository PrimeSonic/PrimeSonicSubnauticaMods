namespace CyclopsSpeedUpgrades
{
    using System.Reflection;
    using BepInEx;
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Speed Upgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsspeedupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var speedUpgrade = new CyclopsSpeedModule();
            speedUpgrade.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(speedUpgrade.CreateSpeedUpgradeHandler);
            MCUServices.Register.PdaIconOverlay(speedUpgrade.TechType, speedUpgrade.CreateSpeedIconOverlay);

            var harmony = new Harmony(GUID);
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
