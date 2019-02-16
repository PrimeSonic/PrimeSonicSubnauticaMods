namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;

    internal class CustomCraftingTab : EmPropertyCollection, ICraftingTab
    {
        internal static readonly string[] TutorialText = new[]
        {
            "CustomCraftingTab: Add your own custom tabs into the fabricator crafting trees. ",
            "An absolute must for organizing large numbers of crafts."
        };

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

        public CustomCraftingTab() : this(KeyName, CustomCraftingTabProperties)
        {
        }

        protected CustomCraftingTab(string key, ICollection<EmProperty> definitions) : base(key, definitions)
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

        private void ParsePath()
        {
            try
            {
                craftingPath = new CraftingPath(this.ParentTabPath);
            }
            catch
            {
                craftingPath = null;
            }
        }

        public string ID => FullPath;

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

        public string FullPath => $"{this.ParentTabPath}{"/"}{this.TabID}";

        internal override EmProperty Copy() => new CustomCraftingTab(this.Key, this.CopyDefinitions);

        public bool PassesPreValidation() => craftingPath != null && ValidFabricator();

        private bool ValidFabricator()
        {
            if (this.FabricatorType > CraftTree.Type.Rocket)
            {
                QuickLogger.Error($"Error on crafting tab '{this.TabID}'. This API in intended only for use with standard, non-modded CraftTree.Types.");
                return false;
            }

            if (this.FabricatorType == CraftTree.Type.None)
            {
                QuickLogger.Error($"Error on crafting tab '{this.TabID}'. ParentTabPath must identify a fabricator for the custom tab.");
                return false;
            }

            return true;
        }

        public bool SendToSMLHelper()
        {
            try
            {
                HandleCraftingTab();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling crafting tab '{this.TabID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        protected void HandleCraftingTab()
        {
            Atlas.Sprite sprite = GetCraftingTabSprite();

            if (this.StepsToTab == null)
            {
                CraftTreeHandler.AddTabNode(this.FabricatorType, this.TabID, this.DisplayName, sprite);
            }
            else
            {
                CraftTreeHandler.AddTabNode(this.FabricatorType, this.TabID, this.DisplayName, sprite, this.StepsToTab);
            }
        }

        protected Atlas.Sprite GetCraftingTabSprite()
        {
            Atlas.Sprite sprite;
            string imagePath = FileReaderWriter.AssetsFolder + this.TabID + @".png";
            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found for CraftingTab '{this.TabID}'");
                sprite = ImageUtils.LoadSpriteFromFile(imagePath);
            }
            else if (this.SpriteItemID != TechType.None)
            {
                QuickLogger.Message($"SpriteItemID used for CraftingTab '{this.TabID}'");
                sprite = SpriteManager.Get(this.SpriteItemID);
            }
            else
            {
                QuickLogger.Warning($"No sprite loaded for CraftingTab '{this.TabID}'");
                sprite = SpriteManager.Get(TechType.None);
            }

            return sprite;
        }
    }
}
