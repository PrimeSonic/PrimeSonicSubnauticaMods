namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Craftables;
    using Harmony;

    [HarmonyPatch(typeof(BatteryCharger))] // Patches the BatteryCharger class
    [HarmonyPatch(nameof(BatteryCharger.Start))]
    internal class BatteryCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref BatteryCharger __instance)
        {
            // The HashSet of compatible batteries is a private static field
            FieldInfo compatibleTechFieldInfo = typeof(BatteryCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(null);

            // Make sure the Deep Battery is allowed in the battery charger
            if (!compatibleTech.Contains(DeepBatteryCellBase.BatteryID))
                compatibleTech.Add(DeepBatteryCellBase.BatteryID);

        }
    }

    [HarmonyPatch(typeof(PowerCellCharger))] // Patches the PowerCellCharger class
    [HarmonyPatch(nameof(PowerCellCharger.Start))]
    internal class PowerCellCharger_Patcher
    {
        [HarmonyPrefix] // This will run just before the code in the chosen method
        public static void Prefix(ref PowerCellCharger __instance)
        {
            // The HashSet of compatible power cells is a private static field
            FieldInfo compatibleTechFieldInfo = typeof(PowerCellCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(null);

            // Make sure the Deep Power Cell is allowed in the power cell charger
            if (!compatibleTech.Contains(DeepBatteryCellBase.PowerCellID))
                compatibleTech.Add(DeepBatteryCellBase.PowerCellID);
        }
    }
}
