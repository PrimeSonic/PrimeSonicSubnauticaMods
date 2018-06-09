namespace MoreCyclopsUpgrades
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            UpgradeConsole upgradeConsole = __instance.subRoot.upgradeConsole;

            if (upgradeConsole == null)
                return; // safety check

            float currentReservePower = PowerCharging.GetTotalReservePower(upgradeConsole.modules);

            float currentBatteryPower = __instance.subRoot.powerRelay.GetPower();

            if (currentReservePower > 0f)
            {
                __instance.energyCur.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                __instance.energyCur.color = Color.white; // Normal color
            }

            int totalPower = Mathf.CeilToInt(currentBatteryPower + currentReservePower);

            __instance.energyCur.text = IntStringCache.GetStringForInt(totalPower);
        }
    }
}
