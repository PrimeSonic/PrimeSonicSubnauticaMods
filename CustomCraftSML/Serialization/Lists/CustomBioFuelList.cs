namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>
    {
        internal const string ListKey = "CustomBioFuels";

        public CustomBioFuelList() : base(ListKey)
        {
        }
    }
}
