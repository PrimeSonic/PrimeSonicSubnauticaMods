namespace CustomCraft2SML.Serialization.Entries
{
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.PublicAPI;
    using SMLHelper.V2.Crafting;
    using System;
    using System.Collections.Generic;

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

        public CraftingPath CraftingNodePath => new CraftingPath(this.ParentTabPath, this.TabID);

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
                    ModCraftTreeTab otherTab = this.ParentFabricator.RootNode.GetTabNode(this.CraftingNodePath.Steps);
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
