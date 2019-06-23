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

        public override string Name { get; } = "McuSolChgr";
        protected override string PercentNotation => "%Θ";
        protected override float MaximumEnergyStatus => 90f;
        protected override float MinimumEnergyStatus => 1f;

        protected override float GetEnergyStatus()
        {
            DayNightCycle daynightCycle = DayNightCycle.main;

            if (daynightCycle == null)
                return 0f; // Safety check

            // This based on the how the Solar Panel and Seamoth generate solar power.
            return daynightCycle.GetLocalLightScalar() * // Sun Scalar
                   Mathf.Clamp01((MaxSolarDepth - Cyclops.transform.position.y) / MaxSolarDepth) * // Depth Scalar
                   PercentageMaker; // Make into percentage - will be cancled out later
        }

        protected override float ConvertToAvailableEnergy(float energyStatus)
        {
            return energyStatus * DayNightCycle.main.deltaTime * SolarChargingFactor;
        }
    }
}
