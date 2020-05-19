namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomFragmentCountList : EmPropertyCollectionList<CustomFragmentCount>
    {
        internal const string ListKey = "CustomFragmentCounts";

        public CustomFragmentCountList() : base(ListKey)
        {
        }
    }
}
