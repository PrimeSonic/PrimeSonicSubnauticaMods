namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using IOPath = System.IO.Path;

    internal class AliasRecipe : AddedRecipe, IAliasRecipe
    {
        protected const string DisplayNameKey = "DisplayName";
        protected const string ToolTipKey = "Tooltip";
        protected const string FunctionalIdKey = "FunctionalID";
        protected const string SpriteItemIdKey = "SpriteItemID";

        public new const string TypeName = "AliasRecipe";

        internal static new readonly string[] TutorialText = new[]
        {
           $"{AliasRecipeList.ListKey}: A powerful tool with multiple applications.",
            "    Alias recipes allow you to create multiple ways to craft the same item, bypassing one of the limitations of Subnautica's crafting system.",
            "    Alias recipes can also be used to add your own custom items into the game, all without any coding, with some limitations.",
           $"    Alias recipes should NOT include an {AmountCraftedKey} value. {LinkedItemsIdsKey} should be used instead to define the produce being crafted.",
           $"    {AliasRecipeList.ListKey} have all the same properties of {AddedRecipeList.ListKey}, but when creating your own items, you will want to include these new properties:",
           $"        {DisplayNameKey}: Sets the in-game name for the new item.",
           $"        {ToolTipKey}: Sets the in-game tooltip text whenever you hover over the item.",
           $"        {FunctionalIdKey}: Choose an existing item in the game and clone that item's in-game functions into your custom item.",
            "            Without this property, any user created item will be non-functional in-game, usable as a crafting component but otherwise useful for nothing else.",
           $"        {SpriteItemIdKey}: Use the in-game sprite of an existing item for your custom item.",
           $"            This option will be used only if a png file matching the {ItemIdKey} isn't found in the Assets folder.",
            "            If no file is found with that name, the sprite for the first LinkedItem will be used instead.",
            "            This should only be used with non-modded item values.",
        };

        protected readonly EmProperty<string> displayName;
        protected readonly EmProperty<string> tooltip;
        protected readonly EmProperty<string> functionalID;
        protected readonly EmProperty<TechType> spriteID;

        public string DisplayName
        {
            get => displayName.Value;
            set => displayName.Value = value;
        }

        public string Tooltip
        {
            get => tooltip.Value;
            set => tooltip.Value = value;
        }

        public string FunctionalID
        {
            get => functionalID.Value;
            set => functionalID.Value = value;
        }

        public TechType SpriteItemID
        {
            get => spriteID.Value;
            set => spriteID.Value = value;
        }

        public TechType FunctionalCloneID { get; private set; }

        protected static List<EmProperty> AliasRecipeProperties => new List<EmProperty>(AddedRecipeProperties)
        {
            new EmProperty<string>(DisplayNameKey),
            new EmProperty<string>(ToolTipKey),
            new EmProperty<string>(FunctionalIdKey) { Optional = true },
            new EmProperty<TechType>(SpriteItemIdKey) { Optional = true }
        };

        public AliasRecipe() : this(TypeName, AliasRecipeProperties)
        {
        }

        protected AliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            displayName = (EmProperty<string>)Properties[DisplayNameKey];
            tooltip = (EmProperty<string>)Properties[ToolTipKey];
            functionalID = (EmProperty<string>)Properties[FunctionalIdKey];
            spriteID = (EmProperty<TechType>)Properties[SpriteItemIdKey];

            amountCrafted.DefaultValue = 0;
        }

        internal override EmProperty Copy() => new AliasRecipe(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation()
        {
            // Alias Recipes must request their techtype be added
            this.TechType = TechTypeHandler.AddTechType(this.ItemID, this.DisplayName, this.Tooltip, this.ForceUnlockAtStart);

            return InnerItemsAreValid() && FunctionalItemIsValid();
        }

        protected virtual bool FunctionalItemIsValid()
        {
            if (string.IsNullOrEmpty(this.FunctionalID))
                return true; // No value provided. This is fine.

            // The functional item for cloning must be valid.
            FunctionalCloneID = GetTechType(this.FunctionalID);

            if (FunctionalCloneID == TechType.None)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {FunctionalIdKey} value '{this.FunctionalID}'. Entry will be discarded.");
                return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            try
            {
                //  See if there is an asset in the asset folder that has the same name
                HandleCustomSprite();

                // Alias recipes should default to not producing the custom item unless explicitly configured
                HandleAddedRecipe(0);

                HandleCraftTreeAddition();

                HandleUnlocks();

                HandleFunctionalClone();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileReaderWriter.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Message($"{SpriteItemIdKey} '{this.SpriteItemID}' used for {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = SpriteManager.Get(this.SpriteItemID);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.LinkedItems.Count > 0)
            {
                QuickLogger.Message($"First entry in {LinkedItemsIdsKey} used for icon of {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = SpriteManager.Get(this.LinkedItems[0]);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.ItemID}' from {this.Origin}");
        }

        protected void HandleFunctionalClone()
        {
            if (FunctionalCloneID != TechType.None)
            {
                var clone = new FunctionalClone(this, FunctionalCloneID);
                PrefabHandler.RegisterPrefab(clone);
                QuickLogger.Message($"Custom item '{this.ItemID}' will be a functional clone of '{this.FunctionalID}' - Entry from {this.Origin}");
            }
        }
    }
}
