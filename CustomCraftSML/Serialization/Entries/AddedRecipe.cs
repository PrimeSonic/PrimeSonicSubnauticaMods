namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
        protected const string PathKey = "Path";
        protected const string PdaGroupKey = "PdaGroup";
        protected const string PdaCategoryKey = "PdaCategory";

        internal static new readonly string[] TutorialText = new[]
        {
           $"{AddedRecipeList.ListKey}: Adding your own recipes into any of the existing fabricators.",
           $"    {AddedRecipeList.ListKey} have all the same properties as {ModifiedRecipeList.ListKey}, with the following additions:",
           $"    {PathKey}: Sets the fabricator and crafting tab where the new recipe will be added.",
           $"    {PdaGroupKey}: Sets the main group for blueprint shown in the PDA.",
           $"        This is optional. If {PdaGroupKey} is set, {PdaCategoryKey} must also be set.",
           $"    {PdaCategoryKey}: Sets the category under the group for blueprint shown in the PDA.",
           $"        This is optional. If {PdaCategoryKey} is set, {PdaGroupKey} must also be set."
        };

        protected readonly EmProperty<string> path;
        protected readonly EmProperty<TechGroup> techGroup;
        protected readonly EmProperty<TechCategory> techCategory;

        public string Path
        {
            get => path.Value;
            set => path.Value = value;
        }

        protected static List<EmProperty> AddedRecipeProperties => new List<EmProperty>(ModifiedRecipeProperties)
        {
            new EmProperty<string>(PathKey),
            new EmProperty<TechGroup>(PdaGroupKey) { Optional = true },
            new EmProperty<TechCategory>(PdaCategoryKey) { Optional = true }
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
            path = (EmProperty<string>)Properties[PathKey];
            techGroup = (EmProperty<TechGroup>)Properties[PdaGroupKey];
            techCategory = (EmProperty<TechCategory>)Properties[PdaCategoryKey];
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

            foreach (string linkedItem in this.LinkedItemIDs)
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
            var craftPath = new CraftingPath(this.Path, this.ItemID);

            AddCraftNode(craftPath, this.TechType);
        }
    }
}
