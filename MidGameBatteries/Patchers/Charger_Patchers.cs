namespace MidGameBatteries.Patchers
{
    using Common;
    using Harmony;
    using System.Collections.Generic;
    using System.Reflection;

    [HarmonyPatch(typeof(BatteryCharger))]
    [HarmonyPatch("Start")]
    internal class BatteryCharger_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(ref BatteryCharger __instance)
        {
            FieldInfo compatibleTechFieldInfo = typeof(BatteryCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(null);
            
            if (!compatibleTech.Contains(DeepLithiumBattery.BatteryID))
            {
                compatibleTech.Add(DeepLithiumBattery.BatteryID);
                QuickLogger.Debug("DeepLithiumBattery added to BatteryCharger");
            }
        }
    }

    [HarmonyPatch(typeof(PowerCellCharger))]
    [HarmonyPatch("Start")]
    internal class PowerCellCharger_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(ref PowerCellCharger __instance)
        {
            FieldInfo compatibleTechFieldInfo = typeof(PowerCellCharger).GetField("compatibleTech", BindingFlags.Static | BindingFlags.NonPublic);

            var compatibleTech = (HashSet<TechType>)compatibleTechFieldInfo.GetValue(null);

            if (!compatibleTech.Contains(DeepLithiumPowerCell.PowerCellID))
            {
                compatibleTech.Add(DeepLithiumPowerCell.PowerCellID);
                QuickLogger.Debug("DeepLithiumPowerCell added to PowerCellCharger");
            }
        }
    }
}
