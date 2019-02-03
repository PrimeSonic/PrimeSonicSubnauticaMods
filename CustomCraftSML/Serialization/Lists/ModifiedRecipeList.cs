namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        public ModifiedRecipeList() : base("ModifiedRecipes", new ModifiedRecipe())
        {
        }
    }
}
