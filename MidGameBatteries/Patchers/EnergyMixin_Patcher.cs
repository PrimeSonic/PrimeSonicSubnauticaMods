namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using Craftables;
    using Harmony;

    [HarmonyPatch(typeof(EnergyMixin))] // Patches the EnergyMixin class
    [HarmonyPatch(nameof(EnergyMixin.Start))]
    internal class EnergyMixin_Initialize_Patcher
    {
        [HarmonyPostfix] // This will run right after the code of the chosen method
        public static void Postfix(ref EnergyMixin __instance)
        {
            // This is necessary to allow the new batteries to be compatible with tools and vehicles

            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - No need to make changes

            List<TechType> compatibleBatteries = __instance.compatibleBatteries;

            if (compatibleBatteries.Contains(TechType.Battery) &&
                !compatibleBatteries.Contains(DeepBatteryCellBase.BatteryID))
            {
                // If the regular Battery is compatible with this item,
                // the Deep Lithium Battery should also be compatible
                compatibleBatteries.Add(DeepBatteryCellBase.BatteryID);
                return;
            }

            if (compatibleBatteries.Contains(TechType.PowerCell) &&
                !compatibleBatteries.Contains(DeepBatteryCellBase.PowerCellID))
            {
                // If the regular Power Cell is compatible with this item,
                // the Deep Lithium Power Cell should also be compatible
                compatibleBatteries.Add(DeepBatteryCellBase.PowerCellID);
                return;
            }
        }
    }

    [HarmonyPatch(typeof(EnergyMixin))] // Patches the EnergyMixin class
    [HarmonyPatch(nameof(EnergyMixin.NotifyHasBattery))] // Patches the NotifyHasBattery method
    internal class EnergyMixin_NotifyHasBattery_Patcher
    {
        [HarmonyPostfix] // This will run right after the code of the chosen method
        public static void Postfix(ref EnergyMixin __instance, InventoryItem item)
        {
            // For vehicles that show a battery model when one is equipped,
            // this will replicate the model for the normal Power Cell so it doesn't look empty

            // Null checks added on every step of the way
            if (item?.item?.GetTechType() == DeepBatteryCellBase.PowerCellID)
                __instance.batteryModels[0].model.SetActive(true);

            // Perhaps later a more suiteable model could be added with a more appropriate skin.
            // This is functional for now.
        }
    }
}
