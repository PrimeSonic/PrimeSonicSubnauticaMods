namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using Craftables;
    using Harmony;

    [HarmonyPatch(typeof(BatteryCharger))] // Patches the BatteryCharger class
    [HarmonyPatch(nameof(BatteryCharger.Initialize))]
    internal class BatteryCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref BatteryCharger __instance)
        {
            HashSet<TechType> compatibleTech = BatteryCharger.compatibleTech;

            // Make sure the Deep Battery is allowed in the battery charger
            if (!compatibleTech.Contains(DeepBatteryCellBase.BatteryID))
                compatibleTech.Add(DeepBatteryCellBase.BatteryID);

        }
    }

    [HarmonyPatch(typeof(PowerCellCharger))] // Patches the PowerCellCharger class
    [HarmonyPatch(nameof(PowerCellCharger.Initialize))]
    internal class PowerCellCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref PowerCellCharger __instance)
        {
            HashSet<TechType> compatibleTech = PowerCellCharger.compatibleTech;

            // Make sure the Deep Power Cell is allowed in the power cell charger
            if (!compatibleTech.Contains(DeepBatteryCellBase.PowerCellID))
                compatibleTech.Add(DeepBatteryCellBase.PowerCellID);
        }
    }
}
