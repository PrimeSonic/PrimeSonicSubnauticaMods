namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    public class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        internal ModifiedRecipeList() : base("ModifiedRecipes", new ModifiedRecipe())
        {
        }
    }
}
