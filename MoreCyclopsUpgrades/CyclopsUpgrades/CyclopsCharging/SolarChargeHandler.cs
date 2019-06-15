namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal enum SolarState
    {
        None,
        SunAvailable,
        BatteryAvailable
    }

    internal class SolarChargeHandler : ICyclopsCharger
    {
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float MaxSolarPercentage = 90f;

        internal readonly ChargeManager ChargeManager;
        internal ChargingUpgradeHandler SolarChargers => ChargeManager.SolarCharger;
        internal BatteryUpgradeHandler SolarChargerMk2 => ChargeManager.SolarChargerMk2;
        internal ThermalChargeHandler ThermalCharginer => ChargeManager.ThermalCharging;

        public bool IsRenewable { get; } = true;

        public readonly SubRoot Cyclops;

        private readonly Atlas.Sprite solar1Sprite = SpriteManager.Get(CyclopsModule.SolarChargerID);
        private readonly Atlas.Sprite solar2Sprite = SpriteManager.Get(CyclopsModule.SolarChargerMk2ID);

        internal SolarState SolarState = SolarState.None;
        private float solarPercentage = 0f;

        public SolarChargeHandler(ChargeManager chargeManager)
        {
            Cyclops = chargeManager.Cyclops;
            ChargeManager = chargeManager;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            switch (SolarState)
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
            switch (SolarState)
            {
                case SolarState.SunAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(solarPercentage), NumberFormat.Sun);
                case SolarState.BatteryAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(this.SolarChargerMk2.TotalBatteryCharge), NumberFormat.Amount);
                default:
                    return string.Empty;
            }
        }

        public Color GetIndicatorTextColor()
        {
            switch (SolarState)
            {
                case SolarState.SunAvailable:
                    return NumberFormatter.GetNumberColor(solarPercentage, MaxSolarPercentage, 0f);
                case SolarState.BatteryAvailable:
                    return NumberFormatter.GetNumberColor(this.SolarChargerMk2.TotalBatteryCharge, this.SolarChargerMk2.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public bool HasPowerIndicatorInfo()
        {
            return SolarState != SolarState.None;
        }

        public float ProducePower(float requestedPower)
        {
            if (this.SolarChargers.Count == 0 && this.SolarChargerMk2.Count == 0)
            {
                SolarState = SolarState.None;
                return 0f;
            }

            float solarStatus = GetSolarStatus(Cyclops);
            float availableSolarEnergy = SolarChargingFactor * solarStatus;
            solarPercentage = solarStatus * 100;

            if (availableSolarEnergy > PowerManager.MinimalPowerValue)
            {
                SolarState = SolarState.SunAvailable;
                float mk1Power = this.SolarChargers.Count * availableSolarEnergy;
                float mk2Power = this.SolarChargerMk2.Count * availableSolarEnergy * PowerManager.Mk2ChargeRateModifier;

                this.SolarChargerMk2.RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (this.ThermalCharginer.ThermalState != ThermalState.HeatAvailable && this.SolarChargerMk2.BatteryHasCharge)
            {
                SolarState = SolarState.BatteryAvailable;
                return this.SolarChargerMk2.GetBatteryPower(PowerManager.BatteryDrainRate, requestedPower);
            }
            else
            {
                SolarState = SolarState.None;
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
