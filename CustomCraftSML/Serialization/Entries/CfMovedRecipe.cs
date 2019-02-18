namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;

    internal class CfMovedRecipe : MovedRecipe, ICustomFabricatorEntry
    {
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
    }
}
