namespace MoreCyclopsUpgrades.Patchers
{
    using HarmonyLib;
    using MoreCyclopsUpgrades.Managers;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch(nameof(uGUI_InventoryTab.OnOpenPDA))]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.

            if (__instance == null)
                return;

            PdaOverlayManager.FinishingConnectingToPda(__instance.equipment);
        }
    }
}
