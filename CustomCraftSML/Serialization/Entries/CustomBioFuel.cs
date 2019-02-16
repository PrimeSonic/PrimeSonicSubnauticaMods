namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Handlers;

    internal class CustomBioFuel : EmTechTyped, ICustomBioFuel, ICustomCraft
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

        public string ID => this.ItemID;

        public float Energy
        {
            get => emEnergy.Value;
            set => emEnergy.Value = value;
        }

        internal override EmProperty Copy() => new CustomBioFuel(this.Key, this.CopyDefinitions);

        public bool SendToSMLHelper()
        {
            try
            {
                BioReactorHandler.SetBioReactorCharge(this.TechType, this.Energy);
                QuickLogger.Message($"'{this.ItemID}' now provides {this.Energy} energy in the BioReactor");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }
    }
}
