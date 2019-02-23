namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Handlers;

    internal class CustomBioFuel : EmTechTyped, ICustomBioFuel, ICustomCraft
    {
        private const string EnergyKey = "Energy";

        internal static readonly string[] TutorialText = new[]
        {
            "CustomBioFuel: Customize the energy values of items in the BioReactor. ",
           $"    {EnergyKey}: Set this to the amount of energy the item provides via the BioReactor",
            "    This can also be used to make items compatible with the BioReactor that originally weren't."
        };

        protected readonly EmProperty<float> emEnergy;

        protected static List<EmProperty> BioFuelProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<float>(EnergyKey)
        };

        public CustomBioFuel() : this("CustomBioFuel", BioFuelProperties)
        {
        }

        protected CustomBioFuel(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emEnergy = (EmProperty<float>)Properties[EnergyKey];
        }

        public OriginFile Origin { get; set; }

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
                QuickLogger.Debug($"'{this.ItemID}' now provides {this.Energy} energy in the BioReactor - Entry from {this.Origin}");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{this.ItemID} from {this.Origin}'", ex);
                return false;
            }
        }
    }
}
