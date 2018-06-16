namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using EasyMarkup;
    using UnityEngine.Assertions;

    public class CustomRecipe : EmPropertyCollection
    {
        public const short Max = 25;
        public const short Min = 1;

        protected readonly EmTechType emTechType;
        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList ingredients;
        protected readonly EmTechTypeList linkedItems;

        public TechType ItemID => emTechType.Value;

        public short AmountCrafted
        {
            get
            {
                Assert.IsTrue(amountCrafted.Value <= Max, $"Amount crafted value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(amountCrafted.Value >= Min, $"Amount crafted value for {ItemID} must be greater than {Min}.");
                return amountCrafted.Value;
            }
        }

        public List<TechType> LinkedItems => linkedItems.Values;

        public readonly List<Ingredient> Ingredients = new List<Ingredient>();

        protected static List<EmProperty> RecipeProperties => new List<EmProperty>(4)
        {
            new EmTechType("ItemID"),
            new EmProperty<short>("AmountCrafted"),
            new EmPropertyCollectionList("Ingredients", EmIngredient.IngredientProperties),
            new EmTechTypeList("LinkedItemIDs")
        };

        public CustomRecipe() : base("ModifiedRecipe", RecipeProperties)
        {
            emTechType = (EmTechType)Properties["ItemID"];
            amountCrafted = (EmProperty<short>)Properties["AmountCrafted"];
            ingredients = (EmPropertyCollectionList)Properties["Ingredients"];
            linkedItems = (EmTechTypeList)Properties["LinkedItemIDs"];
        }

        protected CustomRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmTechType)Properties["ItemID"];
            amountCrafted = (EmProperty<short>)Properties["AmountCrafted"];
            ingredients = (EmPropertyCollectionList)Properties["Ingredients"];
            linkedItems = (EmTechTypeList)Properties["LinkedItemIDs"];
        }

        protected override void OnValueExtracted()
        {
            foreach (EmPropertyCollection ingredient in ingredients.Collections)
            {
                TechType itemID = (ingredient["ItemID"] as EmTechType).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;

                Ingredients.Add(new Ingredient(itemID, required));
            }
        }
    }
}
