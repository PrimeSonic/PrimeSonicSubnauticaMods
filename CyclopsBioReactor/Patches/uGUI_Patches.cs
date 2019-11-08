namespace CyclopsBioReactor.Patches
{
    using Harmony;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/CabbageCrow/AssemblyPublicizer
    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch(nameof(uGUI_InventoryTab.OnOpenPDA))]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Cyclops Bioreactor item container.

            if (__instance == null || !CyBioReactorMono.PdaIsOpen)
                return;

            ItemsContainer containerObj = __instance.storage.container;
            CyBioReactorMono reactor = CyBioReactorMono.OpenInPda;
            reactor.ConnectToInventory(__instance.storage.items);
        }
    }
}
