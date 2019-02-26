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
        public void CraftingTab_Serialize_Deserialize()
        {
            var originalTab = new CustomCraftingTab("Workbench/OriginalTab")
            {
                TabID = "CustomTab",
                DisplayName = "Custom Tab",
                SpriteItemID = TechType.ComputerChip
            };

            string serialized = originalTab.ToString();
            Console.WriteLine(originalTab.PrettyPrint());

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual(originalTab.TabID, copiedTab.TabID);
            Assert.AreEqual(originalTab.DisplayName, copiedTab.DisplayName);
            Assert.AreEqual(originalTab.FabricatorType, copiedTab.FabricatorType);
            Assert.AreEqual(originalTab.ParentTabPath, copiedTab.ParentTabPath);
            Assert.AreEqual(originalTab.SpriteItemID, copiedTab.SpriteItemID);
            Assert.IsTrue(originalTab.Equals(copiedTab));
            Assert.IsTrue(originalTab == copiedTab);
        }

        [Test]
        public void CraftingTab_PrettyPrint_Deserialize()
        {
            var originalTab = new CustomCraftingTab("Workbench/OriginalTab")
            {
                TabID = "CustomTab",
                DisplayName = "Custom Tab",
                SpriteItemID = TechType.ComputerChip
            };

            string serialized = originalTab.PrettyPrint();

            var copiedTab = new CustomCraftingTab();
            copiedTab.FromString(serialized);

            Assert.AreEqual(originalTab.TabID, copiedTab.TabID);
            Assert.AreEqual(originalTab.DisplayName, copiedTab.DisplayName);
            Assert.AreEqual(originalTab.FabricatorType, copiedTab.FabricatorType);
            Assert.AreEqual(originalTab.ParentTabPath, copiedTab.ParentTabPath);
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
                                    ParentTabPath: Workbench/OriginalTab;
                                ); ";

            var tab = new CustomCraftingTab();
            tab.FromString(serialized);

            Assert.AreEqual("CustomTab", tab.TabID);
            Assert.AreEqual("Custom Tab", tab.DisplayName);
            Assert.AreEqual(TechType.ComputerChip, tab.SpriteItemID);
            Assert.AreEqual(CraftTree.Type.Workbench, tab.FabricatorType);
            Assert.AreEqual("Workbench/OriginalTab", tab.ParentTabPath);
            Assert.AreEqual(1, tab.StepsToTab.Length);
            Assert.AreEqual("OriginalTab", tab.StepsToTab[0]);
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
            Assert.AreEqual(CraftTree.Type.Workbench, tab.FabricatorType);
            Assert.AreEqual(1, tab.StepsToTab.Length);
        }

        [Test]
        public void CraftingTab_CheckPath()
        {

            var originalTab = new CustomCraftingTab("Fabricator/AdvancedMaterials")
            {
                TabID = "CustomTab",
                DisplayName = "Custom Tab",
                SpriteItemID = TechType.ComputerChip
            };
            

            Assert.IsNotNull(originalTab.StepsToTab);

        }
    }
}
