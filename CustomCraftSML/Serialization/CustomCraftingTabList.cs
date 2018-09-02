namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    internal class CustomCraftingTabList : EmPropertyCollectionList<CustomCraftingTab>
    {
        public CustomCraftingTabList() : base("CustomCraftingTabs", new CustomCraftingTab())
        {
        }
    }
}
