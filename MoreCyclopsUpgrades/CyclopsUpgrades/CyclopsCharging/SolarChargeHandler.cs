namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal class SolarChargeHandler : ICyclopsCharger
    {
        private enum SolarState
        {
            None,
            SunAvailable,
            BatteryAvailable
        }

        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float MaxSolarPercentage = 90f;

        internal readonly ChargingUpgradeHandler SolarChargers;
        internal readonly BatteryUpgradeHandler SolarChargerMk2;

        public readonly SubRoot Cyclops;

        private SolarState solarState = SolarState.None;
        private float solarPercentage = 0f;

        public SolarChargeHandler(SubRoot cyclops,
                                  ChargingUpgradeHandler solarChargers,
                                  BatteryUpgradeHandler solarChargerMk2)
        {
            Cyclops = cyclops;
            SolarChargers = solarChargers;
            SolarChargerMk2 = solarChargerMk2;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return SpriteManager.Get(CyclopsModule.SolarChargerID);
                case SolarState.BatteryAvailable:
                    return SpriteManager.Get(CyclopsModule.SolarChargerMk2ID);
                default:
                    return null;
            }
        }

        public string GetIndicatorText()
        {
            switch (solarState)
            {
                case SolarState.SunAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(solarPercentage), NumberFormat.Sun);
                case SolarState.BatteryAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(SolarChargerMk2.TotalBatteryCharge), NumberFormat.Amount);
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
                    return NumberFormatter.GetNumberColor(SolarChargerMk2.TotalBatteryCharge, SolarChargerMk2.TotalBatteryCapacity, 0f);
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
            if (SolarChargers.Count == 0 && SolarChargerMk2.Count == 0)
            {
                solarState = SolarState.None;
                return 0f;
            }

            float solarStatus = GetSolarStatus(Cyclops);
            float availableSolarEnergy = SolarChargingFactor * solarStatus;
            solarPercentage = solarStatus * 100;

            if (availableSolarEnergy > PowerManager.MinimalPowerValue)
            {
                solarState = SolarState.SunAvailable;
                float mk1Power = SolarChargers.Count * availableSolarEnergy;
                float mk2Power = SolarChargerMk2.Count * availableSolarEnergy * PowerManager.Mk2ChargeRateModifier;

                SolarChargerMk2.RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (SolarChargerMk2.BatteryHasCharge)
            {
                solarState = SolarState.BatteryAvailable;
                return SolarChargerMk2.GetBatteryPower(PowerManager.BatteryDrainRate, requestedPower);
            }
            else
            {
                solarState = SolarState.None;
                return 0f;
            }
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
