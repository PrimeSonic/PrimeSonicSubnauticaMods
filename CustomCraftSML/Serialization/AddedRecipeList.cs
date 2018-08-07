namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    public class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        public AddedRecipeList() : base("AddedRecipes", new AddedRecipe())
        {
        }
    }
}
