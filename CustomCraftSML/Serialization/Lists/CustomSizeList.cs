namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        public CustomSizeList() : base("CustomSizes", new CustomSize())
        {
        }
    }
}
