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

            // This method was put here because it's hit much less often than UpdateThermalReactorCharge
            UpgradeConsoleCache.SyncUpgradeConsoles(__instance.subRoot, __instance.subRoot.GetAllComponentsInChildren<AuxUpgradeConsole>());

            PowerManager.UpdateConsoleHUD(__instance, modules, UpgradeConsoleCache.AuxUpgradeConsoles);
        }
    }
}
