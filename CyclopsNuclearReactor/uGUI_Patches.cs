namespace CyclopsNuclearReactor
{
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    [HarmonyPatch(nameof(uGUI_InventoryTab.OnOpenPDA))]
    internal class UGUI_InventoryTab_OnOpenPDA_Patcher
    {
        private static readonly Type uGUIContainerType = typeof(uGUI_ItemsContainer);
        private static readonly FieldInfo containerField = AccessTools.Field(uGUIContainerType, "container");
        private static readonly FieldInfo itemsField = AccessTools.Field(uGUIContainerType, "items");

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

            var container = (ItemsContainer)containerField.GetValue(storageUI);

            if (container == null)
                return; // Safety check

            string label = (container as IItemsContainer).label;

            if (label != CyNukReactorBuildable.StorageLabel())
                return; // Not a CyNukReactor

            List<CyNukeReactorMono> reactors = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(Player.main.currentSub)?.CyNukeReactors;

            if (reactors == null || reactors.Count == 0)
                return; // Cyclops has no reactors

            // Look for the reactor that matches the container we just opened.
            CyNukeReactorMono reactor = reactors.Find(r => r.RodsContainer == container);

            if (reactor == null)
                return; // Didn't find the reactor we were looking for. Could it be on another cyclops?

            var lookup = (Dictionary<InventoryItem, uGUI_ItemIcon>)itemsField.GetValue(storageUI);
            reactor.ConnectToContainer(lookup); // Found!
        }
    }
}
