namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class MovedRecipeList : EmPropertyCollectionList<MovedRecipe>
    {
        internal const string ListKey = "MovedRecipes";

        public MovedRecipeList() : base(ListKey)
        {
        }
    }
}
