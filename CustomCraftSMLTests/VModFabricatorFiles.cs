namespace CustomCraftSMLTests
{
    using System;
    using System.Reflection;
    using Common;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    internal class VModFabricatorFiles
    {
        private const string vmodFabricatorID = "VModFabricatorCC2";
        private const string cyclopsTabID = "CyclopsModulesCC2";
        private const string cyclopsAbilityTabID = "CyclopsAbilityModulesCC2";
        private const string cyclopsDepthTabID = "CyclopsDepthModulesCC2";
        private const string cyclopsPowerTabID = "CyclopsPowerModulesCC2";
        private const string cyclopsRechargeTabID = "CyclopsRechargeTabCC2";
        private const string exosuitTabID = "ExosuitModulesCC2";
        private const string exosuitDepthTabID = "ExosuitDepthModulesCC2";
        private const string seamothTabID = "SeamothModulesCC2";
        private const string seamothDepthTabID = "SeamothDepthModulesCC2";
        private const string seamothAbilityTabID = "SeamothAbilityModulesCC2";
        private const string commonModulesTabID = "CommonModulesCC2";
        private const string torpedoesTabID = "TorpedoesModulesCC2";

        private static readonly string Today = DateTime.Today.ToString("dd/MMMM/yyyy");
        private static readonly string CC2Version = QuickLogger.GetAssemblyVersion(Assembly.GetAssembly(typeof(CustomCraft2SML.QPatch)));

        [Test]
        public void Generate()
        {
            var vmodFabricator = new CustomFabricator
            {
                ItemID = vmodFabricatorID,
                DisplayName = "Vehicle Module Fabricator",
                Tooltip = "Construct vehicle upgrade modules from the comfort of your favorite habitat or cyclops.",
                ForceUnlockAtStart = false,
                UnlockedBy = { $"{TechType.Workbench}", $"{TechType.BaseUpgradeConsole}", $"{TechType.Cyclops}" },
                PdaGroup = TechGroup.InteriorModules,
                PdaCategory = TechCategory.InteriorModule,
                AllowedInBase = true,
                AllowedInCyclops = true,
                Model = ModelTypes.MoonPool,
                Ingredients =
                {
                    new EmIngredient(TechType.Titanium, 2),
                    new EmIngredient(TechType.ComputerChip),
                    new EmIngredient(TechType.Diamond),
                    new EmIngredient(TechType.Lead),
                },
                CustomCraftingTabs =
                {
                    // Cyclops tabs
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsTabID,
                        DisplayName = "Cyclops Modules",
                        SpriteItemID = TechType.Cyclops,
                        ParentTabPath = vmodFabricatorID
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsAbilityTabID,
                        DisplayName = "Ability Modules",
                        SpriteItemID = TechType.CyclopsShieldModule,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsDepthTabID,
                        DisplayName = "Depth Modules",
                        SpriteItemID = TechType.CyclopsHullModule1,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsPowerTabID,
                        DisplayName = "Power Modules",
                        SpriteItemID = TechType.PowerUpgradeModule,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsRechargeTabID,
                        DisplayName = "Recharge Modules",
                        SpriteItemID = TechType.CyclopsThermalReactorModule,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    },
                    // Exosuit tabs
                    new CfCustomCraftingTab
                    {
                        TabID = exosuitTabID,
                        DisplayName = "Prawn Suit Modules",
                        SpriteItemID = TechType.Exosuit,
                        ParentTabPath = vmodFabricatorID
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = exosuitDepthTabID,
                        DisplayName = "Depth Modules",
                        SpriteItemID = TechType.ExoHullModule1,
                        ParentTabPath = $"{vmodFabricatorID}/{exosuitTabID}"
                    },
                    // Seamoth tabs
                    new CfCustomCraftingTab
                    {
                        TabID = seamothTabID,
                        DisplayName = "Seamoth Modules",
                        SpriteItemID = TechType.Seamoth,
                        ParentTabPath = vmodFabricatorID
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = seamothDepthTabID,
                        DisplayName = "Depth Modules",
                        SpriteItemID = TechType.VehicleHullModule1,
                        ParentTabPath = $"{vmodFabricatorID}/{seamothTabID}"
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = seamothAbilityTabID,
                        DisplayName = "Ability Modules",
                        SpriteItemID = TechType.SeamothElectricalDefense,
                        ParentTabPath = $"{vmodFabricatorID}/{seamothTabID}"
                    },
                    // Common tab
                    new CfCustomCraftingTab
                    {
                        TabID = commonModulesTabID,
                        DisplayName = "Common Modules",
                        SpriteItemID = TechType.VehicleArmorPlating,
                        ParentTabPath = vmodFabricatorID
                    },
                    // Torpedo tab
                    new CfCustomCraftingTab
                    {
                        TabID = torpedoesTabID,
                        DisplayName = "Torpedoes",
                        SpriteItemID = TechType.WhirlpoolTorpedo,
                        ParentTabPath = vmodFabricatorID
                    },
                },
                MovedRecipes =
                {
                    // Cyclops ability modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsShieldModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsSonarModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsSeamothRepairModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsFireSuppressionModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsDecoyModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsAbilityTabID}",
                    },
                    // Cyclops depth modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsHullModule1.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsHullModule2.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsHullModule3.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsDepthTabID}",
                    },
                    // Cyclops power modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.PowerUpgradeModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsPowerTabID}",
                    },
                    // Cyclops power modules - modded
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "PowerUpgradeModuleMk2",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsPowerTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "PowerUpgradeModuleMk3",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsPowerTabID}",
                    },
                    // Cyclops Recharge modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.CyclopsThermalReactorModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsRechargeTabID}",
                    },
                    // Cyclops Recharge modules - Modded
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "CyclopsThermalChargerMk2",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsRechargeTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "CyclopsSolarCharger",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsRechargeTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "CyclopsSolarChargerMk2",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsRechargeTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "CyclopsNuclearModule",
                        NewPath = $"{vmodFabricatorID}/{cyclopsTabID}/{cyclopsRechargeTabID}",
                    },
                    // Exosuit modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExosuitJetUpgradeModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExosuitPropulsionArmModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExosuitGrapplingArmModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExosuitDrillArmModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExosuitTorpedoArmModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}",
                    },
                    // Exosuit depth modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExoHullModule1.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}/{exosuitDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.ExoHullModule2.ToString(),
                        NewPath = $"{vmodFabricatorID}/{exosuitTabID}/{exosuitDepthTabID}",
                    },
                    // Seamoth modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.SeamothSolarCharge.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SeamothThermalModule",
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}",
                    },
                    // Seamoth depth modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehicleHullModule1.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehicleHullModule2.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehicleHullModule3.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothDepthTabID}",
                    },
                    // Seamoth depth modules - modded
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SeamothHullModule4",
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothDepthTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SeamothHullModule5",
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothDepthTabID}",
                    },
                    // Seamoth ability modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.SeamothElectricalDefense.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.SeamothSonarModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.SeamothTorpedoModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothAbilityTabID}",
                    },
                    // Seamoth ability modules - modded
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SeamothDrillModule",
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothAbilityTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SeamothClawModule",
                        NewPath = $"{vmodFabricatorID}/{seamothTabID}/{seamothAbilityTabID}",
                    },
                    // Common modules
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehicleArmorPlating.ToString(),
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehiclePowerUpgradeModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.VehicleStorageModule.ToString(),
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    // Common modules - modded
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "SpeedModule",
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "ScannerModule",
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "RepairModule",
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = "LaserCannon",
                        NewPath = $"{vmodFabricatorID}/{commonModulesTabID}",
                    },
                    // Torpedoes
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.WhirlpoolTorpedo.ToString(),
                        NewPath = $"{vmodFabricatorID}/{torpedoesTabID}",
                    },
                    new CfMovedRecipe
                    {
                        Copied = true,
                        ItemID = TechType.GasTorpedo.ToString(),
                        NewPath = $"{vmodFabricatorID}/{torpedoesTabID}",
                    },
                }
            };

            var list = new CustomFabricatorList
            {
                vmodFabricator
            };

            string serialized = list.ToString();

            Console.WriteLine(serialized);
        }
    }
}
