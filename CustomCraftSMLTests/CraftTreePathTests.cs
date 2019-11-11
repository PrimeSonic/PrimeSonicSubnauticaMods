namespace CustomCraftSMLTests
{
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    internal class CraftTreePathTests
    {
        [TestCase("Fabricator/Resources/BasicMaterials", "Titanium", CraftTree.Type.Fabricator)]
        [TestCase("Fabricator/Resources", "CustomTab", CraftTree.Type.Fabricator)]
        [TestCase("Fabricator", "Titanium", CraftTree.Type.Fabricator)]
        public void CraftTreePath_GivenPathAndNode_GetExpectedScheme(string path, string id, CraftTree.Type expectedScheme)
        {
            var cPath = new CraftTreePath(path, id);
            Assert.AreEqual(expectedScheme, cPath.Scheme);
        }

        [TestCase("Fabricator/Resources/BasicMaterials", "Titanium", false)]
        [TestCase("Fabricator/Resources", "CustomTab", false)]
        [TestCase("Fabricator", "Titanium", true)]
        public void CraftTreePath_GivenPathAndNode_GetExpectedIsAtRoot(string path, string id, bool shouldBeRoot)
        {
            var cPath = new CraftTreePath(path, id);
            Assert.AreEqual(shouldBeRoot, cPath.IsAtRoot);
        }

        [TestCase("Fabricator/Resources/Electronics", "Battery", 3)]
        [TestCase("Fabricator/Resources", "OtherTab", 2)]
        [TestCase("Fabricator", "Titanium", 1)]
        public void CraftTreePath_WhenRemovingNode_CheckPathForRemoval(string path, string id, int expectedStepCount)
        {
            var cPath = new CraftTreePath(path, id);
            string[] stepsToNode = cPath.StepsToNode;
            Assert.AreEqual(expectedStepCount, stepsToNode.Length);
            Assert.AreEqual(id, stepsToNode[expectedStepCount - 1]);
        }

        [TestCase("Fabricator", "NewTab")]
        [TestCase("CyclopsFabricator", "CyclopsModules")]
        [TestCase("Fabricator", "Titanium")]
        public void CraftTreePath_WhenAddingNodeToRoot_NoStepsGiven(string path, string id)
        {
            var cPath = new CraftTreePath(path, id);
            Assert.IsNull(cPath.StepsToParentTab);
            Assert.IsTrue(cPath.IsAtRoot);
        }

        [TestCase("Fabricator/Resources/Electronics", "Battery", 2)]
        [TestCase("Fabricator/Resources", "OtherTab", 1)]
        public void CraftTreePath_WhenAddingNodeNotAtRoot_CheckPathForAddition(string path, string id, int expectedStepCount)
        {
            var cPath = new CraftTreePath(path, id);

            string[] stepsToParent = cPath.StepsToParentTab;
            Assert.AreEqual(expectedStepCount, stepsToParent.Length);
            Assert.AreNotEqual(id, stepsToParent[expectedStepCount - 1]);
            Assert.IsFalse(cPath.IsAtRoot);
        }

    }
}
