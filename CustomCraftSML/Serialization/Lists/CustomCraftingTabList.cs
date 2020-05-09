namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomCraftingTabList : EmPropertyCollectionList<CustomCraftingTab>
    {
        internal const string ListKey = "CustomCraftingTabs";

        public CustomCraftingTabList() : base(ListKey)
        {
        }
    }
}
