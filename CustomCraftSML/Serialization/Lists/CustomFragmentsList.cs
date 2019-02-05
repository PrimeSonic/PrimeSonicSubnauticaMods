namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomFragmentCountList : EmPropertyCollectionList<CustomFragmentCount>
    {
        public CustomFragmentCountList() : base("CustomFragmentCounts", new CustomFragmentCount())
        {
        }
    }
}
