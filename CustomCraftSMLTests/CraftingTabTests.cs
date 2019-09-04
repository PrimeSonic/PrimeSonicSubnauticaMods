namespace CustomCraftSMLTests
{
    using System;
    using CustomCraft2SML.Serialization.Entries;
    using CustomCraft2SML.Serialization.Lists;
    using NUnit.Framework;

    [TestFixture]
    internal class CraftingTabTests
    {

        [Test]
        public void CraftingTab_Deserialize()
        {
            string serialized = @"CustomTab: 
                                (
                                    TabID: CustomTab;
                                    DisplayName: ""Custom Tab"";
                                    SpriteItemID: ComputerChip;
                                    ParentTabPath: Workbench/OriginalTab;
                                ); ";

            var tab = new CustomCraftingTab();
            tab.FromString(serialized);

            Assert.AreEqual("CustomTab", tab.TabID);
            Assert.AreEqual("Custom Tab", tab.DisplayName);
            Assert.AreEqual(TechType.ComputerChip, tab.SpriteItemID);
            Assert.AreEqual(CraftTree.Type.Workbench, tab.CraftingPath.Scheme);
            Assert.AreEqual("Workbench/OriginalTab", tab.ParentTabPath);
            Assert.AreEqual(1, tab.CraftingPath.StepsToParentTab.Length);
            Assert.AreEqual("OriginalTab", tab.CraftingPath.StepsToParentTab[0]);
        }

        [Test]
        public void CraftingTabList_Serialize_Deserialize()
        {
            var tabList = new CustomCraftingTabList
            {
                new CustomCraftingTab
                {
                    TabID = "CustomTab1",
                    DisplayName = "Custom Tab The First",
                    ParentTabPath = "CyclopsFabricator/OriginalTab_1",
                    SpriteItemID = TechType.Cyclops
                },
                new CustomCraftingTab
                {
                    TabID = "CustomTab2",
                    DisplayName = "Custom Tab The Second",
                    ParentTabPath = "SeamothUpgrades/OriginalTab_2",
                    SpriteItemID = TechType.Seamoth
                },
            };

            string serialized = tabList.PrettyPrint();
            Console.WriteLine(serialized);

            var tabList2 = new CustomCraftingTabList();
            tabList2.FromString(serialized);

            Assert.AreEqual(tabList, tabList2);
            Assert.IsTrue(tabList.Equals(tabList2));
            Assert.IsTrue(tabList == tabList2);
        }

        [Test]
        public void CraftingTab_AtRoot_Deserialize()
        {
            string serialized = @"CustomTab: 
                                (
                                    TabID: CustomTab;
                                    DisplayName: ""Custom Tab"";
                                    SpriteItemID: ComputerChip;
                                    ParentTabPath: Workbench;
                                ); ";

            var tab = new CustomCraftingTab();
            tab.FromString(serialized);

            Assert.AreEqual("CustomTab", tab.TabID);
            Assert.AreEqual("Custom Tab", tab.DisplayName);
            Assert.AreEqual(TechType.ComputerChip, tab.SpriteItemID);
            Assert.AreEqual(CraftTree.Type.Workbench, tab.CraftingPath.Scheme);
            Assert.IsNull(tab.CraftingPath.StepsToParentTab);
        }
    }
}
