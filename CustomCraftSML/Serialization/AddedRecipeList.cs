namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        public AddedRecipeList() : base("AddedRecipes", new AddedRecipe())
        {
        }
    }
}
