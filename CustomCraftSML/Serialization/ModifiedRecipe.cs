namespace CustomCraft2SML.Serialization
{
    using Common;
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using UnityEngine.Assertions;

    internal class ModifiedRecipe : EmPropertyCollection, IModifiedRecipe
    {
        public const short Max = 25;
        public const short Min = 0;

        protected readonly EmProperty<string> emTechType;
        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyList<string> linkedItems;
        protected readonly EmYesNo unlockedAtStart;
        protected readonly EmPropertyList<string> unlocks;
        
        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public short? AmountCrafted
        {
            get
            {
                if (amountCrafted.HasValue)
                    return amountCrafted.Value;

                return null;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Amount crafted value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Amount crafted value for {ItemID} must be greater than {Min}.");
                amountCrafted.Value = (short)value;
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

            set => unlockedAtStart.Value = (bool)value;
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

        public void AddIngredient(string techType, short count) => ingredients.Add(new EmIngredient() { ItemID = techType, Required = count });

        public void AddLinkedItem(string linkedItem) => linkedItems.Add(linkedItem);

        public void AddUnlock(string unlock) => unlocks.Add(unlock);

        protected static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(7)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<short>("AmountCrafted", 1),
            new EmPropertyCollectionList<EmIngredient>("Ingredients", new EmIngredient()),
            new EmPropertyList<string>("LinkedItemIDs"),
            new EmYesNo("ForceUnlockAtStart"),
            new EmPropertyList<string>("Unlocks"),
        };

        internal ModifiedRecipe(TechType origTechType) : this()
        {
            ITechData origRecipe = CraftData.Get(origTechType);
            ItemID = origTechType.ToString();
            AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                var origIngredient = origRecipe.GetIngredient(i);
                AddIngredient(origIngredient.techType.ToString(), (short)origIngredient.amount);
            }

            for (int i = 0; i < origRecipe.linkedItemCount; i++)
                linkedItems.Add(origRecipe.GetLinkedItem(i).ToString());
        }

        public ModifiedRecipe() : this("ModifiedRecipe", ModifiedRecipeProperties)
        {
        }

        public ModifiedRecipe(string key) : this(key, ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
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

        internal override EmProperty Copy() => new ModifiedRecipe(Key, CopyDefinitions);

        public EmIngredient GetIngredient(int index) => ingredients[index];

        public string GetLinkedItem(int index) => linkedItems[index];

        public string GetUnlock(int index) => unlocks[index];
    }
}
