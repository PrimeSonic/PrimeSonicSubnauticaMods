namespace VehicleUpgradesInCyclops
{
    using System.Collections.Generic;
    using Common;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;

    [QModCore]
    public class QPatch
    {
        // This mod is intended to be replaced by the VModFabricator
        // But since some people still want it, it's kept up and maintained.
        [QModPatch]
        public static void Patch()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // Remove all original Cyclops fabricator nodes
            foreach (string origNodeID in OriginalCyclopsModuleCraftingNodes)
                CraftTreeHandler.RemoveNode(CraftTree.Type.CyclopsFabricator, origNodeID);

            QuickLogger.Info("Removed original crafting nodes from root of Cyclops Fabricator");
            // Recreates all the tabs from the Vehicle Upgrade Console

            foreach (ModulesTab tab in UpgradeModuleTabs)
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.CyclopsFabricator, tab.TabID, tab.TabName, tab.TabSprite);
                

                foreach (TechType craftTypeID in tab.CraftNodes)
                    CraftTreeHandler.AddCraftingNode(CraftTree.Type.CyclopsFabricator, craftTypeID, tab.TabID);

                QuickLogger.Info($"Added new tab '{tab.TabID}' to Cyclops Fabricator with '{tab.CraftNodes.Count}' crafting nodes");
            }

            QuickLogger.Info("Patching complete");
        }

        private static readonly List<ModulesTab> UpgradeModuleTabs = new List<ModulesTab>(5)
        {
            new ModulesTab("CyclopsModules", "Cyclops Modules", "Workbench_CyclopsMenu",
                new List<TechType>(8)
                {
                    TechType.CyclopsHullModule1,
                    TechType.PowerUpgradeModule,
                    TechType.CyclopsShieldModule,
                    TechType.CyclopsSonarModule,
                    TechType.CyclopsSeamothRepairModule,
                    TechType.CyclopsFireSuppressionModule,
                    TechType.CyclopsDecoyModule,
                    TechType.CyclopsThermalReactorModule
                }),

            new ModulesTab("CommonModules", "Common Modules", "SeamothUpgrades_CommonModules",
                new List<TechType>(3)
                {
                    TechType.VehicleArmorPlating,
                    TechType.VehiclePowerUpgradeModule,
                    TechType.VehicleStorageModule
                }),
            new ModulesTab("SeamothModules", "Seamoth Modules", "SeamothUpgrades_SeamothModules",
                new List<TechType>(5)
                {
                    TechType.VehicleHullModule1,
                    TechType.SeamothSolarCharge,
                    TechType.SeamothElectricalDefense,
                    TechType.SeamothTorpedoModule,
                    TechType.SeamothSonarModule
                }),

            new ModulesTab("ExosuitModules", "Prawn Suit Modules", "SeamothUpgrades_ExosuitModules",
                new List<TechType>(7)
                {
                    TechType.ExoHullModule1,
                    TechType.ExosuitThermalReactorModule,
                    TechType.ExosuitJetUpgradeModule,
                    TechType.ExosuitPropulsionArmModule,
                    TechType.ExosuitGrapplingArmModule,
                    TechType.ExosuitDrillArmModule,
                    TechType.ExosuitTorpedoArmModule
                }),

            new ModulesTab("Torpedoes", "Torpedoes", "SeamothUpgrades_Torpedoes",
                new List<TechType>(2)
                {
                    TechType.WhirlpoolTorpedo,
                    TechType.GasTorpedo,
                })
        };

        private static readonly IEnumerable<string> OriginalCyclopsModuleCraftingNodes = new string[8]
        {
            "CyclopsHullModule1",
            "PowerUpgradeModule",
            "CyclopsShieldModule",
            "CyclopsSonarModule",
            "CyclopsSeamothRepairModule",
            "CyclopsFireSuppressionModule",
            "CyclopsDecoyModule",
            "CyclopsThermalReactorModule"
        };
    }
}
