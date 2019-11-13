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
            if (!compatibleTech.Contains(CbCore.SampleBattery))
            {
                foreach (TechType moddedBattery in CbCore.BatteryTechTypes)
                    compatibleTech.Add(moddedBattery);
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
            if (!compatibleTech.Contains(CbCore.SamplePowerCell))
            {
                foreach (TechType moddedPowerCell in CbCore.PowerCellTechTypes)
                    compatibleTech.Add(moddedPowerCell);
            }
        }
    }
}
