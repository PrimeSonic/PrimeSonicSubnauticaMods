namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        internal const string ListKey = "ModifiedRecipes";

        public ModifiedRecipeList() : base(ListKey)
        {
        }
    }
}
