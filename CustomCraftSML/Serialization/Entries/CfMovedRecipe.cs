namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.PublicAPI;

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

        public CraftingPath CraftingNodePath
        {
            get
            {
                string trimmedPath = this.NewPath.Replace($"{this.ParentFabricator.ItemID}", string.Empty).TrimStart('/');

                return new CraftingPath(this.TreeTypeID, this.NewPath);
            }
        }

        protected override void HandleCraftTreeAddition() => this.ParentFabricator.HandleCraftTreeAddition(this);

        internal override EmProperty Copy() => new CfMovedRecipe(this.Key, CopyDefinitions);
    }
}
