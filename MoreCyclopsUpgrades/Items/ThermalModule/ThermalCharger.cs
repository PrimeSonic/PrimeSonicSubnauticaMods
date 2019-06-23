namespace MoreCyclopsUpgrades.Items.ThermalModule
{
    using CommonCyclopsUpgrades;
    using UnityEngine;

    internal class ThermalCharger : AmbientEnergyCharger<ThermalUpgradeHandler>
    {
        private const float ThermalChargingFactor = 1.5f;

        public ThermalCharger(TechType tier2Id2, SubRoot cyclops)
            : base(TechType.CyclopsThermalReactorModule, tier2Id2, cyclops)
        {
        }

        public override string Name { get; } = "McuHeatChgr";

        protected override string PercentNotation => "°C";
        protected override float MaximumEnergyStatus => 100f;
        protected override float MinimumEnergyStatus => 25f;

        protected override float GetEnergyStatus()
        {
            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return WaterTemperatureSimulation.main.GetTemperature(base.Cyclops.transform.position);
        }

        protected override float ConvertToAvailableEnergy(float energyStatus)
        {
            // This is based on the original Cyclops thermal charging code
            return ThermalChargingFactor *
                   Time.deltaTime *
                   base.Cyclops.thermalReactorCharge.Evaluate(energyStatus); // Temperature
        }
    }
}
