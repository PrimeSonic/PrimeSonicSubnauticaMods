namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>
    {
        internal const string ListKey = "CustomBioFuels";

        public CustomBioFuelList() : base(ListKey, new CustomBioFuel())
        {
        }
    }
}
