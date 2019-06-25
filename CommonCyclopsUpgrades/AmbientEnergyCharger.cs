namespace CommonCyclopsUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal abstract class AmbientEnergyCharger<T> : ICyclopsCharger
        where T : AmbientEnergyUpgradeHandler
    {
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 0.01f;

        protected enum EnergyState
        {
            NoPower = 0,
            AmbientEnergyAvailable = 1,
            RunningOnBatteries = 2
        }

        public bool IsRenewable { get; } = true;

        public abstract string Name { get; }

        protected EnergyState CurrentState { get; private set; } = EnergyState.NoPower;

        protected abstract string PercentNotation { get; }

        protected abstract float MaximumEnergyStatus { get; }
        protected abstract float MinimumEnergyStatus { get; }

        protected readonly T ambientEnergyUpgrade;

        private readonly Atlas.Sprite tier1Sprite;
        private readonly Atlas.Sprite tier2Sprite;

        private readonly TechType tier1Id;
        private readonly TechType tier2Id2;

        protected readonly SubRoot Cyclops;

        private float energyStatus = 0f;

        protected AmbientEnergyCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops)
        {
            tier1Id = tier1TechType;
            tier2Id2 = tier2TechType;
            Cyclops = cyclops;
            tier1Sprite = SpriteManager.Get(tier1TechType);
            tier2Sprite = SpriteManager.Get(tier2TechType);
            ambientEnergyUpgrade = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(cyclops, tier1TechType, tier2TechType);
        }

        public Atlas.Sprite GetIndicatorSprite()
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

        public string GetIndicatorText()
        {
            switch (this.CurrentState)
            {
                case EnergyState.AmbientEnergyAvailable:
                    return EnergyStatusPercent();
                case EnergyState.RunningOnBatteries:
                    return NumberFormatter.FormatValue(ambientEnergyUpgrade.TotalBatteryCharge);
                default:
                    return string.Empty;
            }
        }

        internal string EnergyStatusPercent()
        {
            return NumberFormatter.FormatValue(energyStatus) + this.PercentNotation;
        }

        public Color GetIndicatorTextColor()
        {
            switch (this.CurrentState)
            {
                case EnergyState.AmbientEnergyAvailable:
                    return NumberFormatter.GetNumberColor(energyStatus, this.MaximumEnergyStatus, this.MinimumEnergyStatus);
                case EnergyState.RunningOnBatteries:
                    return NumberFormatter.GetNumberColor(ambientEnergyUpgrade.TotalBatteryCharge, ambientEnergyUpgrade.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public bool HasPowerIndicatorInfo()
        {
            return this.CurrentState > EnergyState.NoPower;
        }

        public float ProducePower(float requestedPower)
        {
            if (ambientEnergyUpgrade.TotalCount == 0)
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }

            energyStatus = GetEnergyStatus();

            if (energyStatus > this.MinimumEnergyStatus)
            {
                this.CurrentState = EnergyState.AmbientEnergyAvailable;

                float availableEnergy = ConvertToAvailableEnergy(energyStatus);

                float multipliedEnergy = ambientEnergyUpgrade.ChargeMultiplier * availableEnergy;

                if (requestedPower < multipliedEnergy)
                    ambientEnergyUpgrade.RechargeBatteries(multipliedEnergy - requestedPower);

                return multipliedEnergy;
            }
            else if (ambientEnergyUpgrade.TotalBatteryCharge > MinimalPowerValue)
            {
                this.CurrentState = EnergyState.RunningOnBatteries;
                return ambientEnergyUpgrade.GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                this.CurrentState = EnergyState.NoPower;
                return 0f;
            }
        }

        public float TotalReservePower()
        {
            return ambientEnergyUpgrade.TotalBatteryCharge;
        }

        protected abstract float GetEnergyStatus();

        protected abstract float ConvertToAvailableEnergy(float energyStatus);
    }
}
