namespace CustomCraft2SML.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>, IEnumerable<ModifiedRecipe>
    {
        private const string KeyName = "ModifiedRecipes";

        public new ModifiedRecipe this[int index] => base[index];

        internal ModifiedRecipeList(string keyOverride) : base(keyOverride, new ModifiedRecipe(KeyName))
        {
        }

        public ModifiedRecipeList() : base(KeyName, new ModifiedRecipe(KeyName))
        {
        }

        public new IEnumerator<ModifiedRecipe> GetEnumerator()
        {
            foreach (EmPropertyCollection item in InternalValues)
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
