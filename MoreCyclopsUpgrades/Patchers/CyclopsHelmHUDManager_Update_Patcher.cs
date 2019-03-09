namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Caching;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch("Update")]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref CyclopsHelmHUDManager __instance)
        {
            if (!__instance.LOD.IsFull())
            {
                return true; // Same early exit
            }

            CyclopsHUDManager hudMgr = CyclopsManager.GeHUDManager(__instance.subRoot);

            if (hudMgr == null)
            {
                return true;
            }

            hudMgr.UpdateHelmHUD(__instance);

            return false;
        }
    }
}
