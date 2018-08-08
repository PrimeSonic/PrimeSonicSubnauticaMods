namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>, ITutorialText
    {
        public ModifiedRecipeList() : base("ModifiedRecipes", new ModifiedRecipe())
        {
        }

        public List<string> TutorialText => 
            new List<string>
            {
                "# Modified Recipes #",
                "# Check the ModifiedRecipes_Samples.txt file in the SampleFiles folder for details on how to alter existing crafting recipes #",
                "# You'll also find all the existing recipes in the OriginalRecipes folder. #"
            };
    }
}
