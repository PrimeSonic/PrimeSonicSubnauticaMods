namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        internal const string ListKey = "AddedRecipes";

        public AddedRecipeList() : base(ListKey)
        {
        }
    }
}
