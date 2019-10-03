namespace UnSlowSeaTruck
{
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.unslowseatruck.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("GetWeight")]
    internal static class SeatruckMotor_GetWeight_Override
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref float __result)
        {
            __result = 1f; // Total weight is always 1 (the same as the cabin does alone)

            return false;
        }
    }

    [HarmonyPatch(typeof(SeaTruckSegment))]
    [HarmonyPatch("GetAttachedWeight")]
    [HarmonyPatch("GetTotalWeight")]
    internal static class SeaTruckSegment_GetAttachedWeight_Override
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref float __result)
        {            
            __result = 0f; // Attached modules weigh nothing

            return false;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor))]
    [HarmonyPatch("Start")]
    internal static class SeatruckMotor_Start_Change
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaTruckMotor __instance)
        {
            __instance.steeringMultiplier *= 1.15f; // 15% better steering
            __instance.acceleration *= 1.25f; // 25% better acceleration
            __instance.underWaterDrag /= 0.25f; // Reduce underwater drag by 25%
        }
    }
}
