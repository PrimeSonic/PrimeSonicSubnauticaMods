namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using Caching;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch("Update")]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        public static void Postfix(ref CyclopsHelmHUDManager __instance)
        {
            if (!__instance.LOD.IsFull() || // can't see
                !__instance.subLiveMixin.IsAlive()) // dead
            {
                return;
            }

            PowerManager powerMgr = CyclopsManager.GetPowerManager(__instance.subRoot);

            if (powerMgr == null)
            {
                return;
            }

            powerMgr.UpdateHelmHUD(__instance);
        }
    }

    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    [HarmonyPatch("Update")]
    internal class CyclopsHelmHUDManager_Update_Transpiler
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool injected = false;

            foreach (var instruction in instructions)
            {
                if (!injected && instruction.opcode.Equals(OpCodes.Ldstr) && instruction.operand.Equals("{0}%"))
                {
                    injected = true;
                    yield return new CodeInstruction(OpCodes.Ldstr, "----");
                    continue;
                }

                yield return instruction;
            }
        }
    }
}
