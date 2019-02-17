namespace CustomCraft2SML.Serialization.Entries
{
    using CustomCraft2SML.Interfaces;

    internal class CfCustomCraftingTab : CustomCraftingTab, ICustomFabricatorEntry
    {
        public CfCustomCraftingTab()
        {
            base.OnValueExtractedEvent -= ParsePath;
        }

        public CustomFabricator ParentFabricator { get; set; }
        
        protected override bool ValidFabricator()
        {
            return true;
        }
    }
}
