namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;

    internal class CfAliasRecipe : AliasRecipe, ICustomFabricatorEntry
    {
        public CustomFabricator ParentFabricator { get; set; }

        protected override void HandleCraftTreeAddition()
        {
        }
    }
}
