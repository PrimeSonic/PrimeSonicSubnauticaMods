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
                    yield return op;
                }
                else if (callCount < 4 && op.opcode.Equals(OpCodes.Call))
                {
                    callCount++;
                }

                yield return op;
            }
        }
    }

    [HarmonyPatch(typeof(CrushDamage))]
    [HarmonyPatch("Awake")]
    internal class CrushDamage_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref CrushDamage __instance)
        {
            var seamoth = __instance.GetComponentInParent<SeaMoth>();
            var exosuit = __instance.GetComponentInParent<Exosuit>();

            if (seamoth != null)
            {
                VehicleUpgrader.UpgradeSeaMoth(seamoth);
            }
            else if (exosuit != null)
            {
                VehicleUpgrader.UpgradeExosuit(exosuit);
            }

        }
    }
}
