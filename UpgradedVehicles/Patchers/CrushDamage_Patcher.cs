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
            // This is just to remove the on-screen message from being shown
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

    [HarmonyPatch(typeof(CrushDamage))]
    [HarmonyPatch("CrushDamageUpdate")]
    internal class CrushDamage_CrushDamageUpdate_Patcher
    {
        

        [HarmonyPrefix]
        internal static bool Prefix(ref CrushDamage __instance)
        {
            // true will return control to the original method
            // false will prevent this method from executing until CrushDepthSet is done
            return VehicleUpgrader.CrushDepthSet;
        }
    }
}
