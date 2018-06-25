//namespace UpgradedVehicles.Patchers
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Reflection.Emit;
//    using Harmony;

//    [HarmonyPatch(typeof(ConstructorInput))]
//    [HarmonyPatch("Craft")]
//    internal static class ConstructorInput_Patcher
//    {

//        private static short checks = 0;

//        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//        {
//            foreach (var instruction in instructions)
//            {
//               yield return instruction;
//            }
//        }

//    }
//}
