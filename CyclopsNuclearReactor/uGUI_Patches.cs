namespace CyclopsNuclearReactor
{
    using Harmony;
    using System.Collections.Generic;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Cyclops Bioreactor item container.

            if (__instance == null)
                return; // Safety check

            if (!Player.main.IsInSub() || !Player.main.currentSub.isCyclops)
                return; // If not in Cyclops then all is irrelevant

            uGUI_ItemsContainer storageUI = __instance.storage;

            if (storageUI == null)
                return; // Not an equipment container

            var container = (ItemsContainer)AccessTools.Field(typeof(uGUI_ItemsContainer), "container").GetValue(storageUI);

            if (container == null)
                return; // Safety check

            string label = (container as IItemsContainer).label;

            if (label != CyNukReactorSMLHelper.StorageLabel())
                return; // Not a CyNukReactor

            List<CyNukeReactorMono> reactors = CyNukeChargeManager.GetReactors(Player.main.currentSub);

            if (reactors == null || reactors.Count == 0)
                return; // Cyclops has no reactors

            // Look for the reactor that matches the container we just opened.
            CyNukeReactorMono reactor = reactors.Find(r => r.RodsContainer == container);

            if (reactor == null)
                return; // Didn't find the reactor we were looking for. Could it be on another cyclops?

            var lookup = (Dictionary<InventoryItem, uGUI_ItemIcon>)AccessTools.Field(typeof(uGUI_ItemsContainer), "items").GetValue(storageUI);
            reactor.ConnectToContainer(lookup); // Found!
        }
    }
}
