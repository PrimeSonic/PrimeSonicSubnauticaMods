namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>, ITutorialText
    {
        public CustomBioFuelList() : base("CustomBioFuels", new CustomBioFuel())
        {
        }

        public List<string> TutorialText =>
            new List<string>
            {
                "# Custom BioFuel Values #",
                "# Check the CustomBioFuels_Samples.txt file in the SampleFiles folder for original values and samples on how to modify bioreactor fuel values #",
                "# You'll also find all the default BioReactor values in the OriginalRecipes folder. #"
            };
    }
}
