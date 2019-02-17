namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;

    internal class CfMovedRecipe : MovedRecipe, ICustomFabricatorEntry
    {
        public CustomFabricator ParentFabricator { get; set; }

        protected override void HandleCraftTreeAddition()
        {
        }
    }
}
