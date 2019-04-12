namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using Caching;
    using Managers;
    using Modules;
    using SaveData;
    using UnityEngine;

    internal enum NuclearState
    {
        None,
        RenewableEnergyAvailable,
        ConservingNuclearEnergy,
        NuclearPowerEngaged
    }

    internal class NuclearChargeHandler : ICyclopsCharger
    {
        internal const float NuclearDrainRate = 0.15f;

        private readonly ChargeManager ChargeManager;
        private SolarChargeHandler SolarCharger => ChargeManager.SolarCharging;
        private ThermalChargeHandler ThermalCharger => ChargeManager.ThermalCharging;
        private BioChargeHandler BioCharger => ChargeManager.BioCharging;
        internal BatteryUpgradeHandler NuclearCharger => ChargeManager.NuclearCharger;

        public readonly SubRoot Cyclops;

        internal NuclearState NuclearState = NuclearState.None;

        public NuclearChargeHandler(ChargeManager chargeManager)
        {
            ChargeManager = chargeManager;
            Cyclops = chargeManager.Cyclops;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return SpriteManager.Get(CyclopsModule.NuclearChargerID);
        }

        public string GetIndicatorText()
        {
            return NumberFormatter.FormatNumber(Mathf.CeilToInt(this.NuclearCharger.TotalBatteryCharge), NumberFormat.Amount);
        }

        public Color GetIndicatorTextColor()
        {
            return NumberFormatter.GetNumberColor(this.NuclearCharger.TotalBatteryCharge, this.NuclearCharger.TotalBatteryCapacity, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return NuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (!this.NuclearCharger.BatteryHasCharge)
            {
                NuclearState = NuclearState.None;
                return 0f;
            }
            else if (this.SolarCharger.SolarState != SolarState.None || this.ThermalCharger.ThermalState != ThermalState.None || this.BioCharger.ProducingPower)
            {
                NuclearState = NuclearState.RenewableEnergyAvailable;
                return 0f;
            }
            else if (requestedPower < NuclearModuleConfig.MinimumEnergyDeficit)
            {
                NuclearState = NuclearState.ConservingNuclearEnergy;
                return 0f;
            }
            else
            {
                NuclearState = NuclearState.NuclearPowerEngaged;
                return this.NuclearCharger.GetBatteryPower(NuclearDrainRate, requestedPower);
            }
        }
    }
}
