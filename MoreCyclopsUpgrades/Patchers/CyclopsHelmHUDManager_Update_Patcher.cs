namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Managers;
    using MoreCyclopsUpgrades.API;

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
}
