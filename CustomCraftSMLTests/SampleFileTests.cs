namespace CustomCraftSMLTests
{
    using System;
    using System.IO;
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML;

    [TestFixture]
    internal class SampleFileTests
    {
        private static string SampleFileDirectory
        {
            get
            {
                var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                path = Directory.GetParent(path).FullName;
                path = Directory.GetParent(path).FullName;
                return Directory.GetParent(path).FullName + "/CustomCraftSML/SampleFiles/";
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
        }

        [Test]
        public void Sample_ModifiedRecipes2_Ok()
        {
            var mRecipes = new ModifiedRecipeList();

            string sample = File.ReadAllText(SampleFileDirectory + "Mattus666.txt");

            bool result = mRecipes.FromString(sample);
            Assert.IsTrue(result);
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
        public void CreateFromClases_ThenTest_SampleAddRecipes()
        {
            var nutrientBlockRecipe = new AddedRecipe()
            {
                ItemID = TechType.NutrientBlock,
                AmountCrafted = 1,
                Path = PathHelper.Fabricator.Sustenance.CuredFood.GetThisNode.ToString()
            };
            nutrientBlockRecipe.AddIngredient(TechType.CuredReginald, 1);
            nutrientBlockRecipe.AddIngredient(TechType.PurpleVegetable, 1);
            nutrientBlockRecipe.AddIngredient(TechType.HangingFruit, 1);

            var bigFilterWaterRecipe = new AddedRecipe()
            {
                ItemID = TechType.BigFilteredWater,
                AmountCrafted = 1,
                Path = PathHelper.Fabricator.Sustenance.Water.GetThisNode.ToString()
            };
            bigFilterWaterRecipe.AddIngredient(TechType.FilteredWater, 2);

            var origRecipeList = new AddedRecipeList();
            origRecipeList.Collections.Add(nutrientBlockRecipe);
            origRecipeList.Collections.Add(bigFilterWaterRecipe);

            string serialized = origRecipeList.PrintyPrint();

            string samples2File = SampleFileDirectory + "AddedRecipes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingRecipeList = new AddedRecipeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingRecipeList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origRecipeList.Count, readingRecipeList.Count);

            // nutrientBlockRecipe
            Assert.AreEqual(origRecipeList[0].ItemID, nutrientBlockRecipe.ItemID);
            Assert.AreEqual(origRecipeList[0].AmountCrafted, nutrientBlockRecipe.AmountCrafted);
            Assert.AreEqual(origRecipeList[0].Path, nutrientBlockRecipe.Path);
            Assert.AreEqual(origRecipeList[0].SmlIngredients.Count, nutrientBlockRecipe.SmlIngredients.Count);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[0].techType, nutrientBlockRecipe.SmlIngredients[0].techType);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[1].techType, nutrientBlockRecipe.SmlIngredients[1].techType);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[2].techType, nutrientBlockRecipe.SmlIngredients[2].techType);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[0].amount, nutrientBlockRecipe.SmlIngredients[0].amount);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[1].amount, nutrientBlockRecipe.SmlIngredients[1].amount);
            Assert.AreEqual(origRecipeList[0].SmlIngredients[2].amount, nutrientBlockRecipe.SmlIngredients[2].amount);

            // bigFilterWaterRecipe
            Assert.AreEqual(origRecipeList[1].ItemID, bigFilterWaterRecipe.ItemID);
            Assert.AreEqual(origRecipeList[1].AmountCrafted, bigFilterWaterRecipe.AmountCrafted);
            Assert.AreEqual(origRecipeList[1].Path, bigFilterWaterRecipe.Path);
            Assert.AreEqual(origRecipeList[1].SmlIngredients.Count, bigFilterWaterRecipe.SmlIngredients.Count);
            Assert.AreEqual(origRecipeList[1].SmlIngredients[0].techType, bigFilterWaterRecipe.SmlIngredients[0].techType);
            Assert.AreEqual(origRecipeList[1].SmlIngredients[0].amount, bigFilterWaterRecipe.SmlIngredients[0].amount);
        }

        [Test]
        public void CreateFromClases_ThenTest_SampleCustomSizes()
        {
            var smallBloodOil = new CustomSize()
            {
                ItemID = TechType.BloodOil,
                Width = 1,
                Height = 1
            };

            var smallMelon = new CustomSize()
            {
                ItemID = TechType.BulboTreePiece,
                Width = 1,
                Height = 1
            };

            var smallPotatoe = new CustomSize()
            {
                ItemID = TechType.PurpleVegetable,
                Width = 1,
                Height = 1
            };

            var origCustSizes = new CustomSizeList();
            origCustSizes.Collections.Add(smallBloodOil);
            origCustSizes.Collections.Add(smallMelon);
            origCustSizes.Collections.Add(smallPotatoe);

            string serialized = origCustSizes.PrintyPrint();

            string samples2File = SampleFileDirectory + "CustomSizes_Samples2.txt";

            File.WriteAllText(samples2File, serialized);

            var readingSizesList = new CustomSizeList();

            string reserialized = File.ReadAllText(samples2File);

            bool success = readingSizesList.FromString(reserialized);

            Assert.IsTrue(success);

            Assert.AreEqual(origCustSizes.Count, readingSizesList.Count);

            Assert.AreEqual(TechType.BloodOil, readingSizesList[0].ItemID);
            Assert.AreEqual(TechType.BulboTreePiece, readingSizesList[1].ItemID);
            Assert.AreEqual(TechType.PurpleVegetable, readingSizesList[2].ItemID);

            Assert.AreEqual(1, readingSizesList[0].Width);
            Assert.AreEqual(1, readingSizesList[0].Height);
            Assert.AreEqual(1, readingSizesList[1].Width);
            Assert.AreEqual(1, readingSizesList[1].Height);
            Assert.AreEqual(1, readingSizesList[2].Width);
            Assert.AreEqual(1, readingSizesList[2].Height);
        }
    }
}
