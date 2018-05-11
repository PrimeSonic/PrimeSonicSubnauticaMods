namespace VehicleUpgradesInCyclops
{
    using SMLHelper; // https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;

    public class QPatch
    {
        public static void Patch()
        {
            // Remove all original Cyclops fabricator nodes
            CraftTreePatcher.nodesToRemove.AddRange(CraftingNodeLists.OriginalCyclopsModuleCraftingNodes);

            // New Cyclops Upgrades Tab (This will keep things more organized and prevent the icons from being rendered off screen when there's too many)
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("CyclopsModules", "Cyclops Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, "Constructor_Vehicles")));
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CyclopsModuleCraftingNodes);

            // Common Modules
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("CommonModules", "Common Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_CommonModules")));
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CommonVehicleModuleCraftingNodes);

            // Seamoth Modules
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("SeamothModules", "Seamoth Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_SeamothModules")));
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.SeamothModuleCraftingNodes);

            // Prawn Suit Modules
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("ExosuitModules", "Prawn Suit Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_ExosuitModules")));
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.ExosuitModuleCraftingNodes);

            // Torpedoes
            CraftTreePatcher.customTabs.Add(new CustomCraftTab("Torpedoes", "Torpedoes", CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, "SeamothUpgrades_Torpedoes")));
            CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.TorpedoCraftingNodes);
        }
    }
}
