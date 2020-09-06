namespace CyclopsBioReactor.Patches
{
    using HarmonyLib;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/CabbageCrow/AssemblyPublicizer
    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch(nameof(uGUI_InventoryTab.OnOpenPDA))]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_InventoryTab __instance)
        {
            if (__instance == null || !CyBioReactorMono.PdaIsOpen)
                return;

            ItemsContainer containerObj = __instance.storage.container;
            CyBioReactorMono reactor = CyBioReactorMono.OpenInPda;
            reactor.ConnectToContainer(__instance.storage.items);
        }
    }
}
