namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class SolarCharger : ICyclopsCharger
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float Mk2ChargeRateModifier = 1.10f;
        private const float BatteryDrainRate = 0.01f;
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float MaxSolarPercentage = 90f;

        private readonly Atlas.Sprite solar1Sprite;
        private readonly Atlas.Sprite solar2Sprite;

        private readonly TechType solarTier1;
        private readonly TechType solarTier2;

        private readonly SolarUpgrade upgradeHandler;
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
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<SolarUpgrade>(cyclops, solarUpgradeMk1, solarUpgradeMk2);
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
                    return TextFormatter.FormatSolarPercentage(solarPercentage);
                case SolarState.BatteryAvailable:
                    return TextFormatter.FormatBatteryCharge(upgradeHandler.TotalBatteryCharge);
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
                    return TextFormatter.GetNumberColor(upgradeHandler.TotalBatteryCharge, upgradeHandler.TotalBatteryCapacity, 0f);
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

            float solarStatus = GetSolarStatus(Cyclops);
            float availableSolarEnergy = SolarChargingFactor * solarStatus;
            solarPercentage = solarStatus * 100;

            if (availableSolarEnergy > MinimalPowerValue)
            {
                solarState = SolarState.SunAvailable;
                float mk1Power = upgradeHandler[solarTier1] * availableSolarEnergy;
                float mk2Power = upgradeHandler[solarTier2] * availableSolarEnergy * Mk2ChargeRateModifier;

                upgradeHandler.RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (upgradeHandler.TotalBatteryCharge > 0f)
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
        private static float GetSolarChargeAmount(SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.

            if (DayNightCycle.main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.

            return SolarChargingFactor *
                   DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + cyclops.transform.position.y) / MaxSolarDepth); // Distance to surfuce
        }

        private static float GetSolarStatus(SubRoot cyclops)
        {
            if (DayNightCycle.main == null)
                return 0f; // Safety check

            return DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + cyclops.transform.position.y) / MaxSolarDepth);
        }

        
    }
}
