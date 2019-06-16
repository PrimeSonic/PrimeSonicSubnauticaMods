namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal enum ThermalState
    {
        None,
        HeatAvailable,
        BatteryAvailable
    }

    internal class ThermalChargeHandler : ICyclopsCharger
    {
        internal const float BatteryDrainRate = ChargeManager.BatteryDrainRate;
        internal const float MinimalPowerValue = ChargeManager.MinimalPowerValue;
        internal const float Mk2ChargeRateModifier = ChargeManager.Mk2ChargeRateModifier;
        internal const float ThermalChargingFactor = 1.5f;

        internal readonly ChargeManager ChargeManager;
        internal ChargingUpgradeHandler ThermalChargers => ChargeManager.ThermalCharger;
        internal BatteryUpgradeHandler ThermalChargerMk2 => ChargeManager.ThermalChargerMk2;
        internal SolarChargeHandler SolarCharger => ChargeManager.SolarCharging;

        public bool IsRenewable { get; } = true;

        public readonly SubRoot Cyclops;

        private readonly Atlas.Sprite thermal1Sprite = SpriteManager.Get(TechType.CyclopsThermalReactorModule);
        private readonly Atlas.Sprite thermal2Sprite = SpriteManager.Get(CyclopsModule.ThermalChargerMk2ID);

        private const float MaxTemperature = 100f;
        private const float MinUsableTemperature = 25f;

        internal ThermalState ThermalState = ThermalState.None;
        private float temperature = 0f;

        public ThermalChargeHandler(ChargeManager chargeManager)
        {
            ChargeManager = chargeManager;
            Cyclops = chargeManager.Cyclops;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            switch (ThermalState)
            {
                case ThermalState.HeatAvailable:
                    return thermal1Sprite;
                case ThermalState.BatteryAvailable:
                    return thermal2Sprite;
                default:
                    return null;
            }
        }

        public string GetIndicatorText()
        {
            switch (ThermalState)
            {
                case ThermalState.HeatAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(temperature), NumberFormat.Temperature);
                case ThermalState.BatteryAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(this.ThermalChargerMk2.TotalBatteryCharge), NumberFormat.Amount);
                default:
                    return string.Empty;
            }
        }

        public Color GetIndicatorTextColor()
        {
            switch (ThermalState)
            {
                case ThermalState.HeatAvailable:
                    return NumberFormatter.GetNumberColor(temperature, MaxTemperature, MinUsableTemperature);
                case ThermalState.BatteryAvailable:
                    return NumberFormatter.GetNumberColor(this.ThermalChargerMk2.TotalBatteryCharge, this.ThermalChargerMk2.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public bool HasPowerIndicatorInfo()
        {
            return ThermalState != ThermalState.None;
        }

        public float ProducePower(float requestedPower)
        {
            if (this.ThermalChargers.Count == 0 && this.ThermalChargerMk2.Count == 0)
            {
                ThermalState = ThermalState.None;
                return 0f;
            }

            temperature = GetThermalStatus(Cyclops);
            float availableThermalEnergy = ThermalChargingFactor * Time.deltaTime * Cyclops.thermalReactorCharge.Evaluate(temperature);

            if (availableThermalEnergy > MinimalPowerValue)
            {
                ThermalState = ThermalState.HeatAvailable;
                float mk1Power = this.ThermalChargers.Count * availableThermalEnergy;
                float mk2Power = this.ThermalChargerMk2.Count * availableThermalEnergy * Mk2ChargeRateModifier;

                this.ThermalChargerMk2.RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (this.SolarCharger.SolarState != SolarState.SunAvailable && this.ThermalChargerMk2.BatteryHasCharge)
            {
                ThermalState = ThermalState.BatteryAvailable;
                return this.ThermalChargerMk2.GetBatteryPower(BatteryDrainRate, requestedPower);
            }
            else
            {
                ThermalState = ThermalState.None;
                return 0f;
            }
        }

        /// <summary>
        ///  Gets the amount of available energy provided by the current ambient heat.
        /// </summary>
        /// <returns>The currently available thermal energy.</returns>
        private static float GetThermalChargeAmount(SubRoot cyclops)
        {
            // This code mostly replicates what the UpdateThermalReactorCharge() method does from the SubRoot class

            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return ThermalChargingFactor *
                   Time.deltaTime *
                   cyclops.thermalReactorCharge.Evaluate(WaterTemperatureSimulation.main.GetTemperature(cyclops.transform.position)); // Temperature
        }

        private static float GetThermalStatus(SubRoot cyclops)
        {
            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return WaterTemperatureSimulation.main.GetTemperature(cyclops.transform.position);
        }
    }
}
