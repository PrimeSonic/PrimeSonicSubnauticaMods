namespace CustomCraftSMLTests
{
    using EasyMarkup;
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

        [TestCase("PrecursorIonBattery", TechType.PrecursorIonBattery)]
        [TestCase("WhiteMushroom", TechType.WhiteMushroom)]
        [TestCase("Lithium", TechType.Lithium)]
        [TestCase("Magnetite", TechType.Magnetite)]
        [TestCase("UraniniteCrystal", TechType.UraniniteCrystal)]
        public void FromString_BasicCasesHandled(string serialized, TechType value)
        {
            const string key = "TT";
            var emTechType = new EmProperty<TechType>(key);

            Assert.IsTrue(emTechType.FromString($"{key}:{serialized};"));
            Assert.AreEqual(value, emTechType.Value);
            Assert.AreEqual(serialized, emTechType.SerializedValue);
        }

        [Test]
        public void FromString_ForList_AllEntriesParsed()
        {
            const string key = "TT";
            const string serialized = "WhiteMushroom,WhiteMushroom,WhiteMushroom,\r\n" +
                                      "Lithium,AluminumOxide,Magnetite,\r\n" +
                                      "PrecursorIonBattery,HatchingEnzymes,HatchingEnzymes,\r\n" +
                                      "Lead,UraniniteCrystal,UraniniteCrystal";

            var emTechType = new EmPropertyList<TechType>(key);

            Assert.IsTrue(emTechType.FromString($"{key}:{serialized};"));
            Assert.IsTrue(emTechType.HasValue);
            Assert.AreEqual(12, emTechType.Values.Count);

            Assert.AreEqual(TechType.WhiteMushroom, emTechType.Values[0]);
            Assert.AreEqual(TechType.WhiteMushroom, emTechType.Values[1]);
            Assert.AreEqual(TechType.WhiteMushroom, emTechType.Values[2]);
            Assert.AreEqual(TechType.Lithium, emTechType.Values[3]);
            Assert.AreEqual(TechType.AluminumOxide, emTechType.Values[4]);
            Assert.AreEqual(TechType.Magnetite, emTechType.Values[5]);
            Assert.AreEqual(TechType.PrecursorIonBattery, emTechType.Values[6]);
            Assert.AreEqual(TechType.HatchingEnzymes, emTechType.Values[7]);
            Assert.AreEqual(TechType.HatchingEnzymes, emTechType.Values[8]);
            Assert.AreEqual(TechType.Lead, emTechType.Values[9]);
            Assert.AreEqual(TechType.UraniniteCrystal, emTechType.Values[10]);
            Assert.AreEqual(TechType.UraniniteCrystal, emTechType.Values[11]);
        }
    }
}
