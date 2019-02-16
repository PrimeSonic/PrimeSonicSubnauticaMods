namespace CustomCraftSMLTests
{
    using System;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
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

            Assert.AreEqual(TechType.Aerogel.ToString().ToLower(), recipe.ItemID.ToLower());
            Assert.AreEqual(4, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.Ingredients.Count);
            Assert.AreEqual(TechType.Titanium.ToString().ToLower(), recipe.Ingredients[0].ItemID.ToLower());
            Assert.AreEqual(2, recipe.Ingredients[0].Required);
            Assert.AreEqual(TechType.Copper.ToString().ToLower(), recipe.Ingredients[1].ItemID.ToLower());
            Assert.AreEqual(3, recipe.Ingredients[1].Required);

            Assert.AreEqual(2, recipe.LinkedItemIDs.Count);
            Assert.AreEqual(TechType.Silver.ToString().ToLower(), recipe.LinkedItemIDs[0].ToLower());
            Assert.AreEqual(TechType.Gold.ToString().ToLower(), recipe.LinkedItemIDs[1].ToLower());
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

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe[0].ItemID);
            Assert.AreEqual(1, recipe[0].AmountCrafted);

            Assert.AreEqual(2, recipe[0].Ingredients.Count);
            Assert.AreEqual(TechType.Titanium.ToString(), recipe[0].Ingredients[0].ItemID);
            Assert.AreEqual(2, recipe[0].Ingredients[0].Required);
            Assert.AreEqual(TechType.Copper.ToString(), recipe[0].Ingredients[1].ItemID);
            Assert.AreEqual(3, recipe[0].Ingredients[1].Required);

            Assert.AreEqual(2, recipe[0].LinkedItemIDs.Count);
            Assert.AreEqual(TechType.Silver.ToString(), recipe[0].LinkedItemIDs[0]);
            Assert.AreEqual(TechType.Gold.ToString(), recipe[0].LinkedItemIDs[1]);
        }

        [Test]
        public void Deserialize_ModifiedRecipesList_NoAmounts_Defaults()
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

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe[0].ItemID);
            Assert.AreEqual(false, recipe[0].AmountCrafted.HasValue);

            Assert.AreEqual(2, recipe[0].Ingredients.Count);
            Assert.AreEqual(TechType.Titanium.ToString(), recipe[0].Ingredients[0].ItemID);
            Assert.AreEqual(1, recipe[0].Ingredients[0].Required);
            Assert.AreEqual(TechType.Copper.ToString(), recipe[0].Ingredients[1].ItemID);
            Assert.AreEqual(1, recipe[0].Ingredients[1].Required);

            Assert.AreEqual(false, recipe[0].LinkedItemIDs.Count > 0);
            Assert.AreEqual(false, recipe[0].Unlocks.Count > 0);
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

            Console.WriteLine(recipes.PrettyPrint());
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


            Assert.AreEqual(TechType.Aerogel.ToString(), recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.Ingredients.Count);
            Assert.AreEqual(TechType.Titanium.ToString(), recipe.Ingredients[0].ItemID);
            Assert.AreEqual(2, recipe.Ingredients[0].Required);
            Assert.AreEqual(TechType.Copper.ToString(), recipe.Ingredients[1].ItemID);
            Assert.AreEqual(3, recipe.Ingredients[1].Required);

            Assert.AreEqual(2, recipe.LinkedItemIDs.Count);
            Assert.AreEqual(TechType.Silver.ToString(), recipe.LinkedItemIDs[0]);
            Assert.AreEqual(TechType.Gold.ToString(), recipe.LinkedItemIDs[1]);

            Assert.AreEqual(false, recipe.ForceUnlockAtStart);

            Assert.AreEqual(2, recipe.Unlocks.Count);
            Assert.AreEqual(TechType.ComputerChip.ToString(), recipe.Unlocks[0]);
            Assert.AreEqual(TechType.Cyclops.ToString(), recipe.Unlocks[1]);
        }
    }
}
