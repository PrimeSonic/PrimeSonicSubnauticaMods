//namespace UpgradedVehicles
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Reflection.Emit;
//    using Harmony;

//    [HarmonyPatch(typeof(ConstructorInput))]
//    [HarmonyPatch("Craft")]
//    internal static class ConstructorInput_Patcher
//    {
//        private static bool requiredDepthCheckOverride = false;

//        internal static void Prefix(TechType techType)
//        {
//            requiredDepthCheckOverride = techType == SeaMothMk2.TechTypeID;
//            Console.WriteLine($"[UpgradedVehicles] requiredDepthCheckOverride : {requiredDepthCheckOverride}");
//        }

//        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//        {
//            foreach (var instruction in instructions)
//            {                 
//                if (requiredDepthCheckOverride && instruction.opcode.Equals(OpCodes.Ldc_I4) && instruction.operand.Equals(TechType.Seamoth))
//                {
//                    yield return new CodeInstruction(OpCodes.Ldc_I4, SeaMothMk2.TechTypeID);
//                    requiredDepthCheckOverride = false;
//                    continue;
//                }

//                yield return instruction;
//            }
//        }

//    }
//}
