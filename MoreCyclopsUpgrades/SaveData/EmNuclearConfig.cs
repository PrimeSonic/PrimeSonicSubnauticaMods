namespace MoreCyclopsUpgrades
{
    using System;
    using System.Collections.Generic;
    using Common.EasyMarkup;

    internal class EmNuclearConfig : EmPropertyCollection
    {
        internal bool ValidDataRead = true;

        internal const string ConfigKey = "CyclopsNuclearChargerConfig";
        private const string EmConserveKey = "ConserveNuclearModulePower";
        private const string EmDeficitKey = "ChargeBelowPercent";

        internal const string EmConserveDescription = "Conserve Nuclear Module Power";
        internal const string EmDeficitDescription = "Charge Below Percent";

        private readonly float MinF;
        private readonly float MaxF;
        private readonly float DefaultF;

        internal bool ConserveNuclearModulePower
        {
            get => EmConserve.Value;
            set => EmConserve.Value = value;
        }

        internal float RequiredEnergyPercentage
        {
            get => EmDeficit.Value;
            set => EmDeficit.Value = value;
        }

        private readonly EmYesNo EmConserve;
        private readonly EmProperty<float> EmDeficit;

        private static ICollection<EmProperty> definitions = new List<EmProperty>()
        {
            new EmYesNo(EmConserveKey, false),
            new EmProperty<float>(EmDeficitKey, 95f)
        };

        public EmNuclearConfig(float minimumPercent, float maximumPercent, float defaultPercent) : base(ConfigKey, definitions)
        {
            EmConserve = (EmYesNo)Properties[EmConserveKey];
            EmDeficit = (EmProperty<float>)Properties[EmDeficitKey];

            MinF = minimumPercent;
            MaxF = maximumPercent;
            DefaultF = defaultPercent;

            OnValueExtractedEvent += Validate;
        }

        private void Validate()
        {
            if (RequiredEnergyPercentage > MaxF || RequiredEnergyPercentage < MinF)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Config value for {ConfigKey}>{EmDeficit.Key} was out of range. Replaced with default.");
                RequiredEnergyPercentage = DefaultF;
                ValidDataRead = false;
            }

            if (!EmConserve.HasValue)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Config value for {ConfigKey}>{EmConserve.Key} was out of range. Replaced with default.");
                ValidDataRead = false;
            }
        }

        internal override EmProperty Copy() => new EmNuclearConfig(MinF, MaxF, DefaultF);
    }
}
