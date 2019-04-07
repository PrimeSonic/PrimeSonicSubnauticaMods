namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.SaveData;
    using UnityEngine;

    internal class NuclearChargeHandler : ICyclopsCharger
    {
        private enum NuclearState
        {
            None,
            RenewableEnergyAvailable,
            ConservingNuclearEnergy,
            NuclearPowerEngaged
        }

        internal const float NuclearDrainRate = 0.15f;

        private readonly SolarChargeHandler solarCharger;
        private readonly ThermalChargeHandler thermalCharger;
        private readonly BioChargeHandler bioCharger;
        internal readonly BatteryUpgradeHandler NuclearCharger;

        public readonly SubRoot Cyclops;

        private NuclearState nuclearState = NuclearState.None;

        public NuclearChargeHandler(SubRoot cyclops,
                                    BatteryUpgradeHandler nuclearCharger,
                                    SolarChargeHandler solarChargHandler,
                                    ThermalChargeHandler thermalChargHandler,
                                    BioChargeHandler bioChargHandler)
        {
            Cyclops = cyclops;
            NuclearCharger = nuclearCharger;
            solarCharger = solarChargHandler;
            thermalCharger = thermalChargHandler;
            bioCharger = bioChargHandler;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return SpriteManager.Get(CyclopsModule.NuclearChargerID);
        }

        public string GetIndicatorText()
        {
            return NumberFormatter.FormatNumber(Mathf.CeilToInt(NuclearCharger.TotalBatteryCharge), NumberFormat.Amount);
        }

        public Color GetIndicatorTextColor()
        {
            return NumberFormatter.GetNumberColor(NuclearCharger.TotalBatteryCharge, NuclearCharger.TotalBatteryCapacity, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return nuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (NuclearCharger.Count == 0)
            {
                nuclearState = NuclearState.None;
                return 0f;
            }
            else if (solarCharger.HasPowerIndicatorInfo() || thermalCharger.HasPowerIndicatorInfo() || bioCharger.HasPowerIndicatorInfo())
            {
                nuclearState = NuclearState.RenewableEnergyAvailable;
                return 0f;
            }
            else if (requestedPower < NuclearModuleConfig.MinimumEnergyDeficit)
            {
                nuclearState = NuclearState.ConservingNuclearEnergy;
                return 0f;
            }
            else
            {
                nuclearState = NuclearState.NuclearPowerEngaged;
                return NuclearCharger.GetBatteryPower(NuclearDrainRate, requestedPower);
            }
        }
    }
}
