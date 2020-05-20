namespace UnSlowSeaTruck
{
    using System;
    using System.Reflection;
    using Harmony;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        internal static SeaTruckConfig Config;

        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("[UnSlowSeaTruck] Patching v" + assembly.GetName().Version.ToString(3));

            Config = ConfigMgr.LoadConfig<SeaTruckConfig>();
            Console.WriteLine($"[UnSlowSeaTruck] Configured WeightOverride: {Config.WeightOverride}");

            var harmony = HarmonyInstance.Create($"com.{assembly.GetName().Name}.psmod");
            harmony.PatchAll(assembly);
        }
    }

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.GetAttachedWeight))]
    internal static class SeaTruckSegment_GetAttachedWeight_Override
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref float __result)
        {
            __result = QPatch.Config.WeightOverride; // Override attached module weight

            return false;
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.Start))]
    internal static class SeatruckMotor_Start_Change
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaTruckMotor __instance)
        {
            SeaTruckConfig config = QPatch.Config;

            Console.WriteLine($"[UnSlowSeaTruck] Original steeringMultiplier: {__instance.steeringMultiplier}");
            Console.WriteLine($"[UnSlowSeaTruck] Original acceleration: {__instance.acceleration}");

            __instance.steeringMultiplier *= config.SteeringMultiplier; // better steering
            __instance.acceleration *= config.AccelerationMultiplier; // better acceleration

            Console.WriteLine($"[UnSlowSeaTruck] Modified steeringMultiplier: {__instance.steeringMultiplier} (x{config.SteeringMultiplier})");
            Console.WriteLine($"[UnSlowSeaTruck] Modified acceleration: {__instance.acceleration} (x{config.AccelerationMultiplier})");
        }
    }
}
