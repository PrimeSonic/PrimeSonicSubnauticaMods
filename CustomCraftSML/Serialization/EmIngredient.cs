namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using EasyMarkup;
    using UnityEngine.Assertions;

    public class EmIngredient : EmPropertyCollection
    {
        public const short Max = 25;
        public const short Min = 1;

        private readonly EmTechType emTechType;
        private readonly EmProperty<short> required;

        public TechType ItemID => emTechType.Value;

        public short Required
        {
            get
            {
                Assert.IsTrue(required.Value <= Max, $"Amount required value for ingredient {ItemID} must be less than {Max}.");
                Assert.IsTrue(required.Value >= Min, $"Amount required value for ingredient {ItemID} must be greater than {Min}.");
                return required.Value;
            }
        }

        public static List<EmProperty> IngredientProperties => new List<EmProperty>(2)
        {
            new EmTechType("ItemID"),
            new EmProperty<short>("Required"),
        };

        public EmIngredient() : base("Ingredient", IngredientProperties)
        {
            emTechType = (EmTechType)Properties["ItemID"];
            required = (EmProperty<short>)Properties["Required"];
        }
    }
}
