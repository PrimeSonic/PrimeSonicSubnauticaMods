namespace VehicleUpgradesInCyclops
{
    using SMLHelper; // https://github.com/ahk1221/SMLHelper/
    using System.Collections.Generic;

    internal static class CraftingNodeLists
    {
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
                new CustomCraftNode(TechType.CyclopsHullModule1, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsHullModule1"),
                new CustomCraftNode(TechType.PowerUpgradeModule, CraftScheme.CyclopsFabricator, "CyclopsModules/PowerUpgradeModule"),
                new CustomCraftNode(TechType.CyclopsShieldModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsShieldModule"),
                new CustomCraftNode(TechType.CyclopsSonarModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsSonarModule"),
                new CustomCraftNode(TechType.CyclopsSeamothRepairModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsSeamothRepairModule"),
                new CustomCraftNode(TechType.CyclopsFireSuppressionModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsFireSuppressionModule"),
                new CustomCraftNode(TechType.CyclopsDecoyModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsDecoyModule"),
                new CustomCraftNode(TechType.CyclopsThermalReactorModule, CraftScheme.CyclopsFabricator, "CyclopsModules/CyclopsThermalReactorModule")
            };

        internal static IEnumerable<CustomCraftNode> CommonVehicleModuleCraftingNodes => new CustomCraftNode[3]
            {
                new CustomCraftNode(TechType.VehicleArmorPlating, CraftScheme.CyclopsFabricator, "CommonModules/VehicleArmorPlating"),
                new CustomCraftNode(TechType.VehiclePowerUpgradeModule, CraftScheme.CyclopsFabricator, "CommonModules/VehiclePowerUpgradeModule"),
                new CustomCraftNode(TechType.VehicleStorageModule, CraftScheme.CyclopsFabricator, "CommonModules/VehicleStorageModule")
            };

        internal static IEnumerable<CustomCraftNode> SeamothModuleCraftingNodes => new CustomCraftNode[5]
            {
                new CustomCraftNode(TechType.VehicleHullModule1, CraftScheme.CyclopsFabricator, "SeamothModules/VehicleHullModule1"),
                new CustomCraftNode(TechType.SeamothSolarCharge, CraftScheme.CyclopsFabricator, "SeamothModules/SeamothSolarCharge"),
                new CustomCraftNode(TechType.SeamothElectricalDefense, CraftScheme.CyclopsFabricator, "SeamothModules/SeamothElectricalDefense"),
                new CustomCraftNode(TechType.SeamothTorpedoModule, CraftScheme.CyclopsFabricator, "SeamothModules/SeamothTorpedoModule"),
                new CustomCraftNode(TechType.SeamothSonarModule, CraftScheme.CyclopsFabricator, "SeamothModules/SeamothSonarModule"),
            };

        internal static IEnumerable<CustomCraftNode> ExosuitModuleCraftingNodes => new CustomCraftNode[7]
            {
                new CustomCraftNode(TechType.ExoHullModule1, CraftScheme.CyclopsFabricator, "ExosuitModules/ExoHullModule1"),
                new CustomCraftNode(TechType.ExosuitThermalReactorModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitThermalReactorModule"),
                new CustomCraftNode(TechType.ExosuitJetUpgradeModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitJetUpgradeModule"),
                new CustomCraftNode(TechType.ExosuitPropulsionArmModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitPropulsionArmModule"),
                new CustomCraftNode(TechType.ExosuitGrapplingArmModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitGrapplingArmModule"),
                new CustomCraftNode(TechType.ExosuitDrillArmModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitDrillArmModule"),
                new CustomCraftNode(TechType.ExosuitTorpedoArmModule, CraftScheme.CyclopsFabricator, "ExosuitModules/ExosuitTorpedoArmModule")
            };

        internal static IEnumerable<CustomCraftNode> TorpedoCraftingNodes => new CustomCraftNode[2]
            {
                new CustomCraftNode(TechType.WhirlpoolTorpedo, CraftScheme.CyclopsFabricator, "Torpedoes/WhirlpoolTorpedo"),
                new CustomCraftNode(TechType.GasTorpedo, CraftScheme.CyclopsFabricator, "Torpedoes/GasTorpedo")
            };
    }
}
