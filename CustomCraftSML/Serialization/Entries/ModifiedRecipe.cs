namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class ModifiedRecipe : EmTechTyped, IModifiedRecipe
    {
        internal static readonly string[] TutorialText = new[]
        {
            "ModifiedRecipe: Modify an existing crafting recipe. ",
            "    Ingredients: Completely replace a recipe's required ingredients." +
            "        This is optional if you don't want to change the required ingredients.",
            "    AmountCrafted: Change how many copies of the item are created when you craft the recipe.",
            "        This is optional if you don't want to change how many copies of the item are created at once.",
            "    LinkedItemIDs: Add or modify the linked items that are created when the recipe is crafted.",
            "        This is optional if you don't want to change what items are crafted with this one.",
            "    Unlocks: Set other recipes to be unlocked when you analyze or craft this one.",
            "        This is optional if you don't want to change what gets unlocked when you scan or craft this item.",
            "    ForceUnlockAtStart: You can also set if this recipe should be unlocked at the start or not. Make sure you have a recipe unlocking this one.",
            "        This is optional. For Added Recipes, this defaults to 'YES'.",
        };

        public const short Max = 25;
        public const short Min = 0;

        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyList<string> linkedItems;
        protected readonly EmYesNo unlockedAtStart;
        protected readonly EmPropertyList<string> unlocks;

        public string ID => this.ItemID;

        public short? AmountCrafted
        {
            get
            {
                if (amountCrafted.HasValue && amountCrafted.Value > -1)
                    return amountCrafted.Value;

                return null;
            }
            set
            {
                if (amountCrafted.HasValue = value.HasValue)
                    amountCrafted.Value = value.Value;
            }
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

        public IEnumerable<string> Unlocks => unlocks.Values;

        public int? UnlocksCount
        {
            get
            {
                if (unlocks.HasValue)
                    return unlocks.Count;

                return null;
            }
        }

        public IEnumerable<EmIngredient> Ingredients => ingredients.Values;

        public int? IngredientsCount
        {
            get
            {
                if (ingredients.HasValue)
                    return ingredients.Count;

                return null;
            }
        }

        public IEnumerable<string> LinkedItems => linkedItems.Values;

        public int? LinkedItemsCount
        {
            get
            {
                if (linkedItems.HasValue)
                    return linkedItems.Count;

                return null;
            }
        }

        public void AddIngredient(string techType, short count = 1) => ingredients.Add(new EmIngredient() { ItemID = techType, Required = count });

        public void AddIngredient(TechType techType, short count = 1) => AddIngredient(techType.ToString(), count);

        public void AddLinkedItem(string linkedItem) => linkedItems.Add(linkedItem);

        public void AddLinkedItem(TechType linkedItem) => AddLinkedItem(linkedItem.ToString());

        public void AddUnlock(string unlock) => unlocks.Add(unlock);

        protected static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>("AmountCrafted", 1) { Optional = true },
            new EmPropertyCollectionList<EmIngredient>("Ingredients", new EmIngredient()) { Optional = true },
            new EmPropertyList<string>("LinkedItemIDs") { Optional = true },
            new EmYesNo("ForceUnlockAtStart") { Optional = true },
            new EmPropertyList<string>("Unlocks") { Optional = true },
        };

        internal ModifiedRecipe(TechType origTechType) : this()
        {
            ITechData origRecipe = CraftData.Get(origTechType);
            this.ItemID = origTechType.ToString();
            this.AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                IIngredient origIngredient = origRecipe.GetIngredient(i);
                AddIngredient(origIngredient.techType.ToString(), (short)origIngredient.amount);
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
            amountCrafted = (EmProperty<short>)Properties["AmountCrafted"];
            ingredients = (EmPropertyCollectionList<EmIngredient>)Properties["Ingredients"];
            linkedItems = (EmPropertyList<string>)Properties["LinkedItemIDs"];
            unlockedAtStart = (EmYesNo)Properties["ForceUnlockAtStart"];
            unlocks = (EmPropertyList<string>)Properties["Unlocks"];

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

        internal override EmProperty Copy() => new ModifiedRecipe(this.Key, this.CopyDefinitions);

        public EmIngredient GetIngredient(int index) => ingredients[index];

        public string GetLinkedItem(int index) => linkedItems[index];

        public string GetUnlock(int index) => unlocks[index];

        public override bool PassesPreValidation() => base.PassesPreValidation() && InnerItemsAreValid();

        protected bool InnerItemsAreValid()
        {
            // Sanity check of the blueprints ingredients and linked items to be sure that it only contains known items
            // Modded items are okay, but they must be for mods the player already has installed
            bool internalItemsPassCheck = true;

            foreach (EmIngredient ingredient in this.Ingredients)
            {
                TechType ingredientID = GetTechType(ingredient.ItemID);

                if (ingredientID == TechType.None)
                {
                    QuickLogger.Warning($"Entry with ID of '{this.ItemID}' contained an unknown ingredient '{ingredient.ItemID}'.  Entry will be discarded.");
                    internalItemsPassCheck = false;
                    continue;
                }

                ingredient.TechType = ingredientID;
            }

            foreach (string linkedItem in this.LinkedItems)
            {
                TechType linkedItemID = GetTechType(linkedItem);

                if (linkedItemID == TechType.None)
                {
                    QuickLogger.Warning($"Entry with ID of '{this.ItemID}' contained an unknown linked item '{linkedItem}'. Entry will be discarded.");
                    internalItemsPassCheck = false;
                    continue;
                }
            }

            return internalItemsPassCheck;
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
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }

        protected bool HandleModifiedRecipe()
        {
            bool overrideRecipe = false;

            if (this.TechType == TechType.None)
                return false; // Unknown item

            ITechData original = CraftData.Get(this.TechType, skipWarnings: true);

            if (original == null) // Possibly a mod recipe
                original = CraftDataHandler.GetModdedTechData(this.TechType);

            if (original == null)
                return false;  // Unknown recipe

            var replacement = new TechData();

            // Amount
            if (this.AmountCrafted.HasValue)
            {
                overrideRecipe |= true;
                replacement.craftAmount = this.AmountCrafted.Value;
            }
            else
            {
                replacement.craftAmount = original.craftAmount;
            }

            // Ingredients
            if (this.IngredientsCount.HasValue)
            {
                overrideRecipe |= true;
                foreach (EmIngredient ingredient in this.Ingredients)
                {
                    replacement.Ingredients.Add(
                        new Ingredient(
                            GetTechType(ingredient.ItemID),
                            ingredient.Required));
                }
            }
            else
            {
                for (int i = 0; i < original.ingredientCount; i++)
                    replacement.Ingredients.Add(
                        new Ingredient(
                        original.GetIngredient(i).techType,
                        original.GetIngredient(i).amount));
            }

            // Linked Items
            if (this.LinkedItemsCount.HasValue)
            {
                overrideRecipe |= true;
                foreach (string linkedItem in this.LinkedItems)
                    replacement.LinkedItems.Add(GetTechType(linkedItem));
            }
            else
            {
                for (int i = 0; i < original.linkedItemCount; i++)
                    replacement.LinkedItems.Add(original.GetLinkedItem(i));
            }

            if (overrideRecipe)
            {
                CraftDataHandler.SetTechData(this.TechType, replacement);
                QuickLogger.Message($"Modifying recipe for '{this.ItemID}'");
            }

            return true;
        }

        protected bool HandleUnlocks()
        {
            if (this.ForceUnlockAtStart)
            {
                KnownTechHandler.UnlockOnStart(this.TechType);
                QuickLogger.Message($"Recipe for '{this.ItemID}' will be a unlocked at the start of the game");
            }

            if (this.UnlocksCount.HasValue && this.UnlocksCount > 0)
            {
                var unlocks = new List<TechType>();

                foreach (string value in this.Unlocks)
                {
                    unlocks.Add(GetTechType(value));
                    QuickLogger.Message($"Recipe for '{value}' will be a unlocked when '{this.ItemID}' is scanned or picked up");
                }

                KnownTechHandler.SetAnalysisTechEntry(this.TechType, unlocks);
            }

            return true;
        }
    }
}
