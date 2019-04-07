namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using UnityEngine;

    internal class ThermalChargeHandler : CyclopsCharger
    {
        private enum ThermalState
        {
            None,
            HeatAvailable,
            BatteryAvailable
        }

        internal const float ThermalChargingFactor = 1.5f;
        internal const float BatteryDrainRate = 0.01f;
        internal const float Mk2ChargeRateModifier = 1.15f;

        internal readonly ChargingUpgradeHandler ThermalChargers;
        internal readonly BatteryUpgradeHandler ThermalChargerMk2;

        private const float MaxTemperature = 100f;
        private const float MinUsableTemperature = 25f;

        private ThermalState thermalState = ThermalState.None;
        private float temperature = 0f;

        public ThermalChargeHandler(SubRoot cyclops,
                                    ChargingUpgradeHandler thermalChargers,
                                    BatteryUpgradeHandler thermalChargerMk2) : base(cyclops)
        {
            ThermalChargers = thermalChargers;
            ThermalChargerMk2 = thermalChargerMk2;
        }

        public override Atlas.Sprite GetIndicatorSprite()
        {
            switch (thermalState)
            {
                case ThermalState.HeatAvailable:
                    return SpriteManager.Get(TechType.CyclopsThermalReactorModule);
                case ThermalState.BatteryAvailable:
                    return SpriteManager.Get(CyclopsModule.ThermalChargerMk2ID);
                default:
                    return null;
            }
        }

        public override string GetIndicatorText()
        {
            switch (thermalState)
            {
                case ThermalState.HeatAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(temperature), NumberFormat.Temperature);
                case ThermalState.BatteryAvailable:
                    return NumberFormatter.FormatNumber(Mathf.CeilToInt(ThermalChargerMk2.TotalBatteryCharge), NumberFormat.Amount);
                default:
                    return string.Empty;
            }
        }

        public override Color GetIndicatorTextColor()
        {
            switch (thermalState)
            {
                case ThermalState.HeatAvailable:
                    return NumberFormatter.GetNumberColor(temperature, MaxTemperature, MinUsableTemperature);
                case ThermalState.BatteryAvailable:
                    return NumberFormatter.GetNumberColor(ThermalChargerMk2.TotalBatteryCharge, ThermalChargerMk2.TotalBatteryCapacity, 0f);
                default:
                    return Color.white;
            }
        }

        public override bool HasPowerIndicatorInfo()
        {
            return thermalState != ThermalState.None;
        }

        public override float ProducePower(float requestedPower)
        {
            if (ThermalChargers.Count == 0 && ThermalChargerMk2.Count == 0)
            {
                thermalState = ThermalState.None;
                return 0f;
            }

            temperature = GetThermalStatus(base.Cyclops);
            float availableThermalEnergy = ThermalChargingFactor * Time.deltaTime * base.Cyclops.thermalReactorCharge.Evaluate(temperature);

            if (availableThermalEnergy > PowerManager.MinimalPowerValue)
            {
                thermalState = ThermalState.HeatAvailable;
                float mk1Power = ThermalChargers.Count * availableThermalEnergy;
                float mk2Power = ThermalChargerMk2.Count * availableThermalEnergy * PowerManager.Mk2ChargeRateModifier;

                ThermalChargerMk2.RechargeBatteries(mk1Power + mk2Power);

                return mk1Power + mk2Power;
            }
            else if (ThermalChargerMk2.BatteryHasCharge)
            {
                thermalState = ThermalState.BatteryAvailable;
                return ThermalChargerMk2.GetBatteryPower(PowerManager.BatteryDrainRate, requestedPower);
            }
            else
            {
                thermalState = ThermalState.None;
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
