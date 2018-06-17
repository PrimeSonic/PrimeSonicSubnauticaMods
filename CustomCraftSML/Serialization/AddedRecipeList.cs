namespace CustomCraftSML.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using EasyMarkup;

    public class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>, IEnumerable<AddedRecipe>
    {
        private const string KeyName = "AddedRecipes";

        public new AddedRecipe this[int index] => (AddedRecipe)base[index];

        public AddedRecipeList() : base(KeyName, new AddedRecipe(KeyName))
        {
        }

        public IEnumerator<AddedRecipe> GetEnumerator()
        {
            foreach (EmPropertyCollection item in Collections)
            {
                yield return (AddedRecipe)item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
