namespace CustomCraftSMLTests
{
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

            Assert.AreEqual(TechType.Aerogel.ToString().ToLower(), recipe.ItemID.ToLower());
            Assert.AreEqual(5, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.IngredientsCount);

            EmIngredient item0 = recipe.GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString().ToLower(), item0.ItemID.ToLower());
            Assert.AreEqual(2, item0.amount);

            EmIngredient item1 = recipe.GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString().ToLower(), item1.ItemID.ToLower());
            Assert.AreEqual(3, item1.amount);

            Assert.AreEqual(2, recipe.LinkedItemsCount);

            Assert.AreEqual(TechType.Silver.ToString().ToLower(), recipe.GetLinkedItem(0).ToLower());
            Assert.AreEqual(TechType.Gold.ToString().ToLower(), recipe.GetLinkedItem(1).ToLower());

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


            var recipe = new CustomCraft2SML.Serialization.AddedRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe[0].ItemID);
            Assert.AreEqual(1, recipe[0].AmountCrafted);

            Assert.AreEqual(2, recipe[0].IngredientsCount);

            EmIngredient item0 = recipe[0].GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(2, item0.Required);

            EmIngredient item1 = recipe[0].GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(3, item1.Required);

            Assert.AreEqual(2, recipe[0].LinkedItemsCount);


            Assert.AreEqual(TechType.Silver.ToString(), recipe[0].GetLinkedItem(0));
            Assert.AreEqual(TechType.Gold.ToString(), recipe[0].GetLinkedItem(1));

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

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.IngredientsCount);

            var item0 = recipe.GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(2, item0.Required);

            var item1 = recipe.GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(3, item1.Required);

            Assert.AreEqual(false, recipe.LinkedItemsCount.HasValue);

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

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe.ItemID);
            Assert.AreEqual(false, recipe.AmountCrafted.HasValue);

            Assert.AreEqual(2, recipe.IngredientsCount);

            var item0 = recipe.GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(1, item0.Required);

            var item1 = recipe.GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(1, item1.Required);

            Assert.AreEqual(false, recipe.LinkedItemsCount.HasValue);
            Assert.AreEqual(true, recipe.ForceUnlockAtStart);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }
        
        [Test]
        public void Deserialize_AddedRecipesList_NoAmounts_Defaults()
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


            var recipe = new CustomCraft2SML.Serialization.AddedRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel.ToString(), recipe[0].ItemID);
            Assert.AreEqual(false, recipe[0].AmountCrafted.HasValue);

            Assert.AreEqual(2, recipe[0].IngredientsCount);

            var item0 = recipe[0].GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(1, item0.Required);

            var item1 = recipe[0].GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(1, item1.Required);

            Assert.AreEqual(false, recipe[0].LinkedItemsCount.HasValue);
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

            var recipeList = new CustomCraft2SML.Serialization.AddedRecipeList();

            bool success = recipeList.FromString(serialized);

            Assert.IsTrue(success);
            Assert.AreEqual(3, recipeList.Count);
        }
    }
}
