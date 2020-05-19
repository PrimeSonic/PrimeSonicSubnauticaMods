namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        internal const string ListKey = "CustomSizes";

        public CustomSizeList() : base(ListKey)
        {
        }
    }
}
