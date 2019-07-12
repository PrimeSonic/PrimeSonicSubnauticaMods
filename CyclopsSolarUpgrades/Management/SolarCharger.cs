namespace CyclopsSolarUpgrades.Management
{
    using CommonCyclopsUpgrades;
    using UnityEngine;

    internal class SolarCharger : AmbientEnergyCharger<SolarUpgradeHandler>
    {
        private const float MaxSolarDepth = 200f;
        private const float PercentageMaker = 100f;
        private const float SolarChargingFactor = 1.25f / PercentageMaker;

        public SolarCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops)
            : base(tier1TechType, tier2TechType, cyclops)
        {
        }

        protected override string PercentNotation => "%Θ";
        protected override float MaximumEnergyStatus => 90f;
        protected override float MinimumEnergyStatus => 10f;

        protected override void UpdateEnergyStatus(ref float ambientEnergyStatus)
        {
            DayNightCycle daynightCycle = DayNightCycle.main;

            if (daynightCycle == null)
            {
                ambientEnergyStatus = 0f; // Safety check
                return;
            }

            // This based on the how the Solar Panel and Seamoth generate solar power.
            ambientEnergyStatus = daynightCycle.GetLocalLightScalar() * // Sun Scalar
                                  Mathf.Clamp01((MaxSolarDepth - Cyclops.transform.position.y) / MaxSolarDepth) * // Depth Scalar
                                  PercentageMaker; // Make into percentage - will be cancled out later
        }

        protected override float ConvertToAvailableEnergy(float energyStatus)
        {
            return energyStatus * DayNightCycle.main.deltaTime * SolarChargingFactor;
        }
    }
}
