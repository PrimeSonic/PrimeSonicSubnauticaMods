namespace CustomCraft2SML.Serialization.Components
{
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;

    public class EmIngredient : EmTechTyped
    {
        public const short Max = 25;
        public const short Min = 1;
        private const string RequiredKey = "Required";

        private readonly EmProperty<short> required;

        public short Required
        {
            get => required.Value;
            set => required.Value = value;

        }

        protected static List<EmProperty> IngredientProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(RequiredKey, 1),
        };

        internal EmIngredient(string item) : this()
        {
            this.ItemID = item;
        }

        internal EmIngredient(string item, short required) : this(item)
        {
            this.Required = required;
        }

        internal EmIngredient() : base("Ingredient", IngredientProperties)
        {
            required = (EmProperty<short>)Properties[RequiredKey];
        }

        internal override EmProperty Copy() => new EmIngredient(this.ItemID, this.Required);

        public override bool PassesPreValidation() => base.PassesPreValidation() && RequireValueInRange();

        private bool RequireValueInRange()
        {
            if (this.Required > Max || this.Required < Min)
            {
                QuickLogger.Error($"Error in {this.Key} {RequiredKey} for '{this.ItemID}'. Required values must be between between {Min} and {Max}.");
                return false;
            }

            return true;
        }
    }
}
