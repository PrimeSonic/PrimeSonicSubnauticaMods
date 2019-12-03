namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using CustomBatteries.Items;
    using Harmony;

    [HarmonyPatch(typeof(BatteryCharger))] // Patches the BatteryCharger class
    [HarmonyPatch(nameof(BatteryCharger.Initialize))]
    internal class BatteryCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref BatteryCharger __instance)
        {
            if (!CbCore.HasBatteries)
                return;

            HashSet<TechType> compatibleTech = BatteryCharger.compatibleTech;

            // Make sure all custom batteries are allowed in the battery charger
            if (!compatibleTech.Contains(CbCore.ModdedBattery))
            {
                for (int i = 0; i < CbCore.BatteryTechTypes.Count; i++)
                    compatibleTech.Add(CbCore.BatteryTechTypes[i]);
            }
        }
    }

    [HarmonyPatch(typeof(PowerCellCharger))] // Patches the PowerCellCharger class
    [HarmonyPatch(nameof(PowerCellCharger.Initialize))]
    internal class PowerCellCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref PowerCellCharger __instance)
        {
            if (!CbCore.HasPowerCells)
                return;

            HashSet<TechType> compatibleTech = PowerCellCharger.compatibleTech;

            // Make sure all modded power cells are allowed in the power cell charger
            if (!compatibleTech.Contains(CbCore.ModdedPowerCell))
            {
                for (int i = 0; i < CbCore.PowerCellTechTypes.Count; i++)
                    compatibleTech.Add(CbCore.PowerCellTechTypes[i]);
            }
        }
    }
}
