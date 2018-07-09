namespace CustomCraft2SML.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using EasyMarkup;

    public class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>, IEnumerable<ModifiedRecipe>
    {
        private const string KeyName = "ModifiedRecipes";

        public new ModifiedRecipe this[int index] => (ModifiedRecipe)base[index];

        public ModifiedRecipeList() : base(KeyName, new ModifiedRecipe(KeyName))
        {
        }

        public IEnumerator<ModifiedRecipe> GetEnumerator()
        {
            foreach (EmPropertyCollection item in Collections)
            {
                yield return (ModifiedRecipe)item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
