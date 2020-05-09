namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>
    {
        internal const string ListKey = "AliasRecipes";

        public AliasRecipeList() : base(ListKey)
        {
        }
    }
}
