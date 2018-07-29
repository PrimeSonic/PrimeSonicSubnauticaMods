namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using UnityEngine.Assertions;

    public class EmIngredient : EmPropertyCollection, IIngredient
    {
        public const short Max = 25;
        public const short Min = 1;

        private readonly EmPropertyTechType emTechType;
        private readonly EmProperty<short> required;

        public TechType ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public short Required
        {
            get
            {
                Assert.IsTrue(required.Value <= Max, $"Amount required value for ingredient {ItemID} must be less than {Max}.");
                Assert.IsTrue(required.Value >= Min, $"Amount required value for ingredient {ItemID} must be greater than {Min}.");
                return required.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Amount required value for ingredient {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Amount required value for ingredient {ItemID} must be greater than {Min}.");
                required.Value = value;
            }

        }

        protected static List<EmProperty> IngredientProperties => new List<EmProperty>(2)
        {
            new EmPropertyTechType("ItemID"),
            new EmProperty<short>("Required", 1),
        };

        public TechType techType => ItemID;

        public int amount => Required;

        internal EmIngredient(TechType item) : this()
        {
            ItemID = item;
        }

        internal EmIngredient(TechType item, short required) : this(item)
        {
            Required = required;
        }

        internal EmIngredient() : base("Ingredients", IngredientProperties)
        {
            emTechType = (EmPropertyTechType)Properties["ItemID"];
            required = (EmProperty<short>)Properties["Required"];
        }

        internal override EmProperty Copy()
        {
            if (ItemID != TechType.None)
                return new EmIngredient(ItemID, Required);

            return new EmIngredient();
        }
    }
}
