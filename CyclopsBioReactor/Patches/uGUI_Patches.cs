namespace CyclopsBioReactor.Patches
{
    using System.Collections.Generic;
    using Harmony;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Cyclops Bioreactor item container.

            if (__instance is null)
                return; // Safety check

            if (!CyBioReactorMono.PdaIsOpen)
                return;

            ItemsContainer containerObj = __instance.storage.container;

            // Look for the reactor that matches the container we just opened.
            CyBioReactorMono reactor = CyBioReactorMono.OpenInPda;

            Dictionary<InventoryItem, uGUI_ItemIcon> lookup = __instance.storage.items;
            reactor.ConnectToInventory(lookup);
        }
    }
}
