namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>, ITutorialText
    {
        public AddedRecipeList() : base("AddedRecipes", new AddedRecipe())
        {
        }

        public List<string> TutorialText => 
            new List<string> 
            {
                "# Added Recipes #",
                "# Check the AddedRecipes_Samples.txt file in the SampleFiles folder for details on how to add recipes for items normally not craftable #"
            };
    }
}
