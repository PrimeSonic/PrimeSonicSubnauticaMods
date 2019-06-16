namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal partial class Solar : ICyclopsCharger
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float Mk2ChargeRateModifier = 1.10f;
        private const float BatteryDrainRate = 0.01f;
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float MaxSolarPercentage = 90f;

        private SolarState solarState = SolarState.None;
        private float solarPercentage = 0f;

        public bool IsRenewable { get; } = true;

        public Atlas.Sprite GetIndicatorSprite()
        {
            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return solar1Sprite;
                case SolarState.BatteryAvailable:
                    return solar2Sprite;
                default:
                    return null;
            }
        }

        public string GetIndicatorText()
        {
            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return TextFormatter.FormatSolarPercentage(solarPercentage);
                case SolarState.BatteryAvailable:
                    return TextFormatter.FormatBatteryCharge(this.TotalBatteryCharge);
                default:
                    return string.Empty;
            }
        }

        public Color GetIndicatorTextColor()
        {

            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return TextFormatter.GetNumberColor(solarPercentage, MaxSolarPercentage, 0f);
                case SolarState.BatteryAvailable:
                    return TextFormatter.GetNumberColor(this.TotalBatteryCharge, this.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public bool HasPowerIndicatorInfo()
        {
            return solarState != SolarState.None;
        }

        public float ProducePower(float requestedPower)
        {
            if (this.TotalCount == 0)
            {
                solarState = SolarState.None;
                return 0f;
            }

            float solarStatus = GetSolarStatus(cyclops);
            float availableSolarEnergy = SolarChargingFactor * solarStatus;
            solarPercentage = solarStatus * 100;

            if (availableSolarEnergy > MinimalPowerValue)
            {
                solarState = SolarState.SunAvailable;
                float mk1Power = this[solarTier1] * availableSolarEnergy;
                float mk2Power = this[solarTier2] * availableSolarEnergy * Mk2ChargeRateModifier;

                RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (this.TotalBatteryCharge > 0f)
            {
                solarState = SolarState.BatteryAvailable;
                return GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                solarState = SolarState.None;
                return 0f;
            }
        }

        public float TotalReservePower()
        {
            return this.TotalBatteryCharge;
        }
    }
}
