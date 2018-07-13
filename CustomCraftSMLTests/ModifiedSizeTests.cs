namespace CustomCraftSMLTests
{
    using System;
    using CustomCraftSML.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Oculus.Newtonsoft.Json;

    [TestClass]
    public class ModifiedSizeTests
    {
        [TestMethod]
        public void ModifiedSize_ToString()
        {
            var modSize = new ModifiedSize()
            {
                ItemID = "Aerogel",// TechType.Aerogel,
                Width = 1,
                Height = 2
            };

            const string expectedString = "{\"ItemID\":\"Aerogel\",\"Width\":1,\"Height\":2}";

            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(expectedString, JsonConvert.SerializeObject(modSize));
            //Assert.AreEqual(expectedString, modSize.ToString());
        }

        [TestMethod]
        public void ModifiedSize_DeserializeObject_GoodString1()
        {
            const string testString = "{\"ItemID\":\"Aerogel\",\"Width\":1,\"Height\":2}";

            var modSize = JsonConvert.DeserializeObject<ModifiedSize>(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_DeserializeObject_GoodString2()
        {
            const string testString = "{\"ItemID\": \"Aerogel\",\"Width\": 1,\"Height\": 2}";

            var modSize = JsonConvert.DeserializeObject<ModifiedSize>(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_DeserializeObject_GoodString3()
        {
            const string testString = "{ \"ItemID\":\"Aerogel\", \"Width\":1, \"Height\":2 }";

            var modSize = JsonConvert.DeserializeObject<ModifiedSize>(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_DeserializeObject_GoodString4()
        {
            const string testString = "{\"ItemID\":\"aerogel\", \"Width\":1,\"Height\":2}";

            var modSize = JsonConvert.DeserializeObject<ModifiedSize>(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_DeserializeObject_GoodString5()
        {
            string testString = "{" + Environment.NewLine +
                                "    \"ItemID\":\"aerogel\", " + Environment.NewLine +
                                "    \"Width\":1, " + Environment.NewLine +
                                "    \"Height\":2" + Environment.NewLine +
                                "}";

            var modSize = JsonConvert.DeserializeObject<ModifiedSize>(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.TechTypeID);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

    }
}
