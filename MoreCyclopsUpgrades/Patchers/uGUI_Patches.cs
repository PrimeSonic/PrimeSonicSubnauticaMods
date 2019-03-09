namespace MoreCyclopsUpgrades.SaveData
{
    using System.Collections.Generic;
    using Harmony;
    using Managers;
    using Monobehaviors;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")]
    public class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_InventoryTab __instance)
        {
            // This event happens whenever the player opens their PDA.
            // We will make a series of checks to see if what they have opened is the Cyclops Bioreactor item container.

            if (__instance is null)
                return; // Safety check

            if (!Player.main.IsInSub() || !Player.main.currentSub.isCyclops)
                return; // If not in Cyclops then all is irrelevant

            if (__instance.storage is null)
                return; // Safety check

            ItemsContainer containerObj = __instance.storage.container;

            if (containerObj is null)
                return; // If this isn't a non-null ItemsContainer, then it's not what we want.

            string label = containerObj._label;

            if (label != CyBioReactorMono.StorageLabel)
                return; // Not a Cyclops Bioreactor storage

            List<CyBioReactorMono> reactors = CyclopsManager.GetBioReactors(Player.main.currentSub);

            if (reactors is null || reactors.Count == 0)
                return; // Cyclops has no bioreactors

            // Look for the reactor that matches the container we just opened.
            CyBioReactorMono reactor = reactors.Find(r => r.Container == containerObj);

            if (reactor is null)
                return; // Didn't find the reactor we were looking for. Could it be on another cyclops?

            Dictionary<InventoryItem, uGUI_ItemIcon> lookup = __instance.storage.items;
            reactor.ConnectToInventory(lookup); // Found!
        }
    }
}
