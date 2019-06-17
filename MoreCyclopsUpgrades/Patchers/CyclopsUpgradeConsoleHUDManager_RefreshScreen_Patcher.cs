namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Managers;
    using MoreCyclopsUpgrades.API;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            CyclopsHUDManager hudMgr = CyclopsManager.GetManager<CyclopsHUDManager>(__instance.subRoot, CyclopsHUDManager.ManagerName);

            if (hudMgr == null)
                return true;

            hudMgr.UpdateConsoleHUD(__instance);

            return false;
        }
    }
}
