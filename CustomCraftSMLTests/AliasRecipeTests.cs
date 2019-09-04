namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
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

            Assert.AreEqual(2, recipe.Ingredients.Count);


            EmIngredient item0 = recipe.Ingredients[0];
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(2, item0.Required);

            EmIngredient item1 = recipe.Ingredients[1];
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(3, item1.Required);

            Assert.AreEqual(2, recipe.LinkedItemIDs.Count);

            Assert.AreEqual(TechType.Silver.ToString(), recipe.LinkedItemIDs[0]);
            Assert.AreEqual(TechType.Gold.ToString(), recipe.LinkedItemIDs[1]);

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
                                      "    FunctionalID:Aerogel;" +
                                      "    SpriteItemID:Aerogel;" +
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
                                      "    FunctionalID:Aerogel;" +
                                      "    SpriteItemID:Aerogel;" +
                                      ");" + "\r\n";


            var recipe = new AliasRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual("AliasAerogel2", recipe[0].ItemID);
            Assert.AreEqual("AliasAerogel3", recipe[1].ItemID);

            foreach (AliasRecipe r in recipe)
            {
                Assert.AreEqual(0, r.AmountCrafted);

                Assert.AreEqual(2, r.Ingredients.Count);

                EmIngredient item0 = r.Ingredients[0];
                Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
                Assert.AreEqual(2, item0.Required);

                EmIngredient item1 = r.Ingredients[1];
                Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
                Assert.AreEqual(3, item1.Required);

                Assert.AreEqual(2, r.LinkedItemIDs.Count);


                Assert.AreEqual(TechType.Silver.ToString(), r.LinkedItemIDs[0]);
                Assert.AreEqual(TechType.Gold.ToString(), r.LinkedItemIDs[1]);

                Assert.AreEqual("Craft Aerogel", r.DisplayName);
                Assert.AreEqual("Custom aerogel tooltip", r.Tooltip);

                Assert.AreEqual("Fabricator/Resources/BasicMaterials", r.Path);

                Assert.AreEqual(TechType.Aerogel.ToString(), r.FunctionalID);
                Assert.AreEqual(TechType.Aerogel, r.SpriteItemID);
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

            Assert.AreEqual(2, recipe.Ingredients.Count);

            EmIngredient item0 = recipe.Ingredients[0];
            Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
            Assert.AreEqual(1, item0.Required);

            EmIngredient item1 = recipe.Ingredients[1];
            Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
            Assert.AreEqual(1, item1.Required);

            Assert.AreEqual(2, recipe.LinkedItemIDs.Count);
            Assert.AreEqual(TechType.Silver.ToString(), recipe.LinkedItemIDs[0]);
            Assert.AreEqual(TechType.Gold.ToString(), recipe.LinkedItemIDs[1]);

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


            var recipe = new AliasRecipeList();

            recipe.FromString(serialized);

            Assert.AreEqual("AliasAerogel5", recipe[0].ItemID);
            Assert.AreEqual("AliasAerogel6", recipe[1].ItemID);
            foreach (AliasRecipe r in recipe)
            {
                Assert.AreEqual(false, recipe[0].AmountCrafted.HasValue);

                Assert.AreEqual(2, r.Ingredients.Count);

                EmIngredient item0 = r.Ingredients[0];
                Assert.AreEqual(TechType.Titanium.ToString(), item0.ItemID);
                Assert.AreEqual(1, item0.Required);

                EmIngredient item1 = r.Ingredients[1];
                Assert.AreEqual(TechType.Copper.ToString(), item1.ItemID);
                Assert.AreEqual(1, item1.Required);

                Assert.AreEqual(2, r.LinkedItemIDs.Count);


                Assert.AreEqual(TechType.Silver.ToString(), r.LinkedItemIDs[0]);
                Assert.AreEqual(TechType.Gold.ToString(), r.LinkedItemIDs[1]);

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

            var recipeList = new AliasRecipeList();

            bool success = recipeList.FromString(serialized);

            Assert.IsTrue(success);
            Assert.AreEqual(3, recipeList.Count);
        }

        [Test]
        public void Deserialize_FoodCloneExample()
        {
            const string lineBreak = "\r\n";
            const string displayName = "My Custom Food";
            const string toolTip = "My Food Tooltip";


            string serialized = "AliasRecipes:" + lineBreak +
                                "(" + lineBreak +
                                "    ItemID:CustomFoodExperiment;" + lineBreak +
                               $"    DisplayName:\"{displayName}\";" + lineBreak +
                               $"    Tooltip:\"{toolTip}\";" + lineBreak +
                                "    AmountCrafted:1;" + lineBreak +
                                "    Ingredients:" + lineBreak +
                                "        (" + lineBreak +
                               $"            ItemID:{TechType.Salt};" + lineBreak +
                                "            Required:2;" + lineBreak +
                                "        )," + lineBreak +
                                "        (" + lineBreak +
                               $"            ItemID:{TechType.CookedPeeper};" + lineBreak +
                                "            Required:1;" + lineBreak +
                                "        )," + lineBreak +
                                "        (" + lineBreak +
                               $"            ItemID:{TechType.CookedEyeye};" + lineBreak +
                                "            Required:1;" + lineBreak +
                                "        );" + lineBreak +
                               $"    Path:Fabricator/Sustenance/CookedFood;" +
                                "    FunctionalID:CuredLavaEyeye;" +
                                ");" + lineBreak;

            var recipe = new AliasRecipeList();

            recipe.FromString(serialized);

            AliasRecipe r = recipe[0];

            Assert.AreEqual("CustomFoodExperiment", r.ItemID);

            Assert.AreEqual(1, r.AmountCrafted);

            Assert.AreEqual(3, r.Ingredients.Count);

            EmIngredient item0 = r.Ingredients[0];
            Assert.AreEqual(TechType.Salt.ToString(), item0.ItemID);
            Assert.AreEqual(2, item0.Required);

            EmIngredient item1 = r.Ingredients[1];
            Assert.AreEqual(TechType.CookedPeeper.ToString(), item1.ItemID);
            Assert.AreEqual(1, item1.Required);

            EmIngredient item2 = r.Ingredients[2];
            Assert.AreEqual(TechType.CookedEyeye.ToString(), item2.ItemID);
            Assert.AreEqual(1, item2.Required);

            Assert.AreEqual(0, r.LinkedItemIDs.Count);

            Assert.AreEqual(displayName, r.DisplayName);
            Assert.AreEqual(toolTip, r.Tooltip);

            Assert.AreEqual("Fabricator/Sustenance/CookedFood", r.Path);

            Assert.AreEqual(TechType.CuredLavaEyeye.ToString(), r.FunctionalID);
        }
    }
}
