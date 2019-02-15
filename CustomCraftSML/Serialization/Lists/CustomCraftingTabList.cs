namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomCraftingTabList : EmPropertyCollectionList<CustomCraftingTab>
    {
        internal const string ListKey = "CustomCraftingTabs";

        public CustomCraftingTabList() : base(ListKey, new CustomCraftingTab())
        {
        }
    }
}
