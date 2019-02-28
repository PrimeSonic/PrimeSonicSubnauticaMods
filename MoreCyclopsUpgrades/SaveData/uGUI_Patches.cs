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
                return; // Not a Cyclops BioReactor storage

            List<CyBioReactorMono> reactors = CyclopsManager.GetBioReactors(container, Player.main.currentSub);

            if (reactors is null)
                return; // Cyclops has no bioreactors?

            foreach (CyBioReactorMono reactor in reactors)
            {
                if (container == reactor.Container)
                {
                    var lookup = (Dictionary<InventoryItem, uGUI_ItemIcon>)itemsInfo.GetValue(__instance.storage);
                    reactor.ConnectToInventory(lookup); // Found!
                    return;
                }
            }
        }
    }
}
