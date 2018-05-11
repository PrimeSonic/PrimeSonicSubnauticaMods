namespace VehicleUpgradesInCyclops
{
    using SMLHelper; // https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    
#if DEBUG
    using System;
    using Logger = Utilites.Logger.Logger;
    // TODO Custom sprites for an look and feel more similar to the actual Vehicle Upgrade Console.    
#endif
    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {
                Logger.Debug("Loading Crafting Nodes - Start");
#endif
                // Remove all original Cyclops fabricator nodes
                CraftTreePatcher.nodesToRemove.AddRange(CraftingNodeLists.OriginalCyclopsModuleCraftingNodes());

                // New Cyclops Upgrades Tab (This will keep things more organized)
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("CyclopsModules", "Cyclops Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.Cyclops)));
                CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CyclopsModuleCraftingNodes());

                // Common Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("CommonModules", "Common Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.VehicleStorageModule)));
                CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.CommonVehicleModuleCraftingNodes());

                // Seamoth Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("SeamothModules", "Seamoth Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.Seamoth)));
                CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.SeamothModuleCraftingNodes());

                // Prawn Suit Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("ExosuitModules", "Prawn Suit Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.Exosuit)));
                CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.ExosuitModuleCraftingNodes());

                // Torpedoes
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("Torpedoes", "Torpedoes", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.SeamothTorpedoModule)));
                CraftTreePatcher.customNodes.AddRange(CraftingNodeLists.TorpedoCraftingNodes());
#if DEBUG
                Logger.Debug("Loading Crafting Nodes - Completes");
            }
            catch (Exception ex)
            {
                Logger.Error("Error on load" + ex.ToString());
            }
#endif
        }

    }
}
