namespace BetterBioReactor.Patchers
{
    using HarmonyLib;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/CabbageCrow/AssemblyPublicizer
    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")] // uGUI_InventoryTab.OnOpenPDA
    public class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Base BioReactor item container.

            if (__instance == null || !CyBioReactorMini.PdaIsOpen)
                return;

            ItemsContainer currentContainer = __instance.storage.container;
            CyBioReactorMini reactor = CyBioReactorMini.OpenInPda;
            reactor.ConnectToInventory(__instance.storage.items);
        }
    }
}
