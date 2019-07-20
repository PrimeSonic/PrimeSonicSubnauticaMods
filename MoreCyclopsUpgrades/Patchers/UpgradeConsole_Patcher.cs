namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;
    using MoreCyclopsUpgrades.Managers;

    [HarmonyPatch(typeof(UpgradeConsole))]
    [HarmonyPatch("OnHandClick")]
    internal static class UpgradeConsole_OnHandClick_Patcher
    {
        [HarmonyPrefix]
        public static void Prefix(UpgradeConsole __instance)
        {
            PdaOverlayManager.StartConnectingToPda(__instance.modules);
        }

        [HarmonyPostfix]
        public static void Postfix(UpgradeConsole __instance)
        {
            PDA pda = Player.main.GetPDA();
            pda.onClose = new PDA.OnClose((PDA closingPda) => PdaOverlayManager.DisconnectFromPda());
        }
    }
}
