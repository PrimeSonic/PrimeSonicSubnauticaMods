namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;

    internal class CfAddedRecipe : AddedRecipe, ICustomFabricatorEntry
    {
        public CustomFabricator ParentFabricator { get; set; }

        protected override void HandleCraftTreeAddition()
        {
        }
    }
}
