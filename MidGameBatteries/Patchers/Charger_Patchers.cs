namespace MidGameBatteries.Patchers
{
    using Common;
    using Harmony;
    using Craftables;
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
            
            if (!compatibleTech.Contains(DeepLithiumBase.BatteryID))
            {
                compatibleTech.Add(DeepLithiumBase.BatteryID);
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

            if (!compatibleTech.Contains(DeepLithiumBase.PowerCellID))
            {
                compatibleTech.Add(DeepLithiumBase.PowerCellID);
                QuickLogger.Debug("DeepLithiumPowerCell added to PowerCellCharger");
            }
        }
    }
}
