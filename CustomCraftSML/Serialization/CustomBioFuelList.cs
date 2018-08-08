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
                "# Check the OriginalBioFuelValues.txt file in the SampleFiles folder for origina values and samples on how to modify bioreactor fuel values #"
            };
    }
}
