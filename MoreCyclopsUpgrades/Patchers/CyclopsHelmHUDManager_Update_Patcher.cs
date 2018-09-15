namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Caching;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch("Update")]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        private static int lastReservePower = -1;

        public static void Postfix(ref CyclopsHelmHUDManager __instance)
        {
            if (!__instance.LOD.IsFull() || // can't see
                !__instance.subLiveMixin.IsAlive()) // dead
            {
                return;
            }

            PowerManager powerMgr = CyclopsManager.GetPowerManager(__instance.subRoot);

            if (powerMgr == null)
            {
                return;
            }

            powerMgr.UpdateHelmHUD(__instance, ref lastReservePower);
        }
    }
}
