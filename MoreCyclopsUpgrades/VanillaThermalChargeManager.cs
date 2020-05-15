namespace MoreCyclopsUpgrades
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class VanillaThermalChargeManager : CyclopsCharger
    {
        private UpgradeHandler cyThermalUpgradeHandler;
        private UpgradeHandler ThermalChargerUpgrade => cyThermalUpgradeHandler ?? (cyThermalUpgradeHandler = MCUServices.Find.CyclopsUpgradeHandler(base.Cyclops, TechType.CyclopsThermalReactorModule));

        public override float TotalReserveEnergy => 0f;

        public bool ThermalEnergyAvailable { get; private set; }
        public float Temperature { get; private set; }

        private const float ThermalChargingFactor = 1.5f;
        private const float MinTemperatureForCharge = 35f;

        public VanillaThermalChargeManager(SubRoot cyclops) : base(cyclops)
        {
        }

        public override Atlas.Sprite StatusSprite()
        {
            return SpriteManager.Get(TechType.CyclopsThermalReactorModule);
        }

        public override string StatusText()
        {
            return this.ThermalEnergyAvailable ? NumberFormatter.FormatValue(this.Temperature) + "°C" : string.Empty;
        }

        public override Color StatusTextColor()
        {
            return this.ThermalEnergyAvailable ? NumberFormatter.GetNumberColor(this.Temperature, 90f, MinTemperatureForCharge) : Color.white;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            return 0f;
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.ThermalChargerUpgrade != null && this.ThermalChargerUpgrade.HasUpgrade)
            {
                this.ThermalEnergyAvailable = HasAmbientEnergy();

                if (this.ThermalEnergyAvailable) // The exact same numbers as the vanilla code
                    return base.Cyclops.thermalReactorCharge.Evaluate(this.Temperature) * DayNightCycle.main.deltaTime * ThermalChargingFactor;
            }

            this.ThermalEnergyAvailable = false;
            return 0f;
        }

        private bool HasAmbientEnergy()
        {
            if (!base.Cyclops.thermalReactorUpgrade)
            {
                this.Temperature = 0f;
                return false;
            }

            this.Temperature = base.Cyclops.GetTemperature();

            return this.Temperature > MinTemperatureForCharge;
        }
    }
}
