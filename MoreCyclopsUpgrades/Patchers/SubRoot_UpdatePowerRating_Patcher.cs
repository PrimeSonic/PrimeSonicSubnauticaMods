namespace MoreCyclopsUpgrades.Patchers
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        public static void Prefix(ref SubRoot __instance, ref float __state)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            // Store the actual current power rating before we override it
            __state = PowerIndexManager.GetCyclopsPowerRating(ref __instance);

            // Force the original method to act as if nothing changed
            PowerIndexManager.SetCyclopsPowerRating(ref __instance, 1f);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // The statement below will catch the call to 'TechType.PowerUpgradeModule' in the following line of the original method:
                // this.currPowerRating = ((modules.GetCount(TechType.PowerUpgradeModule) <= 0) ? this.currPowerRating : 3f);
                if (instruction.opcode.Equals(OpCodes.Ldc_I4) && instruction.operand.Equals(1516))
                {
                    // The override will force evalution of GetCount to always return false by swapping in a TechType that will never be in the Upgrade Console.
                    // This guarantees that we'll always get 'this.currPowerRating' returned which we set to 1f in the PreFix.                    
                    // That way, we can skip over the original method's power rating message and use only the one in UpdatePowerIndex.

                    yield return new CodeInstruction(OpCodes.Ldc_I4, 1); // TechType 1 is Quartz in case you were wondering
                    continue;
                }

                yield return instruction;
            }
        }

        public static void Postfix(ref SubRoot __instance, ref float __state)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            // Handle the new power rating
            PowerIndexManager.UpdatePowerIndex(ref __instance, __state);
        }

    }
}
