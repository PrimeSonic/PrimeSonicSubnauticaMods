namespace CustomCraftSMLTests
{
    using System;
    using CustomCraft2SML.Serialization;
    using NUnit.Framework;

    [TestFixture]
    internal class CraftingTabTests
    {
        [Test]
        public void CraftingTab_Serialize_Deserialize()
        {
            var originalTab = new CustomCraftingTab
            {
                TabID = "CustomTab",
                DisplayName = "Custom Tab",
                FabricatorType = CraftTree.Type.Workbench,
                ParentTabID = "OriginalTab",
                SpriteItemID = TechType.ComputerChip
            };

            string serialized = originalTab.ToString();
            Console.WriteLine(originalTab.PrettyPrint());

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual(originalTab.TabID, copiedTab.TabID);
            Assert.AreEqual(originalTab.DisplayName, copiedTab.DisplayName);
            Assert.AreEqual(originalTab.FabricatorType, copiedTab.FabricatorType);
            Assert.AreEqual(originalTab.ParentTabID, copiedTab.ParentTabID);
            Assert.AreEqual(originalTab.SpriteItemID, copiedTab.SpriteItemID);
            Assert.IsTrue(originalTab.Equals(copiedTab));
            Assert.IsTrue(originalTab == copiedTab);
        }

        [Test]
        public void CraftingTab_PrettyPrint_Deserialize()
        {
            var originalTab = new CustomCraftingTab
            {
                TabID = "CustomTab",
                DisplayName = "Custom Tab",
                FabricatorType = CraftTree.Type.Workbench,
                ParentTabID = "OriginalTab",
                SpriteItemID = TechType.ComputerChip
            };

            string serialized = originalTab.PrettyPrint();

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual(originalTab.TabID, copiedTab.TabID);
            Assert.AreEqual(originalTab.DisplayName, copiedTab.DisplayName);
            Assert.AreEqual(originalTab.FabricatorType, copiedTab.FabricatorType);
            Assert.AreEqual(originalTab.ParentTabID, copiedTab.ParentTabID);
            Assert.AreEqual(originalTab.SpriteItemID, copiedTab.SpriteItemID);
            Assert.IsTrue(originalTab.Equals(copiedTab));
            Assert.IsTrue(originalTab == copiedTab);
        }

        [Test]
        public void CraftingTab_Deserialize()
        {
            string serialized = @"CustomTab: 
                                (
                                    TabID: CustomTab;
                                    DisplayName: ""Custom Tab"";
                                    SpriteItemID: ComputerChip;
                                    FabricatorType: Workbench;
                                    ParentTabID: OriginalTab;
                                ); ";

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual("CustomTab", copiedTab.TabID);
            Assert.AreEqual("Custom Tab", copiedTab.DisplayName);
            Assert.AreEqual(TechType.ComputerChip, copiedTab.SpriteItemID);
            Assert.AreEqual(CraftTree.Type.Workbench, copiedTab.FabricatorType);
            Assert.AreEqual("OriginalTab", copiedTab.ParentTabID);
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
                    FabricatorType = CraftTree.Type.CyclopsFabricator,
                    ParentTabID = "OriginalTab_1",
                    SpriteItemID = TechType.Cyclops
                },
                new CustomCraftingTab
                {
                    TabID = "CustomTab2",
                    DisplayName = "Custom Tab The Second",
                    FabricatorType = CraftTree.Type.SeamothUpgrades,
                    ParentTabID = "OriginalTab_2",
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
        public void CraftingTab_NoParentTab_Deserialize()
        {
            string serialized = @"CustomTab: 
                                (
                                    TabID: CustomTab;
                                    DisplayName: ""Custom Tab"";
                                    SpriteItemID: ComputerChip;
                                    FabricatorType: Workbench;
                                ); ";

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual("CustomTab", copiedTab.TabID);
            Assert.AreEqual("Custom Tab", copiedTab.DisplayName);
            Assert.AreEqual(TechType.ComputerChip, copiedTab.SpriteItemID);
            Assert.AreEqual(CraftTree.Type.Workbench, copiedTab.FabricatorType);
            Assert.AreEqual(null, copiedTab.ParentTabID);
        }
    }
}
