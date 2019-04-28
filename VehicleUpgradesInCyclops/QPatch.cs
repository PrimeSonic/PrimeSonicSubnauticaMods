namespace VehicleUpgradesInCyclops
{
    using Common;
    using SMLHelper.V2.Handlers;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        // This mod is intended to be replaced by the VModFabricator
        // But since some people still want it, it's kept up and maintained.
        public static void Patch()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // Remove all original Cyclops fabricator nodes
            foreach (string origNodeID in NodeCollections.OriginalCyclopsModuleCraftingNodes)
                CraftTreeHandler.RemoveNode(CraftTree.Type.CyclopsFabricator, origNodeID);
                        
            // Recreates all the tabs from the Vehicle Upgrade Console

            foreach (ModulesTab tab in NodeCollections.UpgradeModuleTabs)
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.CyclopsFabricator, tab.TabID, tab.TabName, tab.TabSprite);

                foreach (TechType craftTypeID in tab.CraftNodes)
                    CraftTreeHandler.AddCraftingNode(CraftTree.Type.CyclopsFabricator, craftTypeID, tab.TabID);
            }

            QuickLogger.Info("Patching complete");
        }
    }
}
