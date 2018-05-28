namespace VehicleUpgradesInCyclops
{    
    using SMLHelper.Patchers; // https://github.com/ahk1221/SMLHelper/

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        // This mod is intended to be replaced by the VModFabricator

        public static void Patch()
        {
            // Remove all original Cyclops fabricator nodes
            CraftTreePatcher.nodesToRemove.AddRange(CraftingNodeLists.OriginalCyclopsModuleCraftingNodes);

            // New Cyclops Upgrades Tab (This will keep things more organized and prevent the icons from being rendered off screen when there's too many)
            CraftTreePatcher.customTabs.Add(CraftingNodeLists.CyclopsTab);
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CyclopsModuleCraftingNodes);

            // Common Modules
            CraftTreePatcher.customTabs.Add(CraftingNodeLists.CommonModuleTab);
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CommonVehicleModuleCraftingNodes);

            // Seamoth Modules
            CraftTreePatcher.customTabs.Add(CraftingNodeLists.SeamothModuleTab);
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.SeamothModuleCraftingNodes);

            // Prawn Suit Modules
            CraftTreePatcher.customTabs.Add(CraftingNodeLists.ExosuitModuleTab);
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.ExosuitModuleCraftingNodes);

            // Torpedoes
            CraftTreePatcher.customTabs.Add(CraftingNodeLists.TorpedoTab);
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.TorpedoCraftingNodes);
        }
    }
}
