namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>, ITutorialText
    {
        public AliasRecipeList() : base("AliasRecipes", new AliasRecipe())
        {
        }

        public List<string> TutorialText =>
            new List<string>
            {
                "# Added Recipes #",
                "# Check the AliasRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #"
            };
    }
}
