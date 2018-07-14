namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using SMLHelper.V2.Crafting;
    using UnityEngine.Assertions;

    public class ModifiedRecipe : EmPropertyCollection, IModifiedRecipe
    {
        public const short Max = 25;
        public const short Min = 1;

        protected readonly EmPropertyTechType emTechType;
        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyTechTypeList linkedItems;

        public TechType ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public short AmountCrafted
        {
            get
            {
                Assert.IsTrue(amountCrafted.Value <= Max, $"Amount crafted value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(amountCrafted.Value >= Min, $"Amount crafted value for {ItemID} must be greater than {Min}.");
                return amountCrafted.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Amount crafted value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Amount crafted value for {ItemID} must be greater than {Min}.");
                amountCrafted.Value = value;
            }
        }

        public List<TechType> LinkedItems => linkedItems.Values;

        private readonly List<Ingredient> smlIngredients = new List<Ingredient>();
        public IList<Ingredient> SmlIngredients => smlIngredients;

        public void AddIngredient(TechType techType, short count)
        {
            ingredients.Collections.Add(new EmIngredient() { ItemID = techType, Required = count });
            smlIngredients.Add(new Ingredient(techType, count));
        }

        public static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(4)
        {
            new EmPropertyTechType("ItemID"),
            new EmProperty<short>("AmountCrafted", 1),
            new EmPropertyCollectionList<EmIngredient>("Ingredients", new EmIngredient()),
            new EmPropertyTechTypeList("LinkedItemIDs")
        };

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

            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (var ingredient in ingredients.Collections)
            {
                TechType itemID = (ingredient["ItemID"] as EmPropertyTechType).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;

                smlIngredients.Add(new Ingredient(itemID, required));
            }
        }

        internal override EmProperty Copy() => new ModifiedRecipe(Key, CopyDefinitions);

        public virtual TechData SmlHelperRecipe()
        {
            var ingredientsList = new List<Ingredient>(smlIngredients.Count);

            foreach (Ingredient item in SmlIngredients)
            {
                ingredientsList.Add(new Ingredient(item.techType, item.amount));
            }

            return new TechData(ingredientsList)
            {
                craftAmount = AmountCrafted,
                LinkedItems = LinkedItems
            };
        }
    }
}
