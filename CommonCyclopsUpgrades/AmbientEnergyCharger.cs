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

        protected enum EnergyState
        {
            NoPower = 0,
            AmbientEnergyAvailable = 1,
            RunningOnBatteries = 2
        }

        protected EnergyState CurrentState { get; private set; } = EnergyState.NoPower;

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

        protected AmbientEnergyCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops) : base(cyclops)
        {
            tier1Id = tier1TechType;
            tier2Id2 = tier2TechType;
            tier1Sprite = SpriteManager.Get(tier1TechType);
            tier2Sprite = SpriteManager.Get(tier2TechType);
        }

        public override Atlas.Sprite StatusSprite()
        {
            switch (this.CurrentState)
            {
                case EnergyState.AmbientEnergyAvailable:
                    return tier1Sprite;
                case EnergyState.RunningOnBatteries:
                    return tier2Sprite;
                default:
                    return null;
            }
        }

        public override string StatusText()
        {
            switch (this.CurrentState)
            {
                case EnergyState.AmbientEnergyAvailable:
                    return EnergyStatusText();
                case EnergyState.RunningOnBatteries:
                    return ReservePowerText();
                default:
                    return string.Empty;
            }
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
            switch (this.CurrentState)
            {
                case EnergyState.AmbientEnergyAvailable:
                    return NumberFormatter.GetNumberColor(energyStatus, this.MaximumEnergyStatus, this.MinimumEnergyStatus);
                case EnergyState.RunningOnBatteries:
                    return NumberFormatter.GetNumberColor(this.AmbientEnergyUpgrade.TotalBatteryCharge, this.AmbientEnergyUpgrade.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public float ProducePower(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade.Count == 0)
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }

            energyStatus = GetEnergyStatus();

            if (energyStatus > this.MinimumEnergyStatus)
            {
                this.CurrentState = EnergyState.AmbientEnergyAvailable;

                float availableEnergy = ConvertToAvailableEnergy(energyStatus);

                float multipliedEnergy = this.AmbientEnergyUpgrade.ChargeMultiplier * availableEnergy;

                if (requestedPower < multipliedEnergy)
                    this.AmbientEnergyUpgrade.RechargeBatteries(multipliedEnergy - requestedPower);

                return multipliedEnergy;
            }
            else if (this.AmbientEnergyUpgrade.TotalBatteryCharge > MinimalPowerValue)
            {
                this.CurrentState = EnergyState.RunningOnBatteries;
                return this.AmbientEnergyUpgrade.GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade.Count == 0)
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }

            energyStatus = GetEnergyStatus();

            if (energyStatus > this.MinimumEnergyStatus)
            {
                this.CurrentState = EnergyState.AmbientEnergyAvailable;

                float availableEnergy = ConvertToAvailableEnergy(energyStatus);

                float multipliedEnergy = this.AmbientEnergyUpgrade.ChargeMultiplier * availableEnergy;

                if (requestedPower < multipliedEnergy)
                    this.AmbientEnergyUpgrade.RechargeBatteries(multipliedEnergy - requestedPower);

                return multipliedEnergy;
            }

            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade.TotalBatteryCharge > MinimalPowerValue)
            {
                this.CurrentState = EnergyState.RunningOnBatteries;
                return this.AmbientEnergyUpgrade.GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }
        }

        protected abstract float GetEnergyStatus();

        protected abstract float ConvertToAvailableEnergy(float energyStatus);
    }
}
