namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        internal const string ListKey = "AddedRecipes";

        public AddedRecipeList() : base(ListKey)
        {
        }
    }
}
