namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>
    {
        public CustomBioFuelList() : base("CustomBioFuels", new CustomBioFuel())
        {
        }
    }
}
