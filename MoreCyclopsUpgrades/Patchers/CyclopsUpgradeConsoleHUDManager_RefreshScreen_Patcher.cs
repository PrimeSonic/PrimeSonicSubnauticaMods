namespace MoreCyclopsUpgrades.Patchers
{
    using Managers;
    using Harmony;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            CyclopsHUDManager hudMgr = CyclopsManager.GeHUDManager(__instance.subRoot);

            if (hudMgr == null)
            {
                return;
            }

            hudMgr.UpdateConsoleHUD(__instance);
        }
    }
}
