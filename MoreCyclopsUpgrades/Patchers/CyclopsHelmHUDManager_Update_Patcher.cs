namespace MoreCyclopsUpgrades
{
    using Harmony;
    using UnityEngine;

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

            int currentReservePower = PowerCharging.GetTotalReservePower(upgradeConsole.modules);

            if (currentReservePower > 0f)
            {
                __instance.powerText.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                __instance.powerText.color = Color.white; // Normal color
            }

            if (lastReservePower != currentReservePower)
            {
                float availablePower = currentReservePower + __instance.subRoot.powerRelay.GetPower();

                float availablePowerRatio = availablePower / __instance.subRoot.powerRelay.GetMaxPower();

                // Min'd with 999 since this textbox can only display 4 characeters
                int percentage = Mathf.Min(999, Mathf.CeilToInt(availablePowerRatio * 100f));

                __instance.powerText.text = $"{percentage}%";

                lastReservePower = currentReservePower;
            }
        }
    }
}
