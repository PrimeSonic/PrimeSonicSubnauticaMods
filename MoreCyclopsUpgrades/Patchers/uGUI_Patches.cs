namespace MoreCyclopsUpgrades.SaveData
{
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Monobehaviors;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch("OnOpenPDA")]
    public class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        private static readonly FieldInfo containerInfo = typeof(uGUI_ItemsContainer).GetField("container", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo labelInfo = typeof(ItemsContainer).GetField("_label", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo itemsInfo = typeof(uGUI_ItemsContainer).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);

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

            object containerObj = containerInfo.GetValue(__instance.storage);

            if (containerObj is null || !(containerObj is ItemsContainer container))
                return; // If this isn't a non-null ItemsContainer, then it's not what we want.

            string label = (string)labelInfo.GetValue(container);

            if (label != CyBioReactorMono.StorageLabel)
                return; // Not a Cyclops Bioreactor storage

            List<CyBioReactorMono> reactors = CyclopsManager.GetBioReactors(container, Player.main.currentSub);

            if (reactors is null || reactors.Count == 0)
                return; // Cyclops has no bioreactors

            // Look for the reactor that matches the container we just opened.
            CyBioReactorMono reactor = reactors.Find(r => r.Container == container);

            if (reactor is null)
                return; // Didn't find the reactor we were looking for. Could it be on another cyclops?

            var lookup = (Dictionary<InventoryItem, uGUI_ItemIcon>)itemsInfo.GetValue(__instance.storage);
            reactor.ConnectToInventory(lookup); // Found!
        }
    }
}
