namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;

    internal class CfAliasRecipe : AliasRecipe, ICustomFabricatorEntry
    {
        public CfAliasRecipe() : this(TypeName, AliasRecipeProperties)
        {
        }

        protected CfAliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.Path == this.ParentFabricator.ItemID;

        public CraftingPath CraftingNodePath
        {
            get
            {
                string trimmedPath = this.Path.Replace($"{this.ParentFabricator.ItemID}", string.Empty).TrimStart('/');

                return new CraftingPath(this.TreeTypeID, this.Path);
            }
        }

        protected override void HandleCraftTreeAddition() => this.ParentFabricator.HandleCraftTreeAddition(this);

        internal override EmProperty Copy() => new CfAliasRecipe(this.Key, CopyDefinitions);
    }
}

