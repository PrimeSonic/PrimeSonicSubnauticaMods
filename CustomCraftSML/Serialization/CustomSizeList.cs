namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>, ITutorialText
    {
        public CustomSizeList() : base("CustomSizes", new CustomSize())
        {
        }

        public List<string> TutorialText =>
            new List<string>
            {
                "# Custom Inventory Item Sizes #",
                "# Check the CustomSizes_Samples.txt file in the SampleFiles folder for details on how to set your own custom sizes #"
            };
    }
}
