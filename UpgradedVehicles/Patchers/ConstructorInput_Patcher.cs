namespace UpgradedVehicles.Patchers
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(ConstructorInput))]
    [HarmonyPatch("Craft")]
    internal class ConstructorInput_Patcher
    {
        private enum State : byte
        {
            Starting,
            TechTypeCheckReplaced,
            JumpReplaced,
            StLodReplaced,
            LdLocReplaced,
            DefaultDurationReplaced,
        }

        private static State state = State.Starting;

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction op in instructions)
            {
                if (state == State.Starting && op.opcode.Equals(OpCodes.Ldc_I4) && op.operand.Equals(TechType.Seamoth))
                {
                    // Ldc_I4 2000 >>> Ldc_I4 2003                    
                    op.operand = TechType.Cyclops;
                    state = State.TechTypeCheckReplaced;
                }
                else if (state == State.TechTypeCheckReplaced && !op.opcode.Equals(OpCodes.Brtrue))
                {
                    op.opcode = OpCodes.Nop;
                }
                else if (state == State.TechTypeCheckReplaced && op.opcode.Equals(OpCodes.Brtrue))
                {
                    // Brtrue IL_002C >>> Bne_Un_S IL_004E
                    op.opcode = OpCodes.Bne_Un_S;
                    state = State.JumpReplaced;
                }
                else if (state == State.TechTypeCheckReplaced && op.opcode.Equals(OpCodes.Stloc_3))
                {
                    op.opcode = OpCodes.Nop;
                    state = State.StLodReplaced;
                }
                else if (state == State.StLodReplaced && op.opcode.Equals(OpCodes.Ldloc_3))
                {
                    op.opcode = OpCodes.Nop;
                    state = State.LdLocReplaced;
                }
                else if (state == State.LdLocReplaced && op.opcode.Equals(OpCodes.Ldc_R4) && op.operand.Equals(3f))
                {
                    // Replace default duration of 3 with default duration of 10
                    // This is so new vehicles take the same duration as the SeaMoth and ExoSuit
                    op.operand = 10f;
                    state = State.DefaultDurationReplaced;
                }

                yield return op;
            }
        }

    }
}
