namespace MoreCyclopsUpgrades
{
    using Harmony;
    using MoreCyclopsUpgrades.Caching;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            if (__instance.subRoot.upgradeConsole == null)
                return; // safety check

            // This method was put here because it's hit much less often than UpdateThermalReactorCharge
            UpgradeConsoleCache.SyncUpgradeConsoles(__instance.subRoot, __instance.subRoot.GetAllComponentsInChildren<AuxUpgradeConsole>());

            PowerManager.UpdateConsoleHUD(__instance);
        }
    }
}
