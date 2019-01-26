namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class AliasRecipeTests
    {
        [TestCase("AliasAerogel", "Custom display name", "Custom tooltip", "Titanium", "Silver,Gold")]
        public void Deserialize_AliasRecipe_FullDetails(string itemName, string displayName, string tooltip, string ingredient, string linkedItems)
        {
            string serialized = "AliasRecipe:" + "\r\n" +
                                      "(" + "\r\n" +
                                     $"    ItemID:{itemName};" + "\r\n" +
                                     $"    DisplayName:\"{displayName}\";" + "\r\n" +
                                     $"    Tooltip:\"{tooltip}\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
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

            var recipe = new AliasRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual(itemName, recipe.ItemID);
            Assert.AreEqual(0, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.IngredientsCount);


            EmIngredient item0 = recipe.GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(2, item0.amount);

            EmIngredient item1 = recipe.GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(3, item1.amount);

            Assert.AreEqual(2, recipe.LinkedItemsCount);

            Assert.AreEqual(TechType.Silver.ToString(), recipe.GetLinkedItem(0));
            Assert.AreEqual(TechType.Gold.ToString(), recipe.GetLinkedItem(1));

            Assert.AreEqual("Custom display name", recipe.DisplayName);
            Assert.AreEqual("Custom tooltip", recipe.Tooltip);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }

        [Test]
        public void Deserialize_AliasRecipesList_FullDetails()
        {
            const string serialized = "AliasRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:AliasAerogel2;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
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
                                      "    ItemID:AliasAerogel3;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
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


            var recipe = new CustomCraft2SML.Serialization.AliasRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual("AliasAerogel2", recipe[0].ItemID);
            Assert.AreEqual("AliasAerogel3", recipe[1].ItemID);

            foreach (var r in recipe)
            {
                Assert.AreEqual(0, r.AmountCrafted);

                Assert.AreEqual(2, r.IngredientsCount);

                EmIngredient item0 = r.GetIngredient(0);
                Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
                Assert.AreEqual(2, item0.Required);

                EmIngredient item1 = r.GetIngredient(1);
                Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
                Assert.AreEqual(3, item1.Required);

                Assert.AreEqual(2, r.LinkedItemsCount);


                Assert.AreEqual(TechType.Silver.ToString(), r.GetLinkedItem(0));
                Assert.AreEqual(TechType.Gold.ToString(), r.GetLinkedItem(1));

                Assert.AreEqual("Craft Aerogel", r.DisplayName);
                Assert.AreEqual("Custom aerogel tooltip", r.Tooltip);

                Assert.AreEqual("Fabricator/Resources/BasicMaterials", r.Path);
            }

        }

        [Test]
        public void Deserialize_AliasRecipe_NoAmounts_DefaultTo1()
        {
            const string serialized = "AliasRecipe:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:AliasAerogel4;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    ForceUnlockAtStart: YES;" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new AliasRecipe();

            recipe.FromString(serialized);

            Assert.AreEqual("AliasAerogel4", recipe.ItemID);
            Assert.AreEqual(false, recipe.AmountCrafted.HasValue);

            Assert.AreEqual(2, recipe.IngredientsCount);

            var item0 = recipe.GetIngredient(0);
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(1, item0.Required);

            var item1 = recipe.GetIngredient(1);
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(1, item1.Required);

            Assert.AreEqual(2, recipe.LinkedItemsCount);
            Assert.AreEqual(TechType.Silver.ToString(), recipe.GetLinkedItem(0));
            Assert.AreEqual(TechType.Gold.ToString(), recipe.GetLinkedItem(1));

            Assert.AreEqual("Craft Aerogel", recipe.DisplayName);
            Assert.AreEqual("Custom aerogel tooltip", recipe.Tooltip);

            Assert.AreEqual(true, recipe.ForceUnlockAtStart);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }
        
        [Test]
        public void Deserialize_AliasRecipesList_NoAmounts_Defaults()
        {
            const string serialized = "AliasRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:AliasAerogel5;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    ForceUnlockAtStart:NO;" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:AliasAerogel6;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    ForceUnlockAtStart:NO;" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      ");" + "\r\n";


            var recipe = new CustomCraft2SML.Serialization.AliasRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual("AliasAerogel5", recipe[0].ItemID);
            Assert.AreEqual("AliasAerogel6", recipe[1].ItemID);
            foreach (var r in recipe)
            {
                Assert.AreEqual(false, recipe[0].AmountCrafted.HasValue);

                Assert.AreEqual(2, r.IngredientsCount);

                EmIngredient item0 = r.GetIngredient(0);
                Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
                Assert.AreEqual(1, item0.Required);

                EmIngredient item1 = r.GetIngredient(1);
                Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
                Assert.AreEqual(1, item1.Required);

                Assert.AreEqual(2, r.LinkedItemsCount);


                Assert.AreEqual(TechType.Silver.ToString(), r.GetLinkedItem(0));
                Assert.AreEqual(TechType.Gold.ToString(), r.GetLinkedItem(1));

                Assert.AreEqual("Craft Aerogel", r.DisplayName);
                Assert.AreEqual("Custom aerogel tooltip", r.Tooltip);

                Assert.AreEqual(false, r.ForceUnlockAtStart);

                Assert.AreEqual("Fabricator/Resources/BasicMaterials", r.Path);
            }
        }

        [Test]
        public void Deserialize_AliasRecipesList_AllLowerCaseIDs()
        {
            const string serialized = "AliasRecipes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:AliasAerogel7;" + "\r\n" +
                                      "    DisplayName:\"Craft Aerogel\";" + "\r\n" +
                                      "    Tooltip:\"Custom aerogel tooltip\";" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Titanium;" + "\r\n" +
                                      "        )," + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:Copper;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    ForceUnlockAtStart:NO;" + "\r\n" +
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:DummyDisinfectedWater;" + "\r\n" +
                                      "    DisplayName:\"Disinfected water\";" + "\r\n" +
                                      "    Tooltip:\"Craft disinfected water from filtered water\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:FilteredWater;" + "\r\n" +
                                      "            Required:2;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:DisinfectedWater;" + "\r\n" +
                                      "    Path:Fabricator/Survival/Water;" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:DummyDisinfectedWater2;" + "\r\n" +
                                      "    DisplayName:\"Disinfected water\";" + "\r\n" +
                                      "    Tooltip:\"Craft disinfected water from big filtered water\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:BigFilteredWater;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    LinkedItemIDs:DisinfectedWater,DisinfectedWater;" + "\r\n" +
                                      "    Path:Fabricator/Survival/Water;" +
                                      ");" + "\r\n";

            var recipeList = new CustomCraft2SML.Serialization.AliasRecipeList();

            bool success = recipeList.FromString(serialized);

            Assert.IsTrue(success);
            Assert.AreEqual(3, recipeList.Count);
        }
    }
}
