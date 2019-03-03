namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Caching;
    using System.Collections.Generic;
    using System.Reflection.Emit;

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

            PowerManager powerMgr = CyclopsManager.GetPowerManager(__instance.subRoot);

            if (powerMgr == null)
            {
                return true;
            }

            powerMgr.UpdateHelmHUD(__instance);

            return false;
        }
    }
}
