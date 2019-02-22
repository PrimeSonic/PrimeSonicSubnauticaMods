namespace CustomCraftSMLTests
{
    using System;
    using System.Reflection;
    using Common;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Entries;
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
                        SpriteItemID = TechType.CyclopsHullModule1,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    },
                    new CfCustomCraftingTab
                    {
                        TabID = cyclopsRechargeTabID,
                        DisplayName = "Recharge Modules",
                        SpriteItemID = TechType.CyclopsHullModule1,
                        ParentTabPath = $"{vmodFabricatorID}/{cyclopsTabID}"
                    }
                },

            };

        }
    }
}
