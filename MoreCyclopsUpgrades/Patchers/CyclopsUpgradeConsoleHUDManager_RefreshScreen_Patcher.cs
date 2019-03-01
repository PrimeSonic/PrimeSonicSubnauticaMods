namespace MoreCyclopsUpgrades.Patchers
{
    using Caching;
    using Harmony;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            CyclopsManager.SyncronizeAll(__instance);
        }
    }
}
