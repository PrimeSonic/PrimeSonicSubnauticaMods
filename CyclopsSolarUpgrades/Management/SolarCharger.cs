namespace CyclopsSolarUpgrades.Management
{
    using CommonCyclopsUpgrades;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class SolarCharger : ICyclopsCharger
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 0.01f;
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 1.25f;
        private const float MaxSolarPercentage = 90f;
        private const float DiminishingReturnRatio = 0.6f;
        private const double MinUsableSunScalar = 0.01;
        private readonly Atlas.Sprite solar1Sprite;
        private readonly Atlas.Sprite solar2Sprite;

        private readonly TechType solarTier1;
        private readonly TechType solarTier2;

        private readonly SolarUpgradeHandler upgradeHandler;
        private readonly SubRoot Cyclops;

        private SolarState solarState = SolarState.None;
        private float solarPercentage = 0f;

        public SolarCharger(TechType solarUpgradeMk1, TechType solarUpgradeMk2, SubRoot cyclops)
        {
            solarTier1 = solarUpgradeMk1;
            solarTier2 = solarUpgradeMk2;
            solar1Sprite = SpriteManager.Get(solarUpgradeMk1);
            solar2Sprite = SpriteManager.Get(solarUpgradeMk2);
            Cyclops = cyclops;
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<SolarUpgradeHandler>(cyclops, solarUpgradeMk1, solarUpgradeMk2);
        }

        public bool IsRenewable { get; } = true;
        public string Name { get; }

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
                    return NumberFormatter.FormatSolarPercentage(solarPercentage);
                case SolarState.BatteryAvailable:
                    return NumberFormatter.FormatValue(upgradeHandler.TotalBatteryCharge);
                default:
                    return string.Empty;
            }
        }

        public Color GetIndicatorTextColor()
        {

            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return NumberFormatter.GetNumberColor(solarPercentage, MaxSolarPercentage, 0f);
                case SolarState.BatteryAvailable:
                    return NumberFormatter.GetNumberColor(upgradeHandler.TotalBatteryCharge, upgradeHandler.TotalBatteryCapacity, 0f);
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
            if (upgradeHandler.TotalCount == 0)
            {
                solarState = SolarState.None;
                return 0f;
            }

            float solarStatus = GetSolarChargeScalar();

            if (solarStatus > MinUsableSunScalar)
            {
                float availableSolarEnergy = DayNightCycle.main.deltaTime * SolarChargingFactor;
                solarState = SolarState.SunAvailable;
                solarPercentage = solarStatus * 100;

                float solarEnergy = upgradeHandler.ChargeMultiplier * availableSolarEnergy;

                if (requestedPower < solarEnergy)
                    upgradeHandler.RechargeBatteries(solarEnergy - requestedPower);

                return solarEnergy;
            }
            else if (upgradeHandler.TotalBatteryCharge > MinimalPowerValue)
            {
                solarState = SolarState.BatteryAvailable;
                return upgradeHandler.GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                solarState = SolarState.None;
                return 0f;
            }
        }

        public float TotalReservePower()
        {
            return upgradeHandler.TotalBatteryCharge;
        }

        /// <summary>
        /// Gets the amount of available energy provided by the currently available sunlight.
        /// </summary>
        /// <returns>The currently available solar energy.</returns>
        private float GetSolarChargeScalar()
        {
            if (DayNightCycle.main == null)
                return 0f; // Safety check

            // This based on the how the Solar Panel generates power.
            return DayNightCycle.main.GetLocalLightScalar() * // Sun Scalar
                   Mathf.Clamp01((MaxSolarDepth - Cyclops.transform.position.y) / MaxSolarDepth); // Depth Scalar                 
        }
    }
}
