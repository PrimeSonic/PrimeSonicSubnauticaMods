namespace MidGameBatteries.Patchers
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(EnergyMixin))]
    [HarmonyPatch("Initialize")]
    internal class EnergyMixin_Patcher
    {
        public static void Postfix(ref EnergyMixin __instance)
        {
            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - Skip

            if (__instance.compatibleBatteries.Contains(TechType.Battery) &&
                !__instance.compatibleBatteries.Contains(DeepLithiumBattery.BatteryID))
            {
                // Make the Deep Lithium Battery compatible with this item
                __instance.compatibleBatteries.Add(DeepLithiumBattery.BatteryID);
                return;
            }

            if (__instance.compatibleBatteries.Contains(TechType.PowerCell) &&
                !__instance.compatibleBatteries.Contains(DeepLithiumPowerCell.PowerCellID))
            {
                // Make the Deep Lithium Power Cell compatible with this item
                __instance.compatibleBatteries.Add(DeepLithiumPowerCell.PowerCellID);

                // Find the normal PowerCell model
                int indexOfPowerCell = 0;
                do
                {
                    if (__instance.batteryModels[indexOfPowerCell].techType == TechType.PowerCell)
                        break;

                    indexOfPowerCell++;
                }
                while (indexOfPowerCell < __instance.batteryModels.Length);

                if (indexOfPowerCell >= __instance.batteryModels.Length)
                    return; // PowerCell not found

                // Make a copy of the PowerCell model for use as the Deep Lithium Power Cell model
                GameObject powerCellModel = __instance.batteryModels[indexOfPowerCell].model;

                __instance.batteryModels.Add(new EnergyMixin.BatteryModels
                {
                    techType = DeepLithiumPowerCell.PowerCellID,
                    model = powerCellModel,
                });

                return;
            }
        }
    }
}
