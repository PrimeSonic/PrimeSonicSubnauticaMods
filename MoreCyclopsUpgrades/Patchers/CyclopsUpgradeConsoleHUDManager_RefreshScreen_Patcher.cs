namespace MoreCyclopsUpgrades
{
    using Harmony;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            UpgradeConsole upgradeConsole = __instance.subRoot.upgradeConsole;

            if (upgradeConsole == null)
                return; // safety check

            Equipment modules = upgradeConsole.modules;

            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            PowerCharging.UpdateConsoleHUD(__instance, modules, auxUpgradeConsoles);
        }
    }
}
