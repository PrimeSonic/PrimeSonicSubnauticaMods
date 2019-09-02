namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization;

    internal class CfMovedRecipe : MovedRecipe, ICustomFabricatorEntry
    {
        public CfMovedRecipe() : this(TypeName, MovedRecipeProperties)
        {
        }

        protected CfMovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.NewPath == this.ParentFabricator.ItemID;

        public CraftTreePath CraftingNodePath => new CraftTreePath(this.NewPath, this.ItemID);

        protected override void HandleCraftTreeAddition()
        {
            this.ParentFabricator.HandleCraftTreeAddition(this);
        }

        internal override EmProperty Copy()
        {
            return new CfMovedRecipe(this.Key, this.CopyDefinitions);
        }
    }
}
