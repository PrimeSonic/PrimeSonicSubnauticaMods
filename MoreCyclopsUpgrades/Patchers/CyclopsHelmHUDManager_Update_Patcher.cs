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
            CyclopsHUDManager hudMgr = CyclopsManager.GetHUDManager(__instance.subRoot);

            if (hudMgr == null)
                return;

            hudMgr.UpdateHelmHUD(__instance);
        }
    }
}
