namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    public class CustomFoodTests
    {
        [Test]
        public void Deserialize_CustomFood_FullDetails()
        {
            const string serialized = "CustomFoods:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:verybigwater;" + "\r\n" +
                                      "    DisplayName:\"Very Big Water\";" + "\r\n" +
                                      "    Tooltip:\"A very Big Water\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:filteredwater;" + "\r\n" +
                                      "            Required:5;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    Path:Fabricator;" + "\r\n" +
                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ");" + "\r\n";

            var food = new CustomFood();

            food.FromString(serialized);

            //Assert.AreEqual(TechType.Aerogel.ToString(), food.ItemID);
            Assert.AreEqual(0, food.FoodValue);
            Assert.AreEqual(100, food.WaterValue);
        }

        [Test]
        public void Deserialize_CustomFoodsList_FullDetails()
        {
            const string serialized = "CustomFoods:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:verybigwater;" + "\r\n" +
                                      "    DisplayName:\"Very Big Water\";" + "\r\n" +
                                      "    Tooltip:\"A very Big Water\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:filteredwater;" + "\r\n" +
                                      "            Required:5;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    Path:Fabricator;" + "\r\n" +
                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:verybigwater;" + "\r\n" +
                                      "    DisplayName:\"Very Big Water\";" + "\r\n" +
                                      "    Tooltip:\"A very Big Water\";" + "\r\n" +
                                      "    AmountCrafted:0;" + "\r\n" +
                                      "    Ingredients:" + "\r\n" +
                                      "        (" + "\r\n" +
                                      "            ItemID:filteredwater;" + "\r\n" +
                                      "            Required:5;" + "\r\n" +
                                      "        );" + "\r\n" +
                                      "    Path:Fabricator;" + "\r\n" +
                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ");" + "\r\n";

            var foods = new CustomFoodList();

            foods.FromString(serialized);

            Assert.AreEqual(2, foods.Count);

            //Assert.AreEqual(TechType.Aerogel.ToString(), sizes[0].ItemID);
            Assert.AreEqual(0, foods[0].FoodValue);
            Assert.AreEqual(100, foods[0].WaterValue);
        }
    }
}
