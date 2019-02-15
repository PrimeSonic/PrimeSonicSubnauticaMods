namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>
    {
        internal const string ListKey = "AliasRecipes";

        public AliasRecipeList() : base(ListKey, new AliasRecipe())
        {
        }
    }
}
