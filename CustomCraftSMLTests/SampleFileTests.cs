namespace CustomCraftSMLTests
{
    using System.IO;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    internal class SampleFileTests
    {
        private static string SampleFileDirectory
        {
            get
            {
                string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                return Directory.GetParent(path).FullName + "/CustomCraft2SML/SampleFiles/";
            }
        }

        [Test]
        public void Sample_CustomSizes_Ok()
        {
            var cSizes = new CustomSizeList();

            string sample = File.ReadAllText(SampleFileDirectory + "CustomSizes_Samples.txt");

            bool result = cSizes.FromString(sample);
            Assert.IsTrue(result);
        }

        [Test]
        public void Sample_ModifiedRecipes_Ok()
        {
            var mRecipes = new ModifiedRecipeList();

            string sample = File.ReadAllText(SampleFileDirectory + "ModifiedRecipes_Samples.txt");

            bool result = mRecipes.FromString(sample);
            Assert.IsTrue(result);

            ModifiedRecipe reactorRodChange = mRecipes[2];
            Assert.AreEqual(TechType.ReactorRod.ToString(), reactorRodChange.ItemID);
            Assert.AreEqual(false, reactorRodChange.Ingredients.Count > 0);
            Assert.AreEqual(false, reactorRodChange.LinkedItemIDs.Count > 0);

            Assert.AreEqual(1, reactorRodChange.Unlocks.Count);
            Assert.AreEqual(TechType.DepletedReactorRod.ToString(), reactorRodChange.Unlocks[0]);
        }

        [Test]
        public void Sample_AddedRecipes_Ok()
        {
            var aRecipes = new AddedRecipeList();

            string sample = File.ReadAllText(SampleFileDirectory + "AddedRecipes_Samples.txt");

            bool result = aRecipes.FromString(sample);
            Assert.IsTrue(result);
        }

        [Test]
        public void Sample_BioFuels_Ok()
        {
            var cFuels = new CustomBioFuelList();

            string sample = File.ReadAllText(SampleFileDirectory + "CustomBioFuels_Samples.txt");

            bool result = cFuels.FromString(sample);
            Assert.IsTrue(result);
        }

        [Test]
        public void Sample_CustomTabs_Ok()
        {
            var cTabs = new CustomCraftingTabList();

            string sample = File.ReadAllText(SampleFileDirectory + "CustomTab_Samples.txt");

            bool result = cTabs.FromString(sample);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateFromClases_ThenTest_SampleAddRecipes()
        {
            var nutrientBlockRecipe = new AddedRecipe()
            {
                ItemID = TechType.NutrientBlock.ToString(),
                AmountCrafted = 1,
                ForceUnlockAtStart = false,
                Path = "Fabricator/Survival/CuredFood",
                Ingredients =
                {
                    new EmIngredient(TechType.CuredReginald, 1),
                    new EmIngredient(TechType.PurpleVegetable, 1),
                    new EmIngredient(TechType.HangingFruit, 1),
                }
            };

            var bigFilterWaterRecipe = new AddedRecipe()
            {
                ItemID = TechType.BigFilteredWater.ToString(),
                AmountCrafted = 1,
                Path = "Fabricator/Survival/Water",
                ForceUnlockAtStart = false,
                Ingredients =
                {
                    new EmIngredient(TechType.FilteredWater, 2)
                }
            };

            var origRecipeList = new AddedRecipeList
            {
                nutrientBlockRecipe,
                bigFilterWaterRecipe
            };

            string serialized = origRecipeList.PrettyPrint();

            string samples2File = SampleFileDirectory + "AddedRecipes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingRecipeList = new AddedRecipeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingRecipeList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origRecipeList.Count, readingRecipeList.Count);

            // nutrientBlockRecipe
            AddedRecipe nutrientBlock = origRecipeList[0];
            Assert.AreEqual(nutrientBlock.ItemID, nutrientBlockRecipe.ItemID);
            Assert.AreEqual(nutrientBlock.AmountCrafted, nutrientBlockRecipe.AmountCrafted);
            Assert.AreEqual(nutrientBlock.Path, nutrientBlockRecipe.Path);
            Assert.AreEqual(nutrientBlock.Ingredients.Count, nutrientBlockRecipe.Ingredients.Count);
            Assert.AreEqual(nutrientBlock.Ingredients[0].ItemID, nutrientBlockRecipe.Ingredients[0].ItemID);
            Assert.AreEqual(nutrientBlock.Ingredients[1].ItemID, nutrientBlockRecipe.Ingredients[1].ItemID);
            Assert.AreEqual(nutrientBlock.Ingredients[2].ItemID, nutrientBlockRecipe.Ingredients[2].ItemID);
            Assert.AreEqual(nutrientBlock.Ingredients[0].Required, nutrientBlockRecipe.Ingredients[0].Required);
            Assert.AreEqual(nutrientBlock.Ingredients[1].Required, nutrientBlockRecipe.Ingredients[1].Required);
            Assert.AreEqual(nutrientBlock.Ingredients[2].Required, nutrientBlockRecipe.Ingredients[2].Required);
            Assert.AreEqual(nutrientBlock.Unlocks.Count, nutrientBlockRecipe.Unlocks.Count);
            Assert.AreEqual(nutrientBlock.ForceUnlockAtStart, nutrientBlockRecipe.ForceUnlockAtStart);
            Assert.AreEqual("Fabricator/Survival/CuredFood", nutrientBlockRecipe.Path);

            // bigFilterWaterRecipe
            AddedRecipe bigFilteredWater = origRecipeList[1];
            Assert.AreEqual(bigFilteredWater.ItemID, bigFilterWaterRecipe.ItemID);
            Assert.AreEqual(bigFilteredWater.AmountCrafted, bigFilterWaterRecipe.AmountCrafted);
            Assert.AreEqual(bigFilteredWater.Path, bigFilterWaterRecipe.Path);
            Assert.AreEqual(bigFilteredWater.Ingredients.Count, bigFilterWaterRecipe.Ingredients.Count);
            Assert.AreEqual(bigFilteredWater.Ingredients[0].ItemID, bigFilterWaterRecipe.Ingredients[0].ItemID);
            Assert.AreEqual(bigFilteredWater.Ingredients[0].Required, bigFilterWaterRecipe.Ingredients[0].Required);
            Assert.AreEqual(bigFilteredWater.Unlocks.Count, bigFilterWaterRecipe.Unlocks.Count);
            Assert.AreEqual(bigFilteredWater.ForceUnlockAtStart, bigFilterWaterRecipe.ForceUnlockAtStart);
        }

        [Test]
        public void CreateFromClases_ThenTest_SampleModifiedRecipes()
        {
            var curedReginaldRecipe = new ModifiedRecipe()
            {
                ItemID = TechType.CuredReginald.ToString(),
                AmountCrafted = 1,
                Ingredients =
                {
                    new EmIngredient(TechType.Reginald.ToString(), 1),
                    new EmIngredient(TechType.Salt.ToString(), 1)
                },
                Unlocks =
                {
                    TechType.NutrientBlock.ToString()
                }
            };

            var origRecipeList = new ModifiedRecipeList
            {
                curedReginaldRecipe
            };

            string serialized = origRecipeList.PrettyPrint();

            string samples2File = SampleFileDirectory + "ModifiedRecipes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingRecipeList = new ModifiedRecipeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingRecipeList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origRecipeList.Count, readingRecipeList.Count);

            // nutrientBlockRecipe
            ModifiedRecipe curedReginald = origRecipeList[0];
            Assert.AreEqual(curedReginald.ItemID, curedReginaldRecipe.ItemID);
            Assert.AreEqual(curedReginald.AmountCrafted, curedReginaldRecipe.AmountCrafted);
            Assert.AreEqual(curedReginald.Ingredients.Count, curedReginaldRecipe.Ingredients.Count);
            Assert.AreEqual(curedReginald.Ingredients[0].ItemID, curedReginaldRecipe.Ingredients[0].ItemID);
            Assert.AreEqual(curedReginald.Ingredients[1].ItemID, curedReginaldRecipe.Ingredients[1].ItemID);
            Assert.AreEqual(curedReginald.Ingredients[0].Required, curedReginaldRecipe.Ingredients[0].Required);
            Assert.AreEqual(curedReginald.Ingredients[1].Required, curedReginaldRecipe.Ingredients[1].Required);
            Assert.AreEqual(curedReginald.ForceUnlockAtStart, curedReginaldRecipe.ForceUnlockAtStart);
            Assert.AreEqual(curedReginald.Unlocks.Count, curedReginaldRecipe.Unlocks.Count);
            Assert.AreEqual(curedReginald.Unlocks[0], curedReginaldRecipe.Unlocks[0]);


        }

        [Test]
        public void CreateFromClases_ThenTest_SampleCustomSizes()
        {
            var smallBloodOil = new CustomSize()
            {
                ItemID = TechType.BloodOil.ToString(),
                Width = 1,
                Height = 1
            };

            var smallMelon = new CustomSize()
            {
                ItemID = TechType.BulboTreePiece.ToString(),
                Width = 1,
                Height = 1
            };

            var smallPotatoe = new CustomSize()
            {
                ItemID = TechType.PurpleVegetable.ToString(),
                Width = 1,
                Height = 1
            };

            var origCustSizes = new CustomSizeList
            {
                smallBloodOil,
                smallMelon,
                smallPotatoe
            };

            string serialized = origCustSizes.PrettyPrint();

            string samples2File = SampleFileDirectory + "CustomSizes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingSizesList = new CustomSizeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingSizesList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origCustSizes.Count, readingSizesList.Count);

            Assert.AreEqual(TechType.BloodOil.ToString(), readingSizesList[0].ItemID);
            Assert.AreEqual(TechType.BulboTreePiece.ToString(), readingSizesList[1].ItemID);
            Assert.AreEqual(TechType.PurpleVegetable.ToString(), readingSizesList[2].ItemID);

            Assert.AreEqual(1, readingSizesList[0].Width);
            Assert.AreEqual(1, readingSizesList[0].Height);
            Assert.AreEqual(1, readingSizesList[1].Width);
            Assert.AreEqual(1, readingSizesList[1].Height);
            Assert.AreEqual(1, readingSizesList[2].Width);
            Assert.AreEqual(1, readingSizesList[2].Height);
        }

        [Test]
        public void CreateFromClases_ThenTest_SampleCustomFragmentCount()
        {
            var seaglideFrag = new CustomFragmentCount
            {
                ItemID = TechType.Seaglide.ToString(),
                FragmentsToScan = 4
            };
            
            var beaconFrag = new CustomFragmentCount
            {
                ItemID = TechType.Beacon.ToString(),
                FragmentsToScan = 3
            };

            var seamothFrag = new CustomFragmentCount
            {
                ItemID = TechType.Seamoth.ToString(),
                FragmentsToScan = 6
            };

            var origCustomFragList = new CustomFragmentCountList
            {
                seaglideFrag, beaconFrag, seamothFrag
            };

            string serialized = origCustomFragList.PrettyPrint();

            string samples2File = SampleFileDirectory + "CustomFragments_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingSizesList = new CustomFragmentCountList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingSizesList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origCustomFragList.Count, readingSizesList.Count);

            Assert.AreEqual(TechType.Seaglide.ToString(), readingSizesList[0].ItemID);
            Assert.AreEqual(TechType.Beacon.ToString(), readingSizesList[1].ItemID);
            Assert.AreEqual(TechType.Seamoth.ToString(), readingSizesList[2].ItemID);

            Assert.AreEqual(4, readingSizesList[0].FragmentsToScan);
            Assert.AreEqual(3, readingSizesList[1].FragmentsToScan);
            Assert.AreEqual(6, readingSizesList[2].FragmentsToScan);
        }

        [Test]
        public void CreateFromClases_ThenTest_SampleMovedRecipes()
        {
            var moveTiIngot = new MovedRecipe
            {
                ItemID = TechType.TitaniumIngot.ToString(),
                OldPath = "Fabricator/Resources/BasicMaterials",
                NewPath = "Fabricator/Resources/AdvancedMaterials",
            };

            var hideRifile = new MovedRecipe
            {
                ItemID = TechType.StasisRifle.ToString(),
                OldPath = "Fabricator/Personal/Tools",
                Hidden = true
            };

            var hideFss = new MovedRecipe
            {
                ItemID = TechType.CyclopsFireSuppressionModule.ToString(),
                OldPath = "CyclopsFabricator",
                Hidden = true
            };

            var copyUltraFins = new MovedRecipe
            {
                ItemID = TechType.UltraGlideFins.ToString(),
                NewPath = "Fabricator/Personal/Tools",
                Copied = true
            };

            var origMovedRepList = new MovedRecipeList
            {
                moveTiIngot, hideRifile, hideFss, copyUltraFins
            };

            string serialized = origMovedRepList.PrettyPrint();

            string samples2File = SampleFileDirectory + "MovedRecipes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingSizesList = new MovedRecipeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingSizesList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origMovedRepList.Count, readingSizesList.Count);

            Assert.AreEqual(TechType.TitaniumIngot.ToString(), readingSizesList[0].ItemID);
            Assert.AreEqual(TechType.StasisRifle.ToString(), readingSizesList[1].ItemID);
            Assert.AreEqual(TechType.CyclopsFireSuppressionModule.ToString(), readingSizesList[2].ItemID);
            Assert.AreEqual(TechType.UltraGlideFins.ToString(), readingSizesList[3].ItemID);

            Assert.AreEqual(moveTiIngot.OldPath, readingSizesList[0].OldPath);
            Assert.AreEqual(moveTiIngot.NewPath, readingSizesList[0].NewPath);
            Assert.AreEqual(moveTiIngot.Hidden, readingSizesList[0].Hidden);
            Assert.AreEqual(moveTiIngot.Copied, readingSizesList[0].Copied);

            Assert.AreEqual(hideRifile.OldPath, readingSizesList[1].OldPath);
            Assert.AreEqual(hideRifile.NewPath, readingSizesList[1].NewPath);
            Assert.AreEqual(hideRifile.Hidden, readingSizesList[1].Hidden);
            Assert.AreEqual(hideRifile.Copied, readingSizesList[1].Copied);

            Assert.AreEqual(hideFss.OldPath, readingSizesList[2].OldPath);
            Assert.AreEqual(hideFss.NewPath, readingSizesList[2].NewPath);
            Assert.AreEqual(hideFss.Hidden, readingSizesList[2].Hidden);
            Assert.AreEqual(hideFss.Copied, readingSizesList[2].Copied);

            Assert.AreEqual(copyUltraFins.OldPath, readingSizesList[3].OldPath);
            Assert.AreEqual(copyUltraFins.NewPath, readingSizesList[3].NewPath);
            Assert.AreEqual(copyUltraFins.Hidden, readingSizesList[3].Hidden);
            Assert.AreEqual(copyUltraFins.Copied, readingSizesList[3].Copied);
        }
    }
}
