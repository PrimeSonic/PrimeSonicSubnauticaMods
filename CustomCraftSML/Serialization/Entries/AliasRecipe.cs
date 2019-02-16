namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;

    internal class AliasRecipe : AddedRecipe, IAliasRecipe
    {
        internal static new readonly string[] TutorialText = new[]
        {
            "AliasRecipe: A powerful tool with multiple applications.",
            "    Alias recipes allow you to create multiple ways to craft the same item,",
            "      bypassing one of the limitations of Subnautica's crafting system.",
            "    Alias recipes can also be used to add your own custom items into the game, all without any coding, with some limitations.",
            "    Alias recipes have all the same properties of Added Recipes, but when creating your own items, you will want to include these new properties:",
            "        DisplayName: Sets the in-game name for the new item.",
            "        Tooltip: Sets the in-game tooltip text whenever you hover over the item.",
            "        FunctionalID: Choose an existing item in the game and clone that item's in-game functions into your custom item.",
            "            Without this property, any user created item will be non-functional in-game, usable as a crafting component but otherwise useful for nothing else.",
            "        SpriteItemID: Use the in-game sprite of an existing item for your custom item.",
            "            Without this property, the in-game sprite for your item will be determined by a png file in the Assets folder matching its ItemID.",
            "            If no file is found with that name, the sprite for the first LinkedItem will be used instead.",
        };

        private readonly EmProperty<string> displayName;
        private readonly EmProperty<string> tooltip;
        private readonly EmProperty<string> functionalID;
        private readonly EmProperty<TechType> spriteID;

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

        protected static List<EmProperty> AliasRecipeProperties => new List<EmProperty>(AddedRecipeProperties)
        {
            new EmProperty<string>("DisplayName"),
            new EmProperty<string>("Tooltip"),
            new EmProperty<string>("FunctionalID") { Optional = true },
            new EmProperty<TechType>("SpriteItemID") { Optional = true }
        };

        public AliasRecipe() : this("AliasRecipe", AliasRecipeProperties)
        {
        }

        protected AliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            displayName = (EmProperty<string>)Properties["DisplayName"];
            tooltip = (EmProperty<string>)Properties["Tooltip"];
            functionalID = (EmProperty<string>)Properties["FunctionalID"];
            spriteID = (EmProperty<TechType>)Properties["SpriteItemID"];
        }

        internal override EmProperty Copy() => new AliasRecipe(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation()
        {
            this.TechType = TechTypeHandler.AddTechType(this.ItemID, this.DisplayName, this.Tooltip, this.ForceUnlockAtStart);

            return InnerItemsAreValid() && FunctionalItemIsValid();
        }

        private bool FunctionalItemIsValid()
        {
            if (string.IsNullOrEmpty(this.FunctionalID))
                return true; // No value provided. This is fine.

            // The functional item for cloning must be valid.
            TechType functionalCloneId = GetTechType(this.FunctionalID);
            if (functionalCloneId == TechType.None)
            {
                QuickLogger.Warning($"Entry with FunctionalID of '{this.ItemID}' contained an unknown item of '{this.FunctionalID}'.  Entry will be discarded.");
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
                QuickLogger.Error($"Exception thrown while handling Alias Recipe '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        protected void HandleCustomSprite()
        {
            if (this.SpriteItemID > TechType.None)
            {
                QuickLogger.Message($"SpriteItemID {this.SpriteItemID} used for AliasRecipe '{this.ItemID}'");
                Atlas.Sprite sprite = SpriteManager.Get(this.SpriteItemID);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            string imagePath = FileReaderWriter.AssetsFolder + this.ItemID + @".png";
            if (File.Exists(imagePath))
            {
                QuickLogger.Message($"Custom sprite found for AliasRecipe '{this.ItemID}'");
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.LinkedItemsCount > 0)
            {
                QuickLogger.Message($"First LinkedItemID used for icon of AliasRecipe '{this.ItemID}'");
                Atlas.Sprite sprite = SpriteManager.Get(GetTechType(GetLinkedItem(0)));
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            QuickLogger.Warning($"No sprite loaded for '{this.ItemID}'");
        }

        protected void HandleFunctionalClone()
        {
            if (string.IsNullOrEmpty(this.FunctionalID))
                return; // No value provided. This is fine.

            TechType functionalID = GetTechType(this.FunctionalID);

            if (functionalID != TechType.None)
            {
                var clone = new FunctionalClone(this, functionalID);
                PrefabHandler.RegisterPrefab(clone);
                QuickLogger.Message($"Custom item '{this.ItemID}' will be a functional clone of '{this.FunctionalID}'");
            }
        }
    }
}
