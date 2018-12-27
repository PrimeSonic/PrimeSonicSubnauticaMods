namespace MidGameBatteries.Patchers
{
    using Harmony;
    using System.Collections.Generic;
    using System.Reflection;

    [HarmonyPatch(typeof(BatteryCharger))]
    [HarmonyPatch("Initialize")]
    internal class BatteryCharger_Patcher
    {
        public static void Postfix(ref BatteryCharger __instance)
        {
            FieldInfo compatibleTechFieldInfo = typeof(PowerCellCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(__instance);

            if (!compatibleTech.Contains(DeepLithiumPowerCell.PowerCellID))
                compatibleTech.Add(DeepLithiumPowerCell.PowerCellID);
        }
    }

    [HarmonyPatch(typeof(PowerCellCharger))]
    [HarmonyPatch("Initialize")]
    internal class PowerCellCharger_Patcher
    {
        public static void Postfix(ref PowerCellCharger __instance)
        {
            FieldInfo compatibleTechFieldInfo = typeof(PowerCellCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(__instance);

            if (!compatibleTech.Contains(DeepLithiumPowerCell.PowerCellID))
                compatibleTech.Add(DeepLithiumPowerCell.PowerCellID);
        }
    }
}
