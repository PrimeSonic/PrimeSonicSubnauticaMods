namespace MoreCyclopsUpgrades.Patchers
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
            // This method was put here because it's hit much less often than UpdateThermalReactorCharge
            UpgradeConsoleCache.SyncUpgradeConsoles(__instance.subRoot);

            PowerManager.UpdateConsoleHUD(__instance);
        }
    }
}
