namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using SMLHelper.V2.Crafting;

    internal class CfCustomCraftingTab : CustomCraftingTab, ICustomFabricatorEntry
    {
        public CfCustomCraftingTab()
        {
            base.OnValueExtractedEvent -= ParsePath;
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.BuildableFabricator.TreeTypeID;

        public ModCraftTreeRoot RootNode => this.ParentFabricator.BuildableFabricator.RootNode;

        public bool IsAtRoot => this.ParentTabPath == this.ParentFabricator.ItemID;

        public string[] StepsToParentNode => craftingPath.CraftNodeSteps;

        protected override bool ValidFabricator()
        {
            string trimmedPath = this.ParentTabPath.Replace($"{this.ParentFabricator.ItemID}", string.Empty).TrimStart('/');
            craftingPath = new CraftingPath(this.TreeTypeID, trimmedPath);

            if (!this.ParentTabPath.StartsWith(this.ParentFabricator.ItemID) || craftingPath.Scheme != this.TreeTypeID)
            {
                QuickLogger.Warning($"Inner {this.Key} for {this.ParentFabricator.Key} appears to have a {ParentTabPathKey} for another fabricator '{this.ParentTabPath}'");
                return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            try
            {
                if (this.IsAtRoot)
                {
                    RootNode.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite());
                }
                else
                {
                    ModCraftTreeTab otherTab = RootNode.GetTabNode(this.StepsToParentNode);
                    otherTab.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite());
                }

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.TabID}'", ex);
                return false;
            }
        }
    }
}
