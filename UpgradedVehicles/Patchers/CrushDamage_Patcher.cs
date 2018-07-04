namespace UpgradedVehicles.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text;
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
                if (callCount >= 4)
                {
                    op.opcode = OpCodes.Nop;
                    op.operand = null;
                }
                else if (op.opcode.Equals(OpCodes.Call))
                {
                    callCount++;
                }

                yield return op;
            }
        }

    }
}
