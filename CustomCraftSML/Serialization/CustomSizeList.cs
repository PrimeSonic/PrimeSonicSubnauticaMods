namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        public CustomSizeList() : base("CustomSizes", new CustomSize())
        {
        }
    }
}
