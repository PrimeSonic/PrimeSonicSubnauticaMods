namespace CustomCraftSMLTests
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class AddedRecipeTests
    {
        [TestCase("Aerogel", "Titanium", "Silver,Gold")]
        [TestCase("aerogel", "titanium", "silver,gold")]
        public void Deserialize_AddedRecipe_FullDetails(string itemName, string ingredient, string linkedItems)
        {
            string serialized = "AddedRecipe:" + "\r\n" +
                                      "(" + "\r\n" +
                                     $"    ItemID:{itemName};" + "\r\n" +
                                      "    AmountCrafted:5;" + "\r\n" +
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
                                      "    Path:Fabricator/Resources/BasicMaterials;" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new AddedRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(5, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.SmlIngredients[0].techType);
            Assert.AreEqual(2, recipe.SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe.SmlIngredients[1].techType);
            Assert.AreEqual(3, recipe.SmlIngredients[1].amount);

            Assert.AreEqual(2, recipe.LinkedItems.Count);
            Assert.AreEqual(TechType.Silver, recipe.LinkedItems[0]);
            Assert.AreEqual(TechType.Gold, recipe.LinkedItems[1]);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }

        [Test]
        public void Deserialize_AddedRecipesList_FullDetails()
        {
            const string serialized = "AddedRecipes:" + "\r\n" +
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
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
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
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      ");" + "\r\n";


            var recipe = new AddedRecipeList();

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

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe[0].Path);
        }

        [Test]
        public void Deserialize_AddedRecipe_NoLinkedItems()
        {
            const string serialized = "AddedRecipe:" + "\r\n" +
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
                                      "    Path:Fabricator/Resources/BasicMaterials;" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new AddedRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.SmlIngredients[0].techType);
            Assert.AreEqual(2, recipe.SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe.SmlIngredients[1].techType);
            Assert.AreEqual(3, recipe.SmlIngredients[1].amount);

            Assert.AreEqual(0, recipe.LinkedItems.Count);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }

        [Test]
        public void Deserialize_AddedRecipe_NoAmounts_DefaultTo1()
        {
            const string serialized = "AddedRecipe:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new AddedRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.SmlIngredients[0].techType);
            Assert.AreEqual(1, recipe.SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe.SmlIngredients[1].techType);
            Assert.AreEqual(1, recipe.SmlIngredients[1].amount);

            Assert.AreEqual(0, recipe.LinkedItems.Count);
            Assert.AreEqual(true, recipe.ForceUnlockAtStart);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }
        
        [Test]
        public void Deserialize_AddedRecipesList_NoAmounts_DefaultTo1()
        {
            const string serialized = "AddedRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      "    ForceUnlockAtStart: NO;" + 
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
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      ");" + "\r\n";


            var recipe = new AddedRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe[0].ItemID);
            Assert.AreEqual(1, recipe[0].AmountCrafted);

            Assert.AreEqual(2, recipe[0].SmlIngredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe[0].SmlIngredients[0].techType);
            Assert.AreEqual(1, recipe[0].SmlIngredients[0].amount);
            Assert.AreEqual(TechType.Copper, recipe[0].SmlIngredients[1].techType);
            Assert.AreEqual(1, recipe[0].SmlIngredients[1].amount);

            Assert.AreEqual(0, recipe[0].LinkedItems.Count);
            Assert.AreEqual(false, recipe[0].ForceUnlockAtStart);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe[0].Path);
        }

        [Test]
        public void Deserialize_AddedRecipesList_AllLowerCaseIDs()
        {
            const string serialized = "AddedRecipes:                                            " +
                                        "(                                                        " +
                                        "    ItemID:stalkertooth;                                 " +
                                        "    AmountCrafted: 1;                                    " +
                                        "    Ingredients:                                         " +
                                        "        ( ItemID:quartz; Required:1; );                  " +
                                        "    Path:Fabricator/Resources/BasicMaterials;            " +
                                        "),                                                       " +
                                        "(                                                        " +
                                        "    ItemID:coralchunk;                                   " +
                                        "    AmountCrafted: 2;                                    " +
                                        "    Ingredients:                                         " +
                                        "        ( ItemID:crashpowder; Required:2; );             " +
                                        "    Path:Fabricator/Resources/BasicMaterials;            " +
                                        "),                                                       " +
                                        "(                                                        " +
                                        "    ItemID:nutrientblock;                                " +
                                        "    AmountCrafted: 2;                                    " +
                                        "    Ingredients:                                         " +
                                        "        ( ItemID:salt; Required:2; ),                    " +
                                        "        ( ItemID:purplerattlespore; Required:1; ),       " +
                                        "        ( ItemID:purplevaseplantseed; Required:1; ),     " +
                                        "        ( ItemID:orangepetalsplantseed; Required:1; ),   " +
                                        "        ( ItemID:orangemushroomspore; Required:1; ),     " +
                                        "        ( ItemID:pinkmushroomspore; Required:1; );       " +
                                        "    Path:Fabricator/Survival/CuredFood;                  " +
                                        ");                                                       ";

            var recipeList = new AddedRecipeList();

            bool success = recipeList.FromString(serialized);

            Assert.IsTrue(success);
            Assert.AreEqual(3, recipeList.Count);
        }
    }
}
