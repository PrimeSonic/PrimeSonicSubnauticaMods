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

        private UpgradeHandler SolarChargerUpgrade => cySolarUpgradeHandler ?? (cySolarUpgradeHandler = MCUServices.Find.CyclopsUpgradeHandler(base.Cyclops, cySolarModule.TechType));

        public override float TotalReserveEnergy => 0f;

        public bool SolarEnergyAvailable { get; private set; }

        public TechType CrossModSolarCharger1;
        public TechType CrossModSolarCharger2;
        public bool OtherCySolarModsPresent = false;

        private const float MaxSolarDepth = 200f;
        private const float PercentageMaker = 100f;
        private const float SolarChargingFactor = 1.46f;
        private const float MinRequiredLight = 0.05f;
        private float lightRatio;
        private float depthRatio;
        private float rechargeRatio;

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
            return this.SolarEnergyAvailable ? NumberFormatter.FormatValue(energyStatus) + "%Θ" : string.Empty;
        }

        public override Color StatusTextColor()
        {
            return this.SolarEnergyAvailable ? NumberFormatter.GetNumberColor(energyStatus, 90f, 5f) : Color.white;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            return 0f;
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (OtherCySolarModsPresent && this.OtherSolarChargerModsEquipped)
            {
                // Does not stack with other solar charging mods
                this.SolarEnergyAvailable = false;
                return 0f;
            }

            if (this.SolarChargerUpgrade != null && this.SolarChargerUpgrade.HasUpgrade)
            {
                this.SolarEnergyAvailable = HasAmbientEnergy();

                if (this.SolarEnergyAvailable)
                    return rechargeRatio * DayNightCycle.main.deltaTime * SolarChargingFactor;
            }

            this.SolarEnergyAvailable = false;
            return 0f;
        }

        public bool OtherSolarChargerModsEquipped
        {
            get
            {
                return MCUServices.CrossMod.HasUpgradeInstalled(base.Cyclops, CrossModSolarCharger1) ||
                       MCUServices.CrossMod.HasUpgradeInstalled(base.Cyclops, CrossModSolarCharger2);
            }
        }

        private bool HasAmbientEnergy()
        {
            if (base.Cyclops.transform.position.y < -MaxSolarDepth)
            {
                energyStatus = 0f;
                return false;
            }

            depthRatio = Mathf.Clamp01((MaxSolarDepth + Cyclops.transform.position.y) / MaxSolarDepth);

            DayNightCycle daynightCycle = DayNightCycle.main;
            if (daynightCycle == null)
            {
                energyStatus = 0f;
                return false;
            }

            lightRatio = daynightCycle.GetLocalLightScalar();

            bool hasEnergy = lightRatio > MinRequiredLight;

            rechargeRatio = depthRatio * lightRatio;

            if (hasEnergy)
                energyStatus = rechargeRatio * PercentageMaker;

            return hasEnergy;
        }
    }
}
