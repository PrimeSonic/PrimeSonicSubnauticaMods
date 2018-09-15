namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Caching;
    using Common;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            UpgradeManager upgradeMgr = CyclopsManager.GetManager(__instance.subRoot)?.UpgradeManager;

            if (upgradeMgr == null)
            {
                QuickLogger.Debug("RefreshScreen: UpgradeManager not found!", true);
                return;
            }

            PowerManager powerMgr = CyclopsManager.GetManager(__instance.subRoot)?.PowerManager;

            if (powerMgr == null)
            {
                QuickLogger.Debug("RefreshScreen: PowerManager not found!", true);
                return;
            }

            upgradeMgr.SyncUpgradeConsoles();

            powerMgr.UpdateConsoleHUD(__instance);
        }
    }
}
