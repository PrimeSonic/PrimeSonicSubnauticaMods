namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using MoreCyclopsUpgrades.Caching;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            var components = ComponentCache.Find(__instance.subRoot);

            if (components is null)
                return; // safety check

            components.UpgradeManager.SyncUpgradeConsoles(__instance.subRoot);

            components.PowerManager.UpdateConsoleHUD(__instance);
        }
    }
}
