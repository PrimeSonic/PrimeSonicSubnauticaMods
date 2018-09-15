namespace MoreCyclopsUpgrades.Patchers
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch("RefreshUpgradeConsoleIcons")]
    internal class CyclopsHolographicHUD_Patcher
    {
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ret); // Should now be handled by SetCyclopsUpgrades
        }
    }
}
