namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>
    {
        public AliasRecipeList() : base("AliasRecipes", new AliasRecipe())
        {
        }
    }
}
