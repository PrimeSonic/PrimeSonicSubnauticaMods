namespace CustomCraftSMLTests
{
    using System;
    using CustomCraftSML.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UnityEngine;

    [TestClass]
    public class ModifiedSizeTests
    {
        [TestMethod]
        public void ModifiedSize_ToString()
        {
            var modSize = new ModifiedSize()
            {
                InventoryItem = TechType.Aerogel,
                Width = 1,
                Height = 2
            };

            const string expectedString = "(TechType:Aerogel;Width:1;Height:2)";

            Assert.AreEqual(expectedString, modSize.ToString());
        }

        [TestMethod]
        public void ModifiedSize_Parse_GoodString1()
        {
            const string testString = "(TechType:Aerogel;Width:1;Height:2)";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.InventoryItem);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_Parse_GoodString2()
        {
            const string testString = "(TechType:Aerogel; Width:1; Height:2)";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.InventoryItem);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_Parse_GoodString3()
        {
            const string testString = "( TechType: Aerogel; Width: 1; Height: 2 )";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNotNull(modSize);
            Assert.AreEqual(TechType.Aerogel, modSize.InventoryItem);
            Assert.AreEqual(1, modSize.Width);
            Assert.AreEqual(2, modSize.Height);
        }

        [TestMethod]
        public void ModifiedSize_Parse_BadString()
        {
            const string testString = "(Tech: A; W: 1; H: 2 )";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNull(modSize);
        }

        [TestMethod]
        public void ModifiedSize_Parse_BadEnum()
        {
            const string testString = "( TechType: Air-o-gel; Width: 1; Height: 2 )";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNull(modSize);
        }

        [TestMethod]
        public void ModifiedSize_Parse_BadNumbers()
        {
            const string testString = "( TechType: Aerogel; Width: 10; Height: 20 )";

            var modSize = ModifiedSize.Parse(testString);

            Assert.IsNull(modSize);
        }
    }
}
