namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;

    internal class CustomBioFuel : EmTechTyped, ICustomBioFuel
    {
        internal static readonly string[] TutorialText = new[]
        {
            "CustomBioFuel: Customize the energy values of items in the BioReactor. ",
            "This can also be used to make items compatible with the BioReactor that originally weren't."
        };

        private readonly EmProperty<float> emEnergy;

        protected static List<EmProperty> BioFuelProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<float>("Energy")
        };

        public CustomBioFuel() : this("CustomBioFuel", BioFuelProperties)
        {
        }

        protected CustomBioFuel(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emEnergy = (EmProperty<float>)Properties["Energy"];
        }

        public float Energy
        {
            get => emEnergy.Value;
            set => emEnergy.Value = value;
        }

        internal override EmProperty Copy() => new CustomBioFuel(this.Key, this.CopyDefinitions);
    }
}
