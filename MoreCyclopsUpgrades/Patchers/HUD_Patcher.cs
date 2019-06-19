namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Managers;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch("Update")]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsHelmHUDManager __instance)
        {
            CyclopsHUDManager hudMgr = CyclopsManager.GetManager<CyclopsHUDManager>(__instance.subRoot, CyclopsHUDManager.ManagerName);

            if (hudMgr == null)
                return;

            hudMgr.UpdateHelmHUD(__instance);
        }
    }

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch("RefreshUpgradeConsoleIcons")]
    internal class CyclopsHolographicHUD_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(CyclopsHolographicHUD __instance)
        {
            return false; // Should now be handled by SetCyclopsUpgrades
        }
    }

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
