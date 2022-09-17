namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    public class ModifiedFoodTests
    {
        [Test]
        public void Deserialize_ModifiedFood_FullDetails()
        {
            const string serialized = "ModifiedFood:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:filteredwater;" + "\r\n" +                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ");" + "\r\n";

            var food = new ModifiedFood();

            food.FromString(serialized);

            //Assert.AreEqual(TechType.Aerogel.ToString(), food.ItemID);
            Assert.AreEqual(0, food.FoodValue);
            Assert.AreEqual(100, food.WaterValue);
        }

        [Test]
        public void Deserialize_ModifiedFoodsList_FullDetails()
        {
            const string serialized = "ModifiedFoods:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:filteredwater;" + "\r\n" +
                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:filteredwater;" + "\r\n" +
                                      "    FoodValue:0;" + "\r\n" +
                                      "    WaterValue:100;" + "\r\n" +
                                      ");" + "\r\n";

            var foods = new ModifiedFoodList();

            foods.FromString(serialized);

            Assert.AreEqual(2, foods.Count);

            //Assert.AreEqual(TechType.Aerogel.ToString(), sizes[0].ItemID);
            Assert.AreEqual(0, foods[0].FoodValue);
            Assert.AreEqual(100, foods[0].WaterValue);
        }
    }
}
