namespace UpgradedVehicles.Patchers
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patcher
    {
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]    
    [HarmonyPatch("vehicleDefaultName", PropertyMethod.Getter)]
    internal class SeaMoth_get_vehicleDefaultName_Patcher
    {
        internal static bool isSeamothMk2 = false;

        internal static void Prefix(ref SeaMoth __instance)
        {
            isSeamothMk2 = __instance.GetComponentInChildren<PrefabIdentifier>().ClassId == SeaMothMk2.NameID;
        }

        // TODO, find out why this isn't working
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (isSeamothMk2 && instruction.opcode.Equals(OpCodes.Brfalse))
                {
                    yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);
                    continue;
                }

                if (isSeamothMk2 && instruction.opcode.Equals(OpCodes.Ldstr) && instruction.operand.Equals("SEAMOTH"))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "SEAMOTH MK2");
                    continue;
                }

                yield return instruction;
            }
        }
    }

}
