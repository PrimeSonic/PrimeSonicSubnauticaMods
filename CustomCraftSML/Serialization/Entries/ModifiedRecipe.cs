namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class ModifiedRecipe : EmTechTyped, IModifiedRecipe, ICustomCraft
    {
        public virtual string[] TutorialText => ModifiedRecipeTutorial;

        internal static readonly string[] ModifiedRecipeTutorial = new[]
        {
           $"{ModifiedRecipeList.ListKey}: Modify an existing crafting recipe. ",
           $"    {IngredientsKey}: Completely replace a recipe's required ingredients.",
            "        This is optional if you don't want to change the required ingredients.",
           $"    {AmountCraftedKey}: Change how many copies of the item are created when you craft the recipe.",
            "        This is optional if you don't want to change how many copies of the item are created at once.",
           $"    {LinkedItemsIdsKey}: Add or modify the linked items that are created when the recipe is crafted.",
            "        This is optional if you don't want to change what items are crafted with this one.",
           $"    {UnlocksKey}: Set other recipes to be unlocked when you analyze or craft this one.",
            "        This is optional if you don't want to change what gets unlocked when you scan or craft this item.",
           $"    {ForceUnlockAtStartKey}: You can also set if this recipe should be unlocked at the start or not. Make sure you have a recipe unlocking this one.",
            "        This is optional. For Added Recipes, this defaults to 'YES'.",
            $"    {UnlockedByKey}: Set this recipe to be unlocked when any one of the items listed here gets unlocked.",
            "        This is optional. If this is for an existing recipe, note that the original unlocks will not be affected.",
        };

        protected const string AmountCraftedKey = "AmountCrafted";
        protected const string IngredientsKey = "Ingredients";
        protected const string LinkedItemsIdsKey = "LinkedItemIDs";
        protected const string ForceUnlockAtStartKey = "ForceUnlockAtStart";
        protected const string UnlocksKey = "Unlocks";
        protected const string UnlockedByKey = "UnlockedBy";

        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyList<string> linkedItems;
        protected readonly EmYesNo unlockedAtStart;
        protected readonly EmPropertyList<string> unlocks;
        protected readonly EmPropertyList<string> unlockedBy;

        public string ID => this.ItemID;

        public short? AmountCrafted
        {
            get
            {
                if (amountCrafted.HasValue && amountCrafted.Value > -1)
                    return amountCrafted.Value;

                return null;
            }
            set => amountCrafted.Value = value.Value;
        }

        protected bool DefaultForceUnlock = false;

        public bool ForceUnlockAtStart
        {
            get
            {
                if (unlockedAtStart.HasValue)
                    return unlockedAtStart.Value;

                return DefaultForceUnlock;
            }

            set => unlockedAtStart.Value = value;
        }

        public IList<string> Unlocks => unlocks.Values;
        protected List<TechType> UnlockingItems { get; } = new List<TechType>();

        public IList<string> UnlockedBy => unlockedBy.Values;
        protected List<TechType> UnlockedByItems { get; } = new List<TechType>();

        public IList<EmIngredient> Ingredients => ingredients.Values;
        protected List<Ingredient> SMLHelperIngredients { get; } = new List<Ingredient>();

        public IList<string> LinkedItemIDs => linkedItems.Values;
        protected List<TechType> LinkedItems { get; } = new List<TechType>();

        protected static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(AmountCraftedKey, 1) { Optional = true },
            new EmPropertyCollectionList<EmIngredient>(IngredientsKey) { Optional = true },
            new EmPropertyList<string>(LinkedItemsIdsKey) { Optional = true },
            new EmYesNo(ForceUnlockAtStartKey) { Optional = true },
            new EmPropertyList<string>(UnlocksKey) { Optional = true },
            new EmPropertyList<string>(UnlockedByKey) { Optional = true },
        };

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation { get; internal set; } = true;

        internal ModifiedRecipe(TechType origTechType) : this()
        {
#if SUBNAUTICA
            TechData origRecipe = CraftDataHandler.GetTechData(origTechType);
#elif BELOWZERO
            RecipeData origRecipe = CraftDataHandler.GetRecipeData(origTechType);
#endif
            this.ItemID = origTechType.ToString();
            this.AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                Ingredient origIngredient = (Ingredient)origRecipe.GetIngredient(i);
                this.Ingredients.Add(new EmIngredient(origIngredient.techType, (short)origIngredient.amount));
            }

            for (int i = 0; i < origRecipe.linkedItemCount; i++)
                linkedItems.Add(origRecipe.GetLinkedItem(i).ToString());
        }

        public ModifiedRecipe() : this("ModifiedRecipe", ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key) : this(key, ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            amountCrafted = (EmProperty<short>)Properties[AmountCraftedKey];
            ingredients = (EmPropertyCollectionList<EmIngredient>)Properties[IngredientsKey];
            linkedItems = (EmPropertyList<string>)Properties[LinkedItemsIdsKey];
            unlockedAtStart = (EmYesNo)Properties[ForceUnlockAtStartKey];
            unlocks = (EmPropertyList<string>)Properties[UnlocksKey];
            unlockedBy = (EmPropertyList<string>)Properties[UnlockedByKey];

            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (EmIngredient ingredient in ingredients)
            {
                string itemID = (ingredient["ItemID"] as EmProperty<string>).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;
            }
        }

        internal override EmProperty Copy()
        {
            return new ModifiedRecipe(this.Key, this.CopyDefinitions);
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return base.PassesPreValidation(originFile) & InnerItemsAreValid();
        }

        protected bool InnerItemsAreValid()
        {
            // Sanity check of the blueprints ingredients and linked items to be sure that it only contains known items
            // Modded items are okay, but they must be for mods the player already has installed
            return ValidateIngredients() &
                   ValidateLinkedItems() &
                   ValidateUnlocks() &
                   ValidateUnlockedBy();
        }

        protected bool ValidateUnlockedBy()
        {
            bool unlockedByValid = true;

            foreach (string unlockedBy in this.UnlockedBy)
            {
                TechType unlockByItemID = GetTechType(unlockedBy);

                if (unlockByItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {this.UnlockedBy} '{unlockedBy}'. Entry will be discarded.");
                    unlockedByValid = false;
                    continue;
                }

                this.UnlockedByItems.Add(unlockByItemID);
            }

            return unlockedByValid;
        }

        protected bool ValidateUnlocks()
        {
            bool unlocksValid = true;

            foreach (string unlockingItem in this.Unlocks)
            {
                TechType unlockingItemID = GetTechType(unlockingItem);

                if (unlockingItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {UnlocksKey} '{unlockingItem}'. Entry will be discarded.");
                    unlocksValid = false;
                    continue;
                }

                this.UnlockingItems.Add(unlockingItemID);
            }

            return unlocksValid;
        }

        protected bool ValidateLinkedItems()
        {
            bool linkedItemsValid = true;

            foreach (string linkedItem in this.LinkedItemIDs)
            {
                TechType linkedItemID = GetTechType(linkedItem);

                if (linkedItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {LinkedItemsIdsKey} '{linkedItem}'. Entry will be discarded.");
                    linkedItemsValid = false;
                    continue;
                }

                this.LinkedItems.Add(linkedItemID);
            }

            return linkedItemsValid;
        }

        protected bool ValidateIngredients()
        {
            bool ingredientsValid = true;

            foreach (EmIngredient ingredient in this.Ingredients)
            {
                if (ingredient.PassesPreValidation(this.Origin))
                    this.SMLHelperIngredients.Add(ingredient.ToSMLHelperIngredient());
                else
                    ingredientsValid = false;
            }

            return ingredientsValid;
        }

        public virtual bool SendToSMLHelper()
        {
            try
            {
                return
                    HandleModifiedRecipe() &&
                    HandleUnlocks();
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected bool HandleModifiedRecipe()
        {
#if SUBNAUTICA
            RecipeData original = CraftDataHandler.GetTechData(this.TechType);
#elif BELOWZERO
            RecipeData original = CraftDataHandler.GetRecipeData(this.TechType);
#endif

            if (original == null)
            {
                QuickLogger.Warning($"Original recipe for '{this.ItemID}' from {this.Origin} could not be found for modification. Entry will be discarded.");
                return false;  // Unknown recipe
            }

            var replacement = new RecipeData();

            bool overrideRecipe = false;
            string changes = "";
            // Amount
            if (this.AmountCrafted.HasValue)
            {
                overrideRecipe |= true;
                changes += $" {AmountCraftedKey} ";
                replacement.craftAmount = this.AmountCrafted.Value;
            }
            else
            {
                replacement.craftAmount = original.craftAmount;
            }

            // Ingredients
            if (this.Ingredients.Count > 0)
            {
                overrideRecipe |= true;
                changes += $" {IngredientsKey} ";
                replacement.Ingredients = this.SMLHelperIngredients;
            }
            else
            {
                // Copy original ingredients
                for (int i = 0; i < original.ingredientCount; i++)
                {
                    replacement.Ingredients.Add(
                        new Ingredient(
                        original.GetIngredient(i).techType,
                        original.GetIngredient(i).amount));
                }
            }

            // Linked Items
            if (this.LinkedItems.Count > 0)
            {
                overrideRecipe |= true;
                changes += $" {LinkedItemsIdsKey}";
                replacement.LinkedItems = this.LinkedItems;
            }
            else
            {
                // Copy original linked items
                for (int i = 0; i < original.linkedItemCount; i++)
                    replacement.LinkedItems.Add(original.GetLinkedItem(i));
            }

            if (overrideRecipe)
            {
                CraftDataHandler.SetTechData(this.TechType, replacement);
                QuickLogger.Debug($"Modifying recipe for '{this.ItemID}' from {this.Origin} with new values in: {changes}");
            }

            return true;
        }

        protected bool HandleUnlocks()
        {
            if (this.ForceUnlockAtStart)
            {
                KnownTechHandler.UnlockOnStart(this.TechType);
                QuickLogger.Debug($"{this.Key} for '{this.ItemID}' from {this.Origin} will be a unlocked at the start of the game");
            }

            if (this.UnlockingItems.Count > 0)
            {
                KnownTechHandler.SetAnalysisTechEntry(this.TechType, this.UnlockingItems);
            }

            if (this.UnlockedByItems.Count > 0)
            {
                TechType[] thisTechType = new[] { this.TechType };
                foreach (TechType unlockedByItem in this.UnlockedByItems)
                {
                    KnownTechHandler.SetAnalysisTechEntry(unlockedByItem, thisTechType);
                }
            }

            return true;
        }
    }
}
