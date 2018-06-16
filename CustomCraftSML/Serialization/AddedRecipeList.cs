namespace CustomCraftSML.Serialization
{
    using EasyMarkup;

    public class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        private const string KeyName = "AddedRecipes";

        public new AddedRecipe this[int index] => (AddedRecipe)base[index];

        public AddedRecipeList() : base(KeyName, new AddedRecipe(KeyName))
        {
        }
    }
}
