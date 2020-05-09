namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization;
    using EasyMarkup;
    using SMLHelper.V2.Crafting;

    internal class CfCustomCraftingTab : CustomCraftingTab, ICustomFabricatorEntry
    {
        public CfCustomCraftingTab() : this(TypeName, CustomCraftingTabProperties)
        {
            base.OnValueExtractedEvent -= ParsePath;
        }

        protected CfCustomCraftingTab(string key, ICollection<EmProperty> definitions) : base()
        {

        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.ParentTabPath == this.ParentFabricator.ItemID;

        public CraftTreePath GetCraftTreePath()
        {
            return new CraftTreePath(this.ParentTabPath, this.TabID);
        }

        protected override bool ValidFabricator()
        {
            if (!this.ParentTabPath.StartsWith(this.ParentFabricator.ItemID))
            {
                QuickLogger.Warning($"Inner {this.Key} for {this.ParentFabricator.Key} from {this.Origin} appears to have a {ParentTabPathKey} for another fabricator '{this.ParentTabPath}'");
                return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            QuickLogger.Debug($"CraftingNodePath for {this.Key} '{this.TabID}' set to {this.ParentTabPath}");
            try
            {
                if (this.IsAtRoot)
                {
                    this.ParentFabricator.RootNode.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite());
                }
                else
                {
                    CraftTreePath craftTreePath = GetCraftTreePath();
                    if (craftTreePath.HasError)
                    {
                        QuickLogger.Error($"Encountered error in path for '{this.TabID}' - Entry from {this.Origin} - Error Message: {this.CraftingPath.Error}");
                        return false;
                    }

                    ModCraftTreeTab otherTab = this.ParentFabricator.RootNode.GetTabNode(craftTreePath.StepsToParentTab);
                    otherTab.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite());
                }

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.TabID}' from {this.Origin}", ex);
                return false;
            }
        }

        internal override EmProperty Copy()
        {
            return new CfCustomCraftingTab(this.Key, this.CopyDefinitions);
        }
    }
}
