namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(CyclopsHolographicHUD))]
    [HarmonyPatch("RefreshUpgradeConsoleIcons")]
    internal class CyclopsHolographicHUD_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(CyclopsHolographicHUD __instance)
        {
            return false; // Should now be handled by SetCyclopsUpgrades
        }
    }
}
