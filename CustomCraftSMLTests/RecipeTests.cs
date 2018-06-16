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


        }


    }
}
