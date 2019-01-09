namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;

    internal class CustomCraftingTab : EmPropertyCollection, ICraftingTab
    {
        private const string KeyName = "CustomTab";

        private readonly EmProperty<string> emTabID;
        private readonly EmProperty<string> emDisplayName;
        private readonly EmProperty<TechType> emSpriteID;
        private readonly EmProperty<string> emParentTabPath;
        private CraftingPath craftingPath;

        protected static ICollection<EmProperty> CustomCraftingTabProperties => new List<EmProperty>(4)
        {
            new EmProperty<string>("TabID"),
            new EmProperty<string>("DisplayName"),
            new EmProperty<TechType>("SpriteItemID"),
            new EmProperty<string>("ParentTabPath"),
        };

        public CustomCraftingTab() : base(KeyName, CustomCraftingTabProperties)
        {
            emTabID = (EmProperty<string>)Properties["TabID"];
            emDisplayName = (EmProperty<string>)Properties["DisplayName"];
            emSpriteID = (EmProperty<TechType>)Properties["SpriteItemID"];
            emParentTabPath = (EmProperty<string>)Properties["ParentTabPath"];

            base.OnValueExtractedEvent += ParsePath;
        }

        internal CustomCraftingTab(string path) : this()
        {
            this.ParentTabPath = path;
            ParsePath();
        }

        private void ParsePath() => craftingPath = new CraftingPath(this.ParentTabPath);

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
            get
            {
                if (craftingPath is null)
                    return CraftTree.Type.None;

                return craftingPath.Scheme;
            }
        }

        public TechType SpriteItemID
        {
            get => emSpriteID.Value;
            set => emSpriteID.Value = value;
        }

        public string ParentTabPath
        {
            get => emParentTabPath.Value;
            set => emParentTabPath.Value = value;
        }
        public string[] StepsToTab
        {
            get
            {
                if (craftingPath is null)
                    return null;

                return craftingPath.Steps;
            }
        }

        internal override EmProperty Copy() => new CustomCraftingTab();
    }
}
