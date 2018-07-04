namespace UpgradedVehicles.Patchers
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(CrushDamage))]
    [HarmonyPatch("UpdateDepthClassification")]
    internal class CrushDamage_UpdateDepthClassification_Patcher
    {
        private static byte callCount = 0;

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction op in instructions)
            {
                if (callCount == 4)
                {
                    callCount++;
                    yield return new CodeInstruction(OpCodes.Ret);                    
                }
                else if (callCount < 4 && op.opcode.Equals(OpCodes.Call))
                {
                    callCount++;
                }

                yield return op;
            }
        }
    }
}
