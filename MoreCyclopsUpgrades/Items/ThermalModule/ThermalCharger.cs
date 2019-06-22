namespace MoreCyclopsUpgrades.Items.ThermalModule
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using UnityEngine;

    internal enum ThermalState
    {
        None,
        HeatAvailable,
        BatteryAvailable
    }

    internal class ThermalCharger : ICyclopsCharger
    {
        internal const string ChargerName = "McuHeatChgr";
        internal const float BatteryDrainRate = ChargeManager.BatteryDrainRate;
        internal const float MinimalPowerValue = ChargeManager.MinimalPowerValue;
        internal const float Mk2ChargeRateModifier = ChargeManager.Mk2ChargeRateModifier;
        internal const float ThermalChargingFactor = 1.5f;

        public bool IsRenewable { get; } = true;

        public string Name { get; } = ChargerName;

        private readonly ThermalUpgrade upgradeHandler;
        private readonly SubRoot Cyclops;

        private readonly TechType thermalMk2;
        private readonly Atlas.Sprite thermal1Sprite;
        private readonly Atlas.Sprite thermal2Sprite;

        private const float MaxTemperature = 100f;
        private const float MinUsableTemperature = 25f;

        internal ThermalState ThermalState = ThermalState.None;
        private float temperature = 0f;

        public ThermalCharger(SubRoot cyclops, TechType thermalMk2Module)
        {
            Cyclops = cyclops;
            thermalMk2 = thermalMk2Module;

            thermal1Sprite = SpriteManager.Get(TechType.CyclopsThermalReactorModule);
            thermal2Sprite = SpriteManager.Get(thermalMk2Module);

            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<ThermalUpgrade>(cyclops, TechType.CyclopsThermalReactorModule, thermalMk2);
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
                    return NumberFormatter.FormatTemperature(temperature);
                case ThermalState.BatteryAvailable:
                    return NumberFormatter.FormatValue(upgradeHandler.TotalBatteryCharge);
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
                    return NumberFormatter.GetNumberColor(upgradeHandler.TotalBatteryCharge, upgradeHandler.TotalBatteryCapacity, 0f);
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
            if (upgradeHandler.Count == 0)
            {
                ThermalState = ThermalState.None;
                return 0f;
            }

            temperature = GetThermalStatus(Cyclops);

            if (temperature > MinUsableTemperature)
            {
                float availableThermalEnergy = ThermalChargingFactor * Time.deltaTime * Cyclops.thermalReactorCharge.Evaluate(temperature);

                ThermalState = ThermalState.HeatAvailable;
                float thermalEnergy = upgradeHandler.Count * upgradeHandler.ChargeMultiplier;

                if (requestedPower < thermalEnergy)
                    upgradeHandler.RechargeBatteries(thermalEnergy - requestedPower);

                return thermalEnergy;
            }
            else if (upgradeHandler.TotalBatteryCharge > MinimalPowerValue)
            {
                ThermalState = ThermalState.BatteryAvailable;
                return upgradeHandler.GetBatteryPower(BatteryDrainRate, requestedPower);
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

        public float TotalReservePower()
        {
            return upgradeHandler.TotalBatteryCharge;
        }
    }
}
