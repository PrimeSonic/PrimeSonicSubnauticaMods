#if BELOWZERO
namespace UnSlowSeaTruck
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
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

            Harmony.CreateAndPatchAll(assembly, $"com.{assembly.GetName().Name}.psmod");

            ConfigMgr.SaveConfig(Config);
        }
    }

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.GetAttachedWeight))]
    internal static class SeaTruckSegment_GetAttachedWeight_Override
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result)
        {
            __result *= QPatch.Config.WeightOverride; // Override attached module weight
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

            Console.WriteLine($"[UnSlowSeaTruck] Modified steeringMultiplier: {__instance.steeringMultiplier} [Config:{config.SteeringMultiplier}] (More is faster)");
            Console.WriteLine($"[UnSlowSeaTruck] Modified acceleration: {__instance.acceleration} [Config:{config.AccelerationMultiplier}] (More is faster)");
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.GetWeight))]
    internal static class SeatruckMotor_GetWeight_Transpiler
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var allCodes = new List<CodeInstruction>(instructions);
            CodeInstruction instructionToAlter = null;

            foreach (var item in allCodes)
            {
                if (item.opcode == OpCodes.Ldc_R4)
                {
                    // Just in case they ever change the values or change the order of the operation
                    if ((instructionToAlter == null || (float)instructionToAlter.operand > (float)item.operand))
                    {
                        instructionToAlter = item;
                    }
                }
            }

            Console.WriteLine($"[UnSlowSeaTruck] Original HorsePowerModifier: {instructionToAlter.operand}");
            instructionToAlter.operand = QPatch.Config.HorsePowerModifier;
            Console.WriteLine($"[UnSlowSeaTruck] Modified HorsePowerModifier: {instructionToAlter.operand} (Less is faster)");

            return allCodes;
        }
    }
}
#endif