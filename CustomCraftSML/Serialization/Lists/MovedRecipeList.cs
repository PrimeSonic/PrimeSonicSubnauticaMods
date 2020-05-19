namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class MovedRecipeList : EmPropertyCollectionList<MovedRecipe>
    {
        internal const string ListKey = "MovedRecipes";

        public MovedRecipeList() : base(ListKey)
        {
        }
    }
}
