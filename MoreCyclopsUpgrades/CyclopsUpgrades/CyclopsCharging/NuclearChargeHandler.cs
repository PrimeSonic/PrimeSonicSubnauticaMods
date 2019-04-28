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
        NuclearPowerEngaged,
        Overheated
    }

    internal class NuclearChargeHandler : ICyclopsCharger
    {
        private const float MaxNuclearChargeRate = 0.16f;
        private const float MinNuclearChargeRate = PowerManager.MinimalPowerValue * 2;
        private const float CooldownRate = MaxNuclearChargeRate * 6f;
        private const float MaxHeat = 1200f;

        private readonly ChargeManager ChargeManager;
        private SolarChargeHandler SolarCharger => ChargeManager.SolarCharging;
        private ThermalChargeHandler ThermalCharger => ChargeManager.ThermalCharging;
        private BioChargeHandler BioCharger => ChargeManager.BioCharging;
        internal BatteryUpgradeHandler NuclearCharger => ChargeManager.NuclearCharger;

        private bool HasRenewablePower => this.SolarCharger.SolarState != SolarState.None ||
                                          this.ThermalCharger.ThermalState != ThermalState.None ||
                                          this.BioCharger.ProducingPower;

        private readonly Atlas.Sprite sprite = SpriteManager.Get(CyclopsModule.NuclearChargerID);

        public readonly SubRoot Cyclops;

        internal NuclearState NuclearState = NuclearState.None;
        private float heat = 0f;
        private float chargeRate = MinNuclearChargeRate;

        public NuclearChargeHandler(ChargeManager chargeManager)
        {
            ChargeManager = chargeManager;
            Cyclops = chargeManager.Cyclops;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return sprite;
        }

        public string GetIndicatorText()
        {
            // Show remaining energy
            return NumberFormatter.FormatNumber(Mathf.CeilToInt(this.NuclearCharger.TotalBatteryCharge), NumberFormat.Amount);
        }

        public Color GetIndicatorTextColor()
        {
            // Use color to inform heat levels
            return NumberFormatter.GetNumberColor(MaxHeat - heat, MaxHeat, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return NuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (NuclearState != NuclearState.NuclearPowerEngaged && heat > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                heat -= CooldownRate; // Cooldown
            }

            if (!this.NuclearCharger.BatteryHasCharge)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                NuclearState = NuclearState.None;
                return 0f;
            }
            else if (this.HasRenewablePower)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                NuclearState = NuclearState.RenewableEnergyAvailable;
                return 0f;
            }
            else if (requestedPower < NuclearModuleConfig.MinimumEnergyDeficit)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                NuclearState = NuclearState.ConservingNuclearEnergy;
                return 0f;
            }
            else if (heat >= MaxHeat)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                NuclearState = NuclearState.Overheated;
                return 0f;
            }
            else if (NuclearState == NuclearState.Overheated)
            {
                if (heat <= 0) // Do not allow nuclear power to charge again until heat has returned to zero
                    NuclearState = NuclearState.None;

                return 0f;
            }
            else
            {
                NuclearState = NuclearState.NuclearPowerEngaged;

                chargeRate = Mathf.Min(MaxNuclearChargeRate, chargeRate + MinNuclearChargeRate);

                float generatedPower = this.NuclearCharger.GetBatteryPower(chargeRate, requestedPower);

                heat += generatedPower;
                return generatedPower;
            }
        }
    }
}
