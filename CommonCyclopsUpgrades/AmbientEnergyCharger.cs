namespace CommonCyclopsUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal abstract class AmbientEnergyCharger<T> : CyclopsCharger
        where T : AmbientEnergyUpgradeHandler
    {
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 2.0f;

        private bool ambientEnergyAvailable = false;

        protected abstract string PercentNotation { get; }
        protected abstract float MaximumEnergyStatus { get; }
        protected abstract float MinimumEnergyStatus { get; }

        public override float TotalReserveEnergy => this.AmbientEnergyUpgrade.TotalBatteryCharge;

        private T energyUpgrade;
        protected T AmbientEnergyUpgrade => energyUpgrade ?? (energyUpgrade = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(Cyclops, tier1Id, tier2Id2));

        private readonly Atlas.Sprite tier1Sprite;
        private readonly Atlas.Sprite tier2Sprite;

        private readonly TechType tier1Id;
        private readonly TechType tier2Id2;

        private float energyStatus = 0f;
        private float resultingEnergy = 0f;

        protected AmbientEnergyCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops) : base(cyclops)
        {
            tier1Id = tier1TechType;
            tier2Id2 = tier2TechType;
            tier1Sprite = SpriteManager.Get(tier1TechType);
            tier2Sprite = SpriteManager.Get(tier2TechType);
        }

        protected abstract void UpdateEnergyStatus(ref float ambientEnergyStatus);

        protected abstract float ConvertToAvailableEnergy(float energyStatus);

        public override Atlas.Sprite StatusSprite()
        {
            return ambientEnergyAvailable ? tier1Sprite : tier2Sprite;
        }

        public override string StatusText()
        {
            return ambientEnergyAvailable ? EnergyStatusText() : ReservePowerText();
        }

        internal string EnergyStatusText()
        {
            return NumberFormatter.FormatValue(energyStatus) + this.PercentNotation;
        }

        internal string ReservePowerText()
        {
            return NumberFormatter.FormatValue(this.AmbientEnergyUpgrade.TotalBatteryCharge);
        }

        public override Color StatusTextColor()
        {
            return ambientEnergyAvailable
                ? NumberFormatter.GetNumberColor(energyStatus, this.MaximumEnergyStatus, this.MinimumEnergyStatus)
                : NumberFormatter.GetNumberColor(this.AmbientEnergyUpgrade.TotalBatteryCharge, this.AmbientEnergyUpgrade.TotalBatteryCapacity, 0f);
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade.Count == 0)
            {
                return 0f;
            }

            UpdateEnergyStatus(ref energyStatus);

            ambientEnergyAvailable = energyStatus > this.MinimumEnergyStatus;

            if (ambientEnergyAvailable)
            {
                resultingEnergy = this.AmbientEnergyUpgrade.ChargeMultiplier * ConvertToAvailableEnergy(energyStatus);

                if (requestedPower < resultingEnergy)
                    this.AmbientEnergyUpgrade.RechargeBatteries(resultingEnergy - requestedPower);

                return resultingEnergy;
            }

            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (!ambientEnergyAvailable && this.AmbientEnergyUpgrade.TotalBatteryCharge > MinimalPowerValue)
            {
                return this.AmbientEnergyUpgrade.GetBatteryPower(BatteryDrainRate, requestedPower);
            }

            return 0f;
        }
    }
}
