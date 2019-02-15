namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        internal const string ListKey = "ModifiedRecipes";

        public ModifiedRecipeList() : base(ListKey, new ModifiedRecipe())
        {
        }
    }
}
