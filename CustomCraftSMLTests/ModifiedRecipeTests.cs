namespace CustomCraftSMLTests
{
    using System;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class ModifiedRecipeTests
    {
        [TestCase("Aerogel", "Titanium", "Silver,Gold")]
        [TestCase("aerogel", "titanium", "silver,gold")]
        public void Deserialize_ModifiedRecipe_FullDetails(string itemName, string ingredient, string linkedItems)
        {
            string serialized = "ModifiedRecipe:" + "\r\n" +
                                      "(" + "\r\n" +
                                     $"    ItemID:{itemName};" + "\r\n" +
                                      "    AmountCrafted:4;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                     $"            ItemID:{ingredient};" + "\r\n" +
                                      "            Required:2;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "            Required:3;" + "\r\n" +
                                      "        );" + "\r\n" +
                                     $"    LinkedItemIDs:{linkedItems};" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new ModifiedRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(4, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.SmlIngredients[0].techType);
            Assert.AreEqual(2, recipe.SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe.SmlIngredients[1].techType);
            Assert.AreEqual(3, recipe.SmlIngredients[1].amount);

            Assert.AreEqual(2, recipe.LinkedItems.Count);
            Assert.AreEqual(TechType.Silver, recipe.LinkedItems[0]);
            Assert.AreEqual(TechType.Gold, recipe.LinkedItems[1]);
        }

        [Test]
        public void Deserialize_ModifiedRecipesList_FullDetails()
        {
            const string serialized = "ModifiedRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    AmountCrafted:1;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "            Required:2;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "            Required:3;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    AmountCrafted:1;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "            Required:2;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "            Required:3;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      ");" + "\r\n";


            var recipe = new ModifiedRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe[0].ItemID);
            Assert.AreEqual(1, recipe[0].AmountCrafted);

            Assert.AreEqual(2, recipe[0].SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe[0].SmlIngredients[0].techType);
            Assert.AreEqual(2, recipe[0].SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe[0].SmlIngredients[1].techType);
            Assert.AreEqual(3, recipe[0].SmlIngredients[1].amount);

            Assert.AreEqual(2, recipe[0].LinkedItems.Count);
            Assert.AreEqual(TechType.Silver, recipe[0].LinkedItems[0]);
            Assert.AreEqual(TechType.Gold, recipe[0].LinkedItems[1]);
        }

        [Test]
        public void Deserialize_ModifiedRecipesList_NoAmounts_DefaultTo1()
        {
            const string serialized = "ModifiedRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      ");" + "\r\n";


            var recipe = new ModifiedRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe[0].ItemID);
            Assert.AreEqual(1, recipe[0].AmountCrafted);

            Assert.AreEqual(2, recipe[0].SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe[0].SmlIngredients[0].techType);
            Assert.AreEqual(1, recipe[0].SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe[0].SmlIngredients[1].techType);
            Assert.AreEqual(1, recipe[0].SmlIngredients[1].amount);

            Assert.AreEqual(0, recipe[0].LinkedItems.Count);
            Assert.AreEqual(0, recipe[0].Unlocks.Count);
            Assert.AreEqual(false, recipe[0].ForceUnlockAtStart);
        }

        [Test]
        public void Deserialize_Other()
        {
            const string sample = "ModdifiedRecipes: " +
                    "(ItemID:FiberMesh; AmountCrafted:1; Ingredients:(ItemID:CreepvinePiece; Required:1; ); )," +
                    "(ItemID:Silicone; AmountCrafted:1; Ingredients:(ItemID:CreepvineSeedCluster; Required:1; ); )," +
                    "(ItemID:PrecursorIonBattery; AmountCrafted:1; Ingredients:(ItemID:PrecursorIonCrystal; Required:1; ), (ItemID:Gold; Required:1; ), (ItemID:Silver; Required:1; ), (ItemID:Battery; Required:1; ); )," +
                    "(ItemID:PrecursorIonPowerCell; AmountCrafted:1;Ingredients:(ItemID:PrecursorIonBattery; Required:2; ), (ItemID:Silicone; Required:1; ), (ItemID:PowerCell; Required:1; ); )," +
                    "(ItemID:FilteredWater; AmountCrafted:2; Ingredients:(ItemID:Bladderfish; Required:1; ); )," +
                    "(ItemID:FireExtinguisher; AmountCrafted:1; Ingredients:(ItemID:Titanium; Required:1; ); )," +
                    "(ItemID:PrecursorKey_Purple; AmountCrafted:1; Ingredients:(ItemID:PrecursorIonCrystal; Required:1; ), (ItemID:Diamond; Required:1; ); ); ";

            var recipes = new ModifiedRecipeList();

            bool success = recipes.FromString(sample);

            Assert.IsTrue(success);

            Console.WriteLine(recipes.PrintyPrint());
        }

        [Test]
        public void Deserialize_ModifiedRecipesList_Unlocks()
        {
            const string serialized = "ModifiedRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    AmountCrafted:1;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "            Required:2;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "            Required:3;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    ForceUnlockAtStart:NO;" + "\r\n" +
                                      "    Unlocks:ComputerChip,Cyclops;" + "\r\n" +
                                      ");" + "\r\n";


            var recipeList = new ModifiedRecipeList();

            recipeList.FromString(serialized);
            ModifiedRecipe recipe = recipeList[0];


            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.SmlIngredients[0].techType);
            Assert.AreEqual(2, recipe.SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe.SmlIngredients[1].techType);
            Assert.AreEqual(3, recipe.SmlIngredients[1].amount);

            Assert.AreEqual(2, recipe.LinkedItems.Count);
            Assert.AreEqual(TechType.Silver, recipe.LinkedItems[0]);
            Assert.AreEqual(TechType.Gold, recipe.LinkedItems[1]);

            Assert.AreEqual(false, recipe.ForceUnlockAtStart);

            Assert.AreEqual(2, recipe.Unlocks.Count);
            Assert.AreEqual(TechType.ComputerChip, recipe.Unlocks[0]);
            Assert.AreEqual(TechType.Cyclops, recipe.Unlocks[1]);
        }
    }
}
