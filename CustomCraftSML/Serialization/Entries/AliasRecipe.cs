namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

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
            new EmProperty<string>("FunctionalID"),
            new EmProperty<TechType>("SpriteItemID")
        };

        public AliasRecipe() : this(AliasRecipeProperties)
        {
        }

        protected AliasRecipe(ICollection<EmProperty> definitions) : base("AliasRecipe", definitions)
        {
            displayName = (EmProperty<string>)Properties["DisplayName"];
            tooltip = (EmProperty<string>)Properties["Tooltip"];
            functionalID = (EmProperty<string>)Properties["FunctionalID"];
            spriteID = (EmProperty<TechType>)Properties["SpriteItemID"];
        }

        internal override EmProperty Copy() => new AliasRecipe(this.CopyDefinitions);
    }
}
