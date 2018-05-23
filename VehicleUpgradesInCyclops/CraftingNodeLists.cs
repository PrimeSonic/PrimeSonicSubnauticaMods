namespace VehicleUpgradesInCyclops
{
    using SMLHelper; // https://github.com/ahk1221/SMLHelper/
    using System.Collections.Generic;

    internal static class CraftingNodeLists
    {
        internal const string CyclopsModules = "CyclopsModules";
        internal const string CommonModules = "CommonModules";
        internal const string SeamothModules = "SeamothModules";
        internal const string ExosuitModules = "ExosuitModules";
        internal const string Torpedoes = "Torpedoes";

        private class CyclopsCraftNode : CustomCraftNode
        {
            public CyclopsCraftNode(TechType techType, string root) :
                base(techType, CraftScheme.CyclopsFabricator, $"{root}/{techType}")
            {
            }
        }

        private class CyclopsCraftTab : CustomCraftTab
        {
            public CyclopsCraftTab(string root, string tabName, string categorySpriteName) : 
                base(root, tabName, CraftScheme.CyclopsFabricator, SpriteManager.Get(SpriteManager.Group.Category, categorySpriteName))
            {
            }
        }

        internal static CustomCraftTab CyclopsTab => new CyclopsCraftTab(CyclopsModules, "Cyclops Modules", "Workbench_CyclopsMenu");
        internal static CustomCraftTab CommonModuleTab => new CyclopsCraftTab(CommonModules, "Common Modules", "SeamothUpgrades_CommonModules");
        internal static CustomCraftTab SeamothModuleTab => new CyclopsCraftTab(SeamothModules, "Seamoth Modules", "SeamothUpgrades_SeamothModules");
        internal static CustomCraftTab ExosuitModuleTab => new CyclopsCraftTab(ExosuitModules, "Prawn Suit Modules", "SeamothUpgrades_ExosuitModules");
        internal static CustomCraftTab TorpedoTab => new CyclopsCraftTab(Torpedoes, "Torpedoes", "SeamothUpgrades_Torpedoes");

        internal static IEnumerable<CraftNodeToScrub> OriginalCyclopsModuleCraftingNodes => new CraftNodeToScrub[8]
            {
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsHullModule1"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "PowerUpgradeModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsShieldModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsSonarModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsSeamothRepairModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsFireSuppressionModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsDecoyModule"),
                new CraftNodeToScrub(CraftScheme.CyclopsFabricator, "CyclopsThermalReactorModule")
            };

        internal static IEnumerable<CustomCraftNode> CyclopsModuleCraftingNodes => new CustomCraftNode[8]
            {
                new CyclopsCraftNode(TechType.CyclopsHullModule1, CyclopsModules),
                new CyclopsCraftNode(TechType.PowerUpgradeModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsShieldModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsSonarModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsSeamothRepairModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsFireSuppressionModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsDecoyModule, CyclopsModules),
                new CyclopsCraftNode(TechType.CyclopsThermalReactorModule, CyclopsModules)
            };

        internal static IEnumerable<CustomCraftNode> CommonVehicleModuleCraftingNodes => new CustomCraftNode[3]
            {
                new CyclopsCraftNode(TechType.VehicleArmorPlating, CommonModules),
                new CyclopsCraftNode(TechType.VehiclePowerUpgradeModule, CommonModules),
                new CyclopsCraftNode(TechType.VehicleStorageModule, CommonModules)
            };

        internal static IEnumerable<CustomCraftNode> SeamothModuleCraftingNodes => new CustomCraftNode[5]
            {
                new CyclopsCraftNode(TechType.VehicleHullModule1, SeamothModules),
                new CyclopsCraftNode(TechType.SeamothSolarCharge, SeamothModules),
                new CyclopsCraftNode(TechType.SeamothElectricalDefense, SeamothModules),
                new CyclopsCraftNode(TechType.SeamothTorpedoModule, SeamothModules),
                new CyclopsCraftNode(TechType.SeamothSonarModule, SeamothModules),
            };

        internal static IEnumerable<CustomCraftNode> ExosuitModuleCraftingNodes => new CustomCraftNode[7]
            {
                new CyclopsCraftNode(TechType.ExoHullModule1, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitThermalReactorModule, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitJetUpgradeModule, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitPropulsionArmModule, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitGrapplingArmModule, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitDrillArmModule, ExosuitModules),
                new CyclopsCraftNode(TechType.ExosuitTorpedoArmModule, ExosuitModules)
            };

        internal static IEnumerable<CustomCraftNode> TorpedoCraftingNodes => new CustomCraftNode[2]
            {
                new CyclopsCraftNode(TechType.WhirlpoolTorpedo, Torpedoes),
                new CyclopsCraftNode(TechType.GasTorpedo, Torpedoes)
            };
    }
}
