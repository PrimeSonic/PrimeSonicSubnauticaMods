namespace MoreCyclopsUpgrades
{
    using Harmony;

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

            UpgradeConsole upgradeConsole = __instance.subRoot.upgradeConsole;

            if (upgradeConsole == null)
                return; // safety check

            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            PowerCharging.UpdateHelmHUD(ref __instance, upgradeConsole.modules, auxUpgradeConsoles, ref lastReservePower);
        }

        
    }
}
