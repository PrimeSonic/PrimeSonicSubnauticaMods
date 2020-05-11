namespace CyclopsSimpleSolar
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class CySolarChargeManager : CyclopsCharger
    {
        private UpgradeHandler cySolarUpgradeHandler;
        private readonly CySolarModule cySolarModule;

        private UpgradeHandler AmbientEnergyUpgrade => cySolarUpgradeHandler ?? (cySolarUpgradeHandler = MCUServices.Find.CyclopsUpgradeHandler(base.Cyclops, cySolarModule.TechType));

        public override float TotalReserveEnergy => 0f;

        private const float MaxSolarDepth = 200f;
        private const float PercentageMaker = 100f;
        private const float SolarChargingFactor = 1.46f;
        private const float MinRequiredLight = 0.05f;
        private float lightRatio;
        private float depthRatio;
        private float rechargeRatio;
        private bool ambientEnergyAvailable = false;
        private float energyStatus = 0f;

        public CySolarChargeManager(CySolarModule solarModule, SubRoot cyclops) : base(cyclops)
        {
            cySolarModule = solarModule;
        }

        public override Atlas.Sprite StatusSprite()
        {
            return CySolarModule.CustomSprite;
        }

        public override string StatusText()
        {
            return ambientEnergyAvailable ? EnergyStatusText() : ReservePowerText();
        }

        internal string EnergyStatusText()
        {
            return NumberFormatter.FormatValue(energyStatus) + "%Θ";
        }

        internal string ReservePowerText()
        {
            return null;
        }

        public override Color StatusTextColor()
        {
            return NumberFormatter.GetNumberColor(energyStatus, 90f, 5f);
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            return 0f;
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade == null || !this.AmbientEnergyUpgrade.HasUpgrade)
            {
                ambientEnergyAvailable = false;
                return 0f;
            }

            ambientEnergyAvailable = HasAmbientEnergy(ref energyStatus);

            if (ambientEnergyAvailable)
                return rechargeRatio * DayNightCycle.main.deltaTime * SolarChargingFactor;

            return 0f;
        }

        protected bool HasAmbientEnergy(ref float ambientEnergyStatus)
        {
            ambientEnergyStatus = 0f;

            if (base.Cyclops.transform.position.y < -MaxSolarDepth)
                return false;

            depthRatio = Mathf.Clamp01((MaxSolarDepth + Cyclops.transform.position.y) / MaxSolarDepth);

            DayNightCycle daynightCycle = DayNightCycle.main;
            if (daynightCycle == null)
                return false;

            lightRatio = daynightCycle.GetLocalLightScalar();

            bool hasEnergy = lightRatio > MinRequiredLight;

            rechargeRatio = depthRatio * lightRatio;

            if (hasEnergy)
                ambientEnergyStatus = rechargeRatio * PercentageMaker;

            return hasEnergy;
        }
    }
}
