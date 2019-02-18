namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using SMLHelper.V2.Crafting;

    internal class CfAddedRecipe : AddedRecipe, ICustomFabCraftingNode
    {
        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.BuildableFabricator.TreeTypeID;

        public ModCraftTreeRoot RootNode => this.ParentFabricator.BuildableFabricator.RootNode;

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
    }
}
