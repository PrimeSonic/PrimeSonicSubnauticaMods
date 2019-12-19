namespace CyclopsThermalUpgrades.Management
{
    using MoreCyclopsUpgrades.API.AmbientEnergy;

    internal class ThermalCharger : AmbientEnergyCharger<ThermalUpgradeHandler>
    {
        private const float ThermalChargingFactor = 1.5f;

        private float temperature;

        public ThermalCharger(TechType tier2Id2, SubRoot cyclops)
            : base(TechType.CyclopsThermalReactorModule, tier2Id2, cyclops)
        {
        }

        protected override string PercentNotation => "°C";
        protected override float MaximumEnergyStatus => 100f;
        protected override float MinimumEnergyStatus => 35f;

        protected override bool HasAmbientEnergy(ref float ambientEnergyStatus)
        {
            ambientEnergyStatus = 0f;

            if (WaterTemperatureSimulation.main == null)
                return false;

            ambientEnergyStatus = temperature = WaterTemperatureSimulation.main.GetTemperature(base.Cyclops.transform.position);

            return temperature > 35f;
        }

        protected override float GetAmbientEnergy()
        {
            return ThermalChargingFactor *
                   DayNightCycle.main.deltaTime *
                   base.Cyclops.thermalReactorCharge.Evaluate(temperature);
        }
    }
}
