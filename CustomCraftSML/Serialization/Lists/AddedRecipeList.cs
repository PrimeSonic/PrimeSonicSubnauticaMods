namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        public AddedRecipeList() : base("AddedRecipes", new AddedRecipe())
        {
        }
    }
}
