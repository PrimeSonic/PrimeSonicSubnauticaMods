namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>
    {
        public AliasRecipeList() : base("AliasRecipes", new AliasRecipe())
        {
        }
    }
}
