namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    public class CustomSizeTests
    {
        [Test]
        public void Deserialize_CustomSize_FullDetails()
        {
            const string serialized = "CustomSize:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Width:3;" + "\r\n" +
                                      "    Height:4;" + "#THIS IS A COMMENT#" + "\r\n" +
                                      ");" + "\r\n";

            var size = new CustomSize();

            size.FromString(serialized);

            Assert.AreEqual(TechType.Aerogel.ToString(), size.ItemID);
            Assert.AreEqual(3, size.Width);
            Assert.AreEqual(4, size.Height);
        }

        [Test]
        public void Deserialize_CustomSizesList_FullDetails()
        {
            const string serialized = "CustomSizes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "#THIS IS A COMMENT#" + "\r\n" +
                                      "    Width:3;" + "\r\n" +
                                      "    Height:4;" + "\r\n" +
                                      ")," + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      "    Width:3;" + "\r\n" +
                                      "    Height:4;" + "\r\n" +
                                      ");" + "\r\n";
            
            var sizes = new CustomSizeList();

            sizes.FromString(serialized);

            Assert.AreEqual(2, sizes.Count);

            Assert.AreEqual(TechType.Aerogel.ToString(), sizes[0].ItemID);
            Assert.AreEqual(3, sizes[0].Width);
            Assert.AreEqual(4, sizes[0].Height);
        }

        [Test]
        public void Deserialize_CustomSizesList_NoAmounts_DefaultTo1()
        {
            const string serialized = "CustomSizes:" + "\r\n" +
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      ")," + "#THIS IS A COMMENT#" + "\r\n" + 
                                      "(" + "\r\n" +
                                      "    ItemID:Aerogel;" + "\r\n" +
                                      ");" + "\r\n";

            var sizes = new CustomSizeList();

            sizes.FromString(serialized);

            Assert.AreEqual(2, sizes.Count);

            Assert.AreEqual(TechType.Aerogel.ToString(), sizes[0].ItemID);
            Assert.AreEqual(1, sizes[0].Width);
            Assert.AreEqual(1, sizes[0].Height);
        }
    }
}
