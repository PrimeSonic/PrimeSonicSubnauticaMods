namespace CustomCraftSMLTests
{
    using CustomCraftSML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class RecipeTests
    {
        [Test]
        public void DeserializeRecipe()
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
                                      "    LinkedItemIDs:Silver,Gold;" + "\r\n" +
                                      "    Path:Fabricator/Resources/BasicMaterials;" + "\r\n" +
                                      ");" + "\r\n";

            var recipe = new CustomRecipeWithPath();

            recipe.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel, recipe.ItemID);
            Assert.AreEqual(1, recipe.AmountCrafted);

            Assert.AreEqual(2, recipe.Ingredients.Count);
            Assert.AreEqual(TechType.Titanium, recipe.Ingredients[0].ItemID);
            Assert.AreEqual(2, recipe.Ingredients[0].Required);
            Assert.AreEqual(TechType.Copper, recipe.Ingredients[1].ItemID);
            Assert.AreEqual(3, recipe.Ingredients[1].Required);

            Assert.AreEqual(2, recipe.LinkedItems.Count);
            Assert.AreEqual(TechType.Silver, recipe.LinkedItems[0]);
            Assert.AreEqual(TechType.Gold, recipe.LinkedItems[1]);

            Assert.AreEqual("Fabricator/Resources/BasicMaterials", recipe.Path);
        }


    }
}
