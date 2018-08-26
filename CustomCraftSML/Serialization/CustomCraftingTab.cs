namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;


    internal class CustomCraftingTab : EmPropertyCollection, ICraftingTab
    {
        private const string KeyName = "CustomTab";

        private readonly EmProperty<string> emTabID;
        private readonly EmProperty<string> emDisplayName;
        private readonly EmPropertyTechType emSpriteID;
        private readonly EmPropertyCraftTreeType emTreeType;
        private readonly EmProperty<string> emParentTabID;

        protected static ICollection<EmProperty> CustomCraftingTabProperties => new List<EmProperty>(4)
        {
            new EmProperty<string>("TabID"),
            new EmProperty<string>("DisplayName"),
            new EmPropertyTechType("ItemForSprite"),
            new EmPropertyCraftTreeType("FabricatorType"),
            new EmProperty<string>("ParentTabID"),
        };

        public CustomCraftingTab() : base(KeyName, CustomCraftingTabProperties)
        {
            emTabID = (EmProperty<string>)Properties["TabID"];
            emDisplayName = (EmProperty<string>)Properties["DisplayName"];
            emSpriteID = (EmPropertyTechType)Properties["ItemForSprite"];
            emTreeType = (EmPropertyCraftTreeType)Properties["FabricatorType"];
            emParentTabID = (EmProperty<string>)Properties["ParentTabID"];
        }

        public string TabID
        {
            get => emTabID.Value;
            set => emTabID.Value = value;
        }

        public string DisplayName
        {
            get => emDisplayName.Value;
            set => emDisplayName.Value = value;
        }

        public CraftTree.Type FabricatorType
        {
            get => emTreeType.Value;
            set => emTreeType.Value = value;
        }

        public TechType ItemForSprite
        {
            get => emSpriteID.Value;
            set => emSpriteID.Value = value;
        }

        public string ParentTabID
        {
            get => emParentTabID.Value;
            set => emParentTabID.Value = value;
        }

        internal override EmProperty Copy() => new CustomCraftingTab();
    }
}
