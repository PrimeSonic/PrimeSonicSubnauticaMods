namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization;
    using EasyMarkup;

    internal class CfCustomFood : CustomFood, ICustomFabricatorEntry
    {
        public CfCustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CfCustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.Path == this.ParentFabricator.ItemID;

        public CraftTreePath GetCraftTreePath()
        {
            return new CraftTreePath(this.Path, this.ItemID);
        }

        protected override void HandleCraftTreeAddition()
        {
            this.ParentFabricator.HandleCraftTreeAddition(this);
        }

        internal override EmProperty Copy()
        {
            return new CfCustomFood(this.Key, this.CopyDefinitions);
        }
    }
}

