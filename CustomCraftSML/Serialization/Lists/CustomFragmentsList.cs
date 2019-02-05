namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomFragmentCountList : EmPropertyCollectionList<CustomFragmentCount>
    {
        internal const string ListKey = "CustomFragmentCounts";

        public CustomFragmentCountList() : base(ListKey, new CustomFragmentCount())
        {
        }
    }
}
