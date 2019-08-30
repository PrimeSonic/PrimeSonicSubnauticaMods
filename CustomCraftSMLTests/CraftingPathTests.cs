namespace CustomCraftSMLTests
{
    using CustomCraft2SML.PublicAPI;
    using NUnit.Framework;

    [TestFixture]
    internal class CraftingPathTests
    {
        [TestCase("Fabricator/Resources/BasicMaterials", "Titanium", CraftTree.Type.Fabricator)]
        [TestCase("Fabricator/Resources", "Titanium", CraftTree.Type.Fabricator)]
        [TestCase("Fabricator", "Titanium", CraftTree.Type.Fabricator)]
        public void CraftingPath_GivenPathID_GetExpectedScheme(string path, string id, CraftTree.Type expectedScheme)
        {
            var cPath = new CraftingPath(path, id);
            Assert.AreEqual(expectedScheme, cPath.Scheme);
        }

        [TestCase("Fabricator/Resources/BasicMaterials/Other", "Titanium", 3)]
        [TestCase("Fabricator/Resources/BasicMaterials", "Titanium", 2)]
        [TestCase("Fabricator/Resources", "Titanium", 1)]
        [TestCase("Fabricator", "Titanium", 1)]
        public void CraftingPath_GivenPathID_GetExpectedStepCount(string path, string id, int expectedSteps)
        {
            var cPath = new CraftingPath(path, id);
            Assert.AreEqual(expectedSteps, cPath.Steps.Length);
        }

        [TestCase("Fabricator/Resources/BasicMaterials", "Titanium", "Resources")]
        [TestCase("Fabricator/Resources", "Titanium", "Resources")]
        [TestCase("Fabricator", "Titanium", "Titanium")]
        public void CraftingPath_GivenPathID_GetExpectedStepZerp(string path, string id, string expectedZero)
        {
            var cPath = new CraftingPath(path, id);
            Assert.AreEqual(expectedZero, cPath.Steps[0]);
        }

        [Test]
        public void CraftingPath_GivenPathID_WhenAtRoot_GetExpected()
        {
            var cPath = new CraftingPath("CyclopsFabricator", "CyclopsFireSuppressionModule");
            Assert.IsTrue(cPath.IsAtRoot);
            Assert.AreEqual(1, cPath.Steps.Length);
            Assert.AreEqual("CyclopsFireSuppressionModule", cPath.Steps[0]);
            Assert.AreEqual(CraftTree.Type.CyclopsFabricator, cPath.Scheme);
        }

        [Test]
        public void CraftingPath_GivenPathID_WhenInDeepTab_GetExpected()
        {
            var cPath = new CraftingPath("CyclopsFabricator/CyclopsModules/PowerModules", "PowerUpgradeModule");
            Assert.IsFalse(cPath.IsAtRoot);
            Assert.AreEqual(2, cPath.Steps.Length);
            Assert.AreEqual("CyclopsModules", cPath.Steps[0]);
            Assert.AreEqual("PowerModules", cPath.Steps[1]);
            Assert.AreEqual(CraftTree.Type.CyclopsFabricator, cPath.Scheme);
        }

        [Test]
        public void CraftingPath_GivenPathID_WhenInShallowTab_GetExpected()
        {
            var cPath = new CraftingPath("Fabricator/Personal", "CustomItem");
            Assert.IsFalse(cPath.IsAtRoot);
            Assert.AreEqual(1, cPath.Steps.Length);
            Assert.AreEqual("Personal", cPath.Steps[0]);
            Assert.AreEqual(CraftTree.Type.Fabricator, cPath.Scheme);
        }
    }
}
