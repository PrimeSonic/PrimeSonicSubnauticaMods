namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        internal const string ListKey = "CustomSizes";

        public CustomSizeList() : base(ListKey, new CustomSize())
        {
        }
    }
}
