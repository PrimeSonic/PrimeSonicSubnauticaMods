namespace VehicleUpgradesInCyclops
{
    using System.Collections.Generic;
    using SMLHelper.V2.Handlers;

    internal static class NodeCollections
    {
        internal static void CheckForCrossModAdditions()
        {
            if (TechTypeHandler.TryGetModdedTechType("CyclopsSolarCharger", out TechType solarChargerID))
            {
                UpgradeModuleTabs[0].CraftNodes.Add(solarChargerID);
            }

            if (TechTypeHandler.TryGetModdedTechType("CyclopsSpeedModule", out TechType cyclopsSpeedModuleID))
            {
                UpgradeModuleTabs[0].CraftNodes.Add(cyclopsSpeedModuleID);
            }

            if (TechTypeHandler.TryGetModdedTechType("BioReactorBooster", out TechType bioReactorBooster))
            {
                UpgradeModuleTabs[0].CraftNodes.Add(bioReactorBooster);
            }

            if (TechTypeHandler.TryGetModdedTechType("SpeedModule", out TechType speedModuleID))
            {
                UpgradeModuleTabs[1].CraftNodes.Add(speedModuleID);
            }
        }

        internal static List<ModulesTab> UpgradeModuleTabs = new List<ModulesTab>(5)
        {
            new ModulesTab("CyclopsModules", "Cyclops Modules", "Workbench_CyclopsMenu",
                new List<TechType>(8+3)
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
                new List<TechType>(3+2)
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

        internal static IEnumerable<string> OriginalCyclopsModuleCraftingNodes => new string[8]
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
