namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using SMLHelper.V2.Crafting;
    using UnityEngine.Assertions;

    public class ModifiedRecipe : EmPropertyCollection, IModifiedRecipe
    {
        public const short Max = 25;
        public const short Min = 0;

        protected readonly EmPropertyTechType emTechType;
        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyTechTypeList linkedItems;
        protected readonly EmYesNo unlockedAtStart;
        protected readonly EmPropertyTechTypeList unlocks;

        public TechType ItemID
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

        protected virtual bool DefaultForceUnlock => false;

        public bool? ForceUnlockAtStart
        {
            get
            {
                if (unlockedAtStart.HasValue)
                    return unlockedAtStart.Value;

                return null;
            }

            set => unlockedAtStart.Value = (bool)value;
        }

        public IEnumerable<TechType> Unlocks => unlocks.Values;

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

        public IEnumerable<TechType> LinkedItems => linkedItems.Values;

        public int? LinkedItemsCount
        {
            get
            {
                if (linkedItems.HasValue)
                    return linkedItems.Count;

                return null;
            }
        }

        public void AddIngredient(TechType techType, short count) =>
    ingredients.Add(new EmIngredient() { ItemID = techType, Required = count });

        public void AddLinkedItem(TechType linkedItem) => linkedItems.Add(linkedItem);

        public void AddUnlock(TechType unlock) => unlocks.Add(unlock);

        protected static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(4)
        {
            new EmPropertyTechType("ItemID"),
            new EmProperty<short>("AmountCrafted", 1),
            new EmPropertyCollectionList<EmIngredient>("Ingredients", new EmIngredient()),
            new EmPropertyTechTypeList("LinkedItemIDs"),
            new EmYesNo("ForceUnlockAtStart"),
            new EmPropertyTechTypeList("Unlocks"),
        };

        public int craftAmount => throw new System.NotImplementedException();

        public int ingredientCount => throw new System.NotImplementedException();

        public int linkedItemCount => throw new System.NotImplementedException();

        internal ModifiedRecipe(TechType origTechType) : this()
        {
            ITechData origRecipe = CraftData.Get(origTechType);
            ItemID = origTechType;
            AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                var origIngredient = origRecipe.GetIngredient(i);
                AddIngredient(origIngredient.techType, (short)origIngredient.amount);
            }

            for (int i = 0; i < origRecipe.linkedItemCount; i++)
            {
                linkedItems.Add(origRecipe.GetLinkedItem(i));
            }

        }

        public ModifiedRecipe() : this("ModifiedRecipe", ModifiedRecipeProperties)
        {
        }

        public ModifiedRecipe(string key) : this(key, ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmPropertyTechType)Properties["ItemID"];
            amountCrafted = (EmProperty<short>)Properties["AmountCrafted"];
            ingredients = (EmPropertyCollectionList<EmIngredient>)Properties["Ingredients"];
            linkedItems = (EmPropertyTechTypeList)Properties["LinkedItemIDs"];
            unlockedAtStart = (EmYesNo)Properties["ForceUnlockAtStart"];
            unlocks = (EmPropertyTechTypeList)Properties["Unlocks"];

            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (EmIngredient ingredient in ingredients)
            {
                TechType itemID = (ingredient["ItemID"] as EmPropertyTechType).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;
            }
        }

        internal override EmProperty Copy() => new ModifiedRecipe(Key, CopyDefinitions);

        public EmIngredient GetIngredient(int index) => ingredients[index];

        public TechType GetLinkedItem(int index) => linkedItems[index];

        public TechType GetUnlock(int index) => unlocks[index];
    }
}
