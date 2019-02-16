namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
        internal static new readonly string[] TutorialText = new[]
        {
            "AddedRecipe: Adding your own recipes into any of the existing fabricators.",
            "    Added recipes have all the same properties as Modified recipes, with the following additions:",
            "    Path: Sets the fabricator and crafting tab where the new recipe will be added.",
            "    PdaGroup: Sets the main group for blueprint shown in the PDA.",
            "    PdaCategory: Sets the category under the group for blueprint shown in the PDA."
        };

        private readonly EmProperty<string> path;
        private readonly EmProperty<TechGroup> techGroup;
        private readonly EmProperty<TechCategory> techCategory;

        public string Path
        {
            get => path.Value;
            set => path.Value = value;
        }

        protected static List<EmProperty> AddedRecipeProperties => new List<EmProperty>(ModifiedRecipeProperties)
        {
            new EmProperty<string>("Path"),
            new EmProperty<TechGroup>("PdaGroup") { Optional = true },
            new EmProperty<TechCategory>("PdaCategory") { Optional = true }
        };

        public TechGroup PdaGroup
        {
            get => techGroup.Value;
            set => techGroup.Value = value;
        }

        public TechCategory PdaCategory
        {
            get => techCategory.Value;
            set => techCategory.Value = value;
        }

        public AddedRecipe() : this("AddedRecipe", AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key) : this(key, AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            path = (EmProperty<string>)Properties["Path"];
            techGroup = (EmProperty<TechGroup>)Properties["PdaGroup"];
            techCategory = (EmProperty<TechCategory>)Properties["PdaCategory"];
            DefaultForceUnlock = true;
            this.PdaGroup = TechGroup.Uncategorized;
            this.PdaCategory = TechCategory.Misc;

        }

        internal override EmProperty Copy() => new AddedRecipe(this.Key, this.CopyDefinitions);

        public override bool SendToSMLHelper()
        {
            try
            {
                HandleAddedRecipe();

                HandleCraftTreeAddition();

                HandleUnlocks();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Added Recipe '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        protected void HandleAddedRecipe(short defaultCraftAmount = 1)
        {
            var replacement = new TechData
            {
                craftAmount = this.AmountCrafted ?? defaultCraftAmount
            };

            foreach (EmIngredient ingredient in this.Ingredients)
                replacement.Ingredients.Add(new Ingredient(GetTechType(ingredient.ItemID), ingredient.Required));

            foreach (string linkedItem in this.LinkedItems)
                replacement.LinkedItems.Add(GetTechType(linkedItem));

            TechType itemID = GetTechType(this.ItemID);

            CraftDataHandler.SetTechData(itemID, replacement);
            QuickLogger.Message($"Adding new recipe for '{this.ItemID}'");

            if (this.PdaGroup != TechGroup.Uncategorized)
            {
                CraftDataHandler.AddToGroup(this.PdaGroup, this.PdaCategory, itemID);
                // SMLHelper logs enough here
            }
        }

        protected void HandleCraftTreeAddition()
        {
            var craftPath = new CraftingPath(this.Path);

            AddCraftNode(craftPath, this.TechType);
        }
    }
}
