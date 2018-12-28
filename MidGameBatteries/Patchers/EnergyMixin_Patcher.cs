namespace MidGameBatteries.Patchers
{
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(EnergyMixin))]
    [HarmonyPatch("Start")]
    internal class EnergyMixin_Initialize_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref EnergyMixin __instance)
        {
            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - Skip

            if (__instance.compatibleBatteries.Contains(TechType.Battery) &&
                !__instance.compatibleBatteries.Contains(DeepLithiumBattery.BatteryID))
            {
                // Make the Deep Lithium Battery compatible with this item
                __instance.compatibleBatteries.Add(DeepLithiumBattery.BatteryID);
                //QuickLogger.Debug("DeepLithiumBattery now compatible with EnergyMixin");
                return;
            }

            if (__instance.compatibleBatteries.Contains(TechType.PowerCell) &&
                !__instance.compatibleBatteries.Contains(DeepLithiumPowerCell.PowerCellID))
            {
                // Make the Deep Lithium Power Cell compatible with this item
                __instance.compatibleBatteries.Add(DeepLithiumPowerCell.PowerCellID);
                //QuickLogger.Debug("DeepLithiumPowerCell now compatible with EnergyMixin");
                return;
            }
        }
    }

    [HarmonyPatch(typeof(EnergyMixin))]
    [HarmonyPatch("NotifyHasBattery")]
    internal class EnergyMixin_NotifyHasBattery_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref EnergyMixin __instance, InventoryItem item)
        {
            if (item?.item?.GetTechType() == DeepLithiumPowerCell.PowerCellID)
            {
                __instance.batteryModels[0].model.SetActive(true);
                //QuickLogger.Debug("Overiding model on vehicle for DeepLithiumPowerCell");
            }
        }
    }
}
