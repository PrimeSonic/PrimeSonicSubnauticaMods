namespace BetterBioReactor.Patchers
{
    using HarmonyLib;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/MrPurple6411/AssemblyPublicizer/releases/tag/MrP1.0
    [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA))]
    public class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Base BioReactor item container.

            if (__instance == null || !CyBioReactorMini.PdaIsOpen)
                return;

            CyBioReactorMini reactor = CyBioReactorMini.OpenInPda;
            reactor.ConnectToInventory(__instance.storage.items);
        }
    }
}
