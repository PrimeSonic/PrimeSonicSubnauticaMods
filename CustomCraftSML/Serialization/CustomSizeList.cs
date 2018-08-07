namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    public class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        public CustomSizeList() : base("CustomSizes", new CustomSize())
        {
        }
    }
}
