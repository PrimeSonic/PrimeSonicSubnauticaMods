namespace CustomCraftSML.Serialization
{
    using EasyMarkup;

    public class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        private const string KeyName = "ModifiedRecipes";

        public new ModifiedRecipe this[int index] => (ModifiedRecipe)base[index];

        public ModifiedRecipeList() : base(KeyName, new ModifiedRecipe(KeyName))
        {
        }
    }
}
