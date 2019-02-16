namespace CustomCraft2SML.Serialization.Components
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    public abstract class EmTechTyped : EmPropertyCollection, ITechTyped
    {
        protected readonly EmProperty<string> emTechType;

        protected static List<EmProperty> TechTypedProperties => new List<EmProperty>(1)
        {
            new EmProperty<string>("ItemID"),
        };

        public EmTechTyped() : this("TechTyped", TechTypedProperties)
        {
        }

        protected EmTechTyped(string key) : this(key, TechTypedProperties)
        {
        }

        protected EmTechTyped(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
        }

        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public TechType TechID { get; set; }
    }
}
