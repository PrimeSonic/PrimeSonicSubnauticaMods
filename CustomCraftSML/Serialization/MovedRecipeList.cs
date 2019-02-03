namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class MovedRecipeList : EmPropertyCollectionList<MovedRecipe>
    {
        public MovedRecipeList() : base("MovedRecipes", new MovedRecipe())
        {
        }
    }
}
