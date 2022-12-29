#if BELOWZERO
namespace UnSlowSeaTruck
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using BepInEx;
    using HarmonyLib;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Vehicle Upgrades In Cyclops",
            AUTHOR = "PrimeSonic",
            GUID = "com.vehicleupgradesincyclops.psmod",
            VERSION = "1.0.0.0";
        #endregion
        internal static SeaTruckConfig SMLConfig;

        public void Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("[UnSlowSeaTruck] Patching v" + assembly.GetName().Version.ToString(3));

            SMLConfig = ConfigMgr.LoadConfig<SeaTruckConfig>();
            Console.WriteLine($"[UnSlowSeaTruck] Configured WeightOverride: {SMLConfig.WeightOverride}");

            Harmony.CreateAndPatchAll(assembly, $"com.{assembly.GetName().Name}.psmod");

            ConfigMgr.SaveConfig(SMLConfig);
        }
    }

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.GetAttachedWeight))]
    internal static class SeaTruckSegment_GetAttachedWeight_Override
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result)
        {
            __result *= Plugin.SMLConfig.WeightOverride; // Override attached module weight
        }
    }

    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.Start))]
    internal static class SeatruckMotor_Start_Change
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaTruckMotor __instance)
        {
            SeaTruckConfig config = Plugin.SMLConfig;

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
            instructionToAlter.operand = Plugin.SMLConfig.HorsePowerModifier;
            Console.WriteLine($"[UnSlowSeaTruck] Modified HorsePowerModifier: {instructionToAlter.operand} (Less is faster)");

            return allCodes;
        }
    }
}
#endif