namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>
    {
        public CustomBioFuelList() : base("CustomBioFuels", new CustomBioFuel())
        {
        }
    }
}
