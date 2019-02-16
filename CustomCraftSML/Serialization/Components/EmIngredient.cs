namespace CustomCraft2SML.Serialization.Components
{
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class EmIngredient : EmTechTyped
    {
        public const short Max = 25;
        public const short Min = 1;

        private readonly EmProperty<short> required;

        public short Required
        {
            get => required.Value;
            set
            {
                if (value > Max || value < Min)
                    value = required.DefaultValue;

                required.Value = value;
            }

        }

        protected static List<EmProperty> IngredientProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>("Required", 1),
        };

        public int amount => this.Required;

        internal EmIngredient(string item) : this()
        {
            this.ItemID = item;
        }

        internal EmIngredient(string item, short required) : this(item)
        {
            this.Required = required;
        }

        internal EmIngredient() : base("Ingredients", IngredientProperties)
        {
            required = (EmProperty<short>)Properties["Required"];
        }

        internal override EmProperty Copy() => new EmIngredient(this.ItemID, this.Required);
    }
}
