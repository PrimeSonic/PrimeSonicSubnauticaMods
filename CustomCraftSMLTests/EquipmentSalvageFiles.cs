﻿namespace CustomCraftSMLTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    internal class EquipmentSalvageFiles
    {
        private static string EquipmentSalvageDirectory
        {
            get
            {
                string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                path = Directory.GetParent(path).FullName;
                path = Directory.GetParent(path).FullName;
                return Directory.GetParent(path).FullName + "/CustomCraftSML/SampleFiles/EquipmentSalvage/";
            }
        }

        private const string Line = "------------------------------------";
        private const string SalvageTabID = "SalvageTab";
        private const string PathToSalvageTab = "Fabricator/Resources";
        private const string SalvageCraftingTab = PathToSalvageTab + "/" + SalvageTabID;

        private static readonly string Today = DateTime.Today.ToString("dd/MMMM/yyyy");
        private static readonly string CC2Version = QuickLogger.GetAssemblyVersion(Assembly.GetAssembly(typeof(CustomCraft2SML.QPatch)));

        private static readonly string[] TopLines = new[]
        {
            "Equipment Salvage",
            "Created for Custom Craft 2",
            "Author: PrimeSonic",
            "Text files included in this mod:",
            "     EquipmentSalvage_Tabs.txt",
            "     EquipmentSalvage_Moves.txt",
            "     EquipmentSalvage_Recipes.txt",
        };

        private static readonly string[] BottomLines = new[]
        {
            "Published on Nexus ~ https://www.nexusmods.com/subnautica/mods/188",
            "Source code to all my Subnautica mods available on GitHub ~ https://github.com/PrimeSonic/PrimeSonicSubnauticaMods",
            $"This file was generated by EasyMarkup code on {Today} for Custom Craft 2 version {CC2Version}",
        };

        private static void WriteFile<T>(T tabList, string fileName) where T : EmProperty
        {
            string filePath = EquipmentSalvageDirectory + fileName;

            var linesToWrite = new List<string>();
            linesToWrite.AddRange(EmUtils.CommentTextLines(TopLines));
            linesToWrite.Add(EmUtils.CommentText(Line));
            linesToWrite.Add(tabList.PrettyPrint());
            linesToWrite.Add(EmUtils.CommentText(Line));
            linesToWrite.AddRange(EmUtils.CommentTextLines(BottomLines));

            File.WriteAllLines(filePath, linesToWrite);
        }

        private static readonly bool EnableUnlocking = true;

        [Test]
        public void Generate_All_Files()
        {
            // TABS
            var salvageTab = new CustomCraftingTab
            {
                TabID = SalvageTabID,
                DisplayName = "Salvage and Recycling",
                ParentTabPath = PathToSalvageTab,
                SpriteItemID = TechType.Trashcans
            };

            var tabList = new CustomCraftingTabList
            {
                salvageTab
            };

            WriteFile(tabList, "EquipmentSalvage_Tabs.txt");

            // Move the Metal Salvage into the new tab
            var moveMetalSalvage = new MovedRecipe
            {
                ItemID = TechType.Titanium.ToString(),
                OldPath = PathHelper.Fabricator.Resources.BasicMaterials.BasicMaterialsTab.GetCraftingPath.ToString(),
                NewPath = tabList[0].FullPath,
                Hidden = false,
                Copied = true
            };

            var movedList = new MovedRecipeList
            {
                moveMetalSalvage
            };

            WriteFile(movedList, "EquipmentSalvage_Moves.txt");

            // RECIPES
            var leadSalvage = new AliasRecipe
            {
                ItemID = "LeadSalvage",
                DisplayName = "Salvage Lead",
                Tooltip = "Recover the useful lead from a radiation suit no longer in use",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.BasicMaterials,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.Lead,
                Ingredients =
                {
                    new EmIngredient(TechType.RadiationSuit),
                    new EmIngredient(TechType.RadiationHelmet),
                    new EmIngredient(TechType.RadiationGloves)
                },
                LinkedItemIDs =
                {
                    TechType.Lead.ToString(),
                    TechType.Lead.ToString()
                },
                UnlockedBy = { TechType.RadiationSuit.ToString() }
            };

            var copperSalvage = new AliasRecipe
            {
                ItemID = "CopperSalvage",
                DisplayName = "Salvage Copper",
                Tooltip = "Recover the precious copper from unneeded power cells",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.BasicMaterials,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.Copper,
                Ingredients =
                {
                    new EmIngredient(TechType.PowerCell)
                },
                LinkedItemIDs =
                {
                    TechType.Copper.ToString(),
                    TechType.Copper.ToString()
                },
                UnlockedBy = { TechType.PowerCell.ToString() }
            };

            var deepSalvage = new AliasRecipe
            {
                ItemID = "DeepSalvage",
                DisplayName = "Salvage Precious Metals",
                Tooltip = "Recover the lithium and magnetite from unneeded deep power cells",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.Magnetite,
                Ingredients =
                {
                    new EmIngredient("DeepPowerCell")
                },
                LinkedItemIDs =
                {
                    TechType.Lithium.ToString(),
                    TechType.Magnetite.ToString(),
                    TechType.Lithium.ToString(),
                    TechType.Magnetite.ToString()
                },
                UnlockedBy = { "DeepPowerCell" }
            };

            var ionSalvage = new AliasRecipe
            {
                ItemID = "IonCubeSalvage",
                DisplayName = "Salvage Ion Cubes",
                Tooltip = "Recover the precious ion cubes from unneeded ion power cells",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.PrecursorIonCrystal,
                Ingredients =
                {
                    new EmIngredient(TechType.PrecursorIonPowerCell)
                },
                LinkedItemIDs =
                {
                    TechType.PrecursorIonCrystal.ToString(),
                    TechType.PrecursorIonCrystal.ToString()
                },
                UnlockedBy = { TechType.PrecursorIonPowerCell.ToString() }
            };

            var diamondSalvage = new AliasRecipe
            {
                ItemID = "DiamondSalvage",
                DisplayName = "Salvage Diamonds",
                Tooltip = "Recover diamonds from retired laser cutters. Don't forget to remove the batteries first.",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.Diamond,
                Ingredients =
                {
                    new EmIngredient(TechType.LaserCutter)
                },
                LinkedItemIDs =
                {
                    TechType.Diamond.ToString(),
                    TechType.Diamond.ToString()
                },
                UnlockedBy = { TechType.Diamond.ToString() }
            };

            var wireSalvage = new AliasRecipe
            {
                ItemID = "WireSalvage",
                DisplayName = "Salvage Copper Wire",
                Tooltip = "Recover copper wire from retired seaglide. Don't forget to remove the batteries first.",
                Path = tabList[0].FullPath,
                ForceUnlockAtStart = !EnableUnlocking,
                PdaCategory = TechCategory.Electronics,
                PdaGroup = TechGroup.Resources,
                SpriteItemID = TechType.CopperWire,
                Ingredients =
                {
                    new EmIngredient(TechType.Seaglide)
                },
                LinkedItemIDs =
                {
                    TechType.CopperWire.ToString(),
                },
                UnlockedBy = { TechType.Seaglide.ToString() }
            };

            var aliasList = new AliasRecipeList
            {
                leadSalvage, copperSalvage, deepSalvage, ionSalvage, diamondSalvage, wireSalvage
            };

            WriteFile(aliasList, "EquipmentSalvage_Recipes.txt");
        }
    }
}


