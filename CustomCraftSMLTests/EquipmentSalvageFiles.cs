namespace CustomCraftSMLTests
{
    using System.Collections.Generic;
    using System.IO;
    using Common.EasyMarkup;
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
        private const string SalvageTabID = "RecycleBatTab";
        private const string PathToSalvageTab = "Fabricator/Resources/";
        private const string SalvageCraftingTab = PathToSalvageTab + SalvageTabID;

        private static readonly string[] TopLines = new[]
        {
            "Equipment Salvage",
            "Created for Custom Craft 2",
            "Author: PrimeSonic",
            "Text files included in this mod:",
            "     EquipmentSalvage_Tabs.txt",
            "     EquipmentSalvage_Recipes.txt",
            "     EquipmentSalvage_Unlocks.txt",
        };

        private static readonly string[] BottomLines = new[]
        {
            "Published on Nexus",
            "    https://www.nexusmods.com/subnautica/mods/188",
            "Source code to all my Subnautica mods available on GitHub",
            "    https://github.com/PrimeSonic/PrimeSonicSubnauticaMods",
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

        [Test]
        public void Generate_Tabs()
        {
            var tabList = new CustomCraftingTabList
            {
                new CustomCraftingTab
                {
                    TabID = SalvageTabID,
                    DisplayName = "Recycling and Salvage",
                    SpriteItemID = TechType.Trashcans,
                    ParentTabPath = PathToSalvageTab
                }
            };

            WriteFile(tabList, "EquipmentSalvage_Tabs.txt");
        }

        [Test]
        public void Generate_Recipes()
        {
            var leadSalvage = new AliasRecipe
            {
                ItemID = "LeadSalvage",
                DisplayName = "Salavage Lead",
                Tooltip = "Recover the useful lead from a radiation suit no longer in use",
                Path = SalvageCraftingTab,
                //ForceUnlockAtStart = false,
                PdaCategory = TechCategory.BasicMaterials,
                PdaGroup = TechGroup.Resources,
            };
            leadSalvage.AddLinkedItem(TechType.Lead);
            leadSalvage.AddLinkedItem(TechType.Lead);
            leadSalvage.AddIngredient(TechType.RadiationSuit);
            leadSalvage.AddIngredient(TechType.RadiationHelmet);
            leadSalvage.AddIngredient(TechType.RadiationGloves);

            var copperSalvage = new AliasRecipe
            {
                ItemID = "CopperSalvage",
                DisplayName = "Salavage Copper",
                Tooltip = "Recover the precious copper from unneeded power cells",
                Path = SalvageCraftingTab,
                //ForceUnlockAtStart = false,
                PdaCategory = TechCategory.BasicMaterials,
                PdaGroup = TechGroup.Resources,
            };
            copperSalvage.AddLinkedItem(TechType.Lead);
            copperSalvage.AddLinkedItem(TechType.Lead);
            copperSalvage.AddIngredient(TechType.PowerCell);

            var deepSalvage = new AliasRecipe
            {
                ItemID = "DeepSalvage",
                DisplayName = "Salavage Precious Metals",
                Tooltip = "Recover the lithium and magnetite from unneeded deep power cells",
                Path = SalvageCraftingTab,
                //ForceUnlockAtStart = false,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
            };
            deepSalvage.AddLinkedItem(TechType.Lithium);
            deepSalvage.AddLinkedItem(TechType.Magnetite);
            deepSalvage.AddLinkedItem(TechType.Lithium);
            deepSalvage.AddLinkedItem(TechType.Magnetite);
            deepSalvage.AddIngredient("DeepPowerCell");

            var ionSalvage = new AliasRecipe
            {
                ItemID = "IonCubeSalvage",
                DisplayName = "Salavage Ion Cubes",
                Tooltip = "Recover the precious ion cubes from unneeded ion power cells",
                Path = SalvageCraftingTab,
                ForceUnlockAtStart = false,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
            };
            ionSalvage.AddLinkedItem(TechType.PrecursorIonCrystal);
            ionSalvage.AddLinkedItem(TechType.PrecursorIonCrystal);
            ionSalvage.AddIngredient(TechType.PrecursorIonPowerCell);

            var diamondSalvage = new AliasRecipe
            {
                ItemID = "DiamondSalvage",
                DisplayName = "Salavage Diamonds",
                Tooltip = "Recover diamonds from retired laser cutters. Don't forget to remove the batteries first.",
                Path = SalvageCraftingTab,
                //ForceUnlockAtStart = false,
                PdaCategory = TechCategory.AdvancedMaterials,
                PdaGroup = TechGroup.Resources,
            };
            diamondSalvage.AddLinkedItem(TechType.Diamond);
            diamondSalvage.AddLinkedItem(TechType.Diamond);
            diamondSalvage.AddIngredient(TechType.LaserCutter);

            var aliasList = new AliasRecipeList
            {
                leadSalvage, copperSalvage, deepSalvage, ionSalvage, diamondSalvage
            };

            WriteFile(aliasList, "EquipmentSalvage_Recipes.txt");
        }

        [Test]
        public void Generate_Unlocks()
        {
            var laserCutter = new ModifiedRecipe { ItemID = TechType.LaserCutter.ToString(), AmountCrafted = null };
            laserCutter.AddUnlock("DiamondSalvage");

            var ionPowerCell = new ModifiedRecipe { ItemID = TechType.PrecursorIonPowerCell.ToString(), AmountCrafted = null };
            ionPowerCell.AddUnlock("IonCubeSalvage");

            //var deepPowerCell = new ModifiedRecipe { ItemID = "DeepPowerCell", AmountCrafted = null };
            //deepPowerCell.AddUnlock("DeepSalvage");

            var powerCell = new ModifiedRecipe { ItemID = TechType.PowerCell.ToString(), AmountCrafted = null };
            powerCell.AddUnlock("IonCubeSalvage");

            var radSuit = new ModifiedRecipe { ItemID = TechType.RadiationSuit.ToString(), AmountCrafted = null };
            radSuit.AddUnlock("LeadSalvage");

            var modList = new ModifiedRecipeList
            {
                radSuit, powerCell, ionPowerCell, laserCutter
            };

            WriteFile(modList, "EquipmentSalvage_Unlocks.txt");
        }
    }
}


