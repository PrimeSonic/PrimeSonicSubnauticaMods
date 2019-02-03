﻿namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomBioFuel : EmPropertyCollection, ICustomBioFuel
    {
        internal static readonly string[] TutorialText = new[]
        {
            "CustomBioFuel: Customize the energy values of items in the BioReactor. ",
            "This can also be used to make items compatible with the BioReactor that originally weren't."
        };

        private readonly EmProperty<string> emTechType;
        private readonly EmProperty<float> emEnergy;

        protected static List<EmProperty> BioFuelProperties => new List<EmProperty>(2)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<float>("Energy")
        };

        public CustomBioFuel() : base("CustomBioFuel", BioFuelProperties)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            emEnergy = (EmProperty<float>)Properties["Energy"];
        }

        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public float Energy
        {
            get => emEnergy.Value;
            set => emEnergy.Value = value;
        }

        internal override EmProperty Copy() => new CustomBioFuel();
    }
}
