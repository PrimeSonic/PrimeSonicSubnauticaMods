namespace MoreCyclopsUpgrades.Patchers
{
    using Caching;
    using Harmony;

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager))]
    [HarmonyPatch("RefreshScreen")]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref CyclopsUpgradeConsoleHUDManager __instance)
        {
            var cyclopsManager = CyclopsManager.GetAllManagers(__instance.subRoot);

            if (cyclopsManager == null)
            {
                return;
            }

            cyclopsManager.UpgradeManager.SyncUpgradeConsoles();
            cyclopsManager.PowerManager.SyncBioReactors();
            cyclopsManager.PowerManager.UpdateConsoleHUD(__instance);
        }
    }
}
