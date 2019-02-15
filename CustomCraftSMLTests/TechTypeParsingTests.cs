namespace CustomCraftSMLTests
{
    using Common.EasyMarkup;
    using NUnit.Framework;

    [TestFixture]
    public class TechTypeParsingTests
    {
        [TestCase("Compass")]
        [TestCase("compass")]
        [TestCase("COMPASS")]
        [TestCase("ComPass")]
        public void FromString_AllCasesHandled(string serialized)
        {
            const string key = "TT";
            var emTechType = new EmProperty<TechType>(key);

            Assert.IsTrue(emTechType.FromString($"{key}:{serialized};"));
            Assert.AreEqual(TechType.Compass, emTechType.Value);
            Assert.AreEqual(serialized, emTechType.SerializedValue);
        }

        [TestCase("Compass")]
        [TestCase("compass")]
        [TestCase("COMPASS")]
        [TestCase("ComPass")]
        public void Deserialize_AllCasesHandled(string serialized)
        {
            const string key = "TT";
            var emTechType = new EmProperty<TechType>(key);

            Assert.IsTrue(emTechType.Deserialize($"{key}: {serialized};"));
            Assert.AreEqual(TechType.Compass, emTechType.Value);
            Assert.AreEqual(serialized, emTechType.SerializedValue);
        }
    }
}
