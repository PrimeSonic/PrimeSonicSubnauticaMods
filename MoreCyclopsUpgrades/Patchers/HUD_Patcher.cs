namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Managers;
    using UnityEngine;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch(nameof(CyclopsHelmHUDManager.Update))]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(ref CyclopsHelmHUDManager __instance, ref int __state)
        {
            __state = -1;
            if (__instance.subLiveMixin.IsAlive() && __instance.subRoot != null)
            {
                // Should prevent the powerText from getting updated normally
                __state = __instance.lastPowerPctUsedForString;
                float ratioNum = __instance.subRoot.powerRelay.GetPower() / __instance.subRoot.powerRelay.GetMaxPower();
                __instance.lastPowerPctUsedForString = Mathf.CeilToInt(ratioNum * 100f);
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref CyclopsHelmHUDManager __instance, ref int __state)
        {
            CyclopsManager.GetManager(__instance.subRoot)?.HUD.FastUpdate(__instance, __state);
        }
    }

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch(nameof(CyclopsHolographicHUD.RefreshUpgradeConsoleIcons))]
    internal class CyclopsHolographicHUD_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(CyclopsHolographicHUD __instance)
        {
            return false; // Should now be handled by SetCyclopsUpgrades
        }
    }

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch(nameof(CyclopsUpgradeConsoleHUDManager.RefreshScreen))]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            PdaOverlayManager.UpdateIconOverlays();

            CyclopsHUDManager hudMgr = CyclopsManager.GetManager(__instance.subRoot)?.HUD;

            if (hudMgr == null)
                return true;

            hudMgr.SlowUpdate(__instance);

            return false;
        }
    }
}
