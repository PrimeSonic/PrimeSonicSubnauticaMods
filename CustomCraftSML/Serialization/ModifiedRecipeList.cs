namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        public ModifiedRecipeList() : base("ModifiedRecipes", new ModifiedRecipe())
        {
        }
    }
}
