namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API.AmbientEnergy;
    using UnityEngine;

    internal class SolarCharger : AmbientEnergyCharger<SolarUpgradeHandler>
    {
        private const float MaxSolarDepth = 200f;
        private const float PercentageMaker = 100f;
        private const float SolarChargingFactor = 1.45f;

        private float lightRatio;
        private float depthRatio;
        private float rechargeRatio;

        public SolarCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops)
            : base(tier1TechType, tier2TechType, cyclops)
        {
        }

        protected override string PercentNotation => "%Θ";
        protected override float MaximumEnergyStatus => 90f;
        protected override float MinimumEnergyStatus => 5f;

        protected override bool HasAmbientEnergy(ref float ambientEnergyStatus)
        {
            ambientEnergyStatus = 0f;

            if (Cyclops.transform.position.y < -MaxSolarDepth)
                return false;

            depthRatio = Mathf.Clamp01((MaxSolarDepth + Cyclops.transform.position.y) / MaxSolarDepth);

            DayNightCycle daynightCycle = DayNightCycle.main;
            if (daynightCycle == null)
                return false;

            lightRatio = daynightCycle.GetLocalLightScalar();

            bool hasEnergy = lightRatio > 0.05f;

            rechargeRatio = depthRatio * lightRatio;

            if (hasEnergy)
                ambientEnergyStatus = rechargeRatio * PercentageMaker;

            return hasEnergy;
        }

        protected override float GetAmbientEnergy()
        {
            return rechargeRatio * DayNightCycle.main.deltaTime * SolarChargingFactor;
        }
    }
}
