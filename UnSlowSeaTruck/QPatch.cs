namespace UnSlowSeaTruck
{
    using System;
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            Console.WriteLine("[UnSlowSeaTruck] Patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            var cfgMgr = new ConfigMgr();
            cfgMgr.LoadConfig();

            var harmony = HarmonyInstance.Create("com.unslowseatruck.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(SeaTruckSegment))]
    [HarmonyPatch(nameof(SeaTruckSegment.GetAttachedWeight))]
    internal static class SeaTruckSegment_GetAttachedWeight_Override
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref float __result)
        {
            __result = ConfigMgr.Config.WeightOverride; // Override attached module weight

            return false;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch(nameof(SeaTruckMotor.Start))]
    internal static class SeatruckMotor_Start_Change
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaTruckMotor __instance)
        {
            SeaTruckConfig config = ConfigMgr.Config;

            __instance.steeringMultiplier *= config.SteeringMultiplier; // better steering
            __instance.acceleration *= config.AccelerationMultiplier; // better acceleration
        }
    }
}
