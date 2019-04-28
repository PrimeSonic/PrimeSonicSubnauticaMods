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
        private const float CooldownRate = MaxNuclearChargeRate / 2;
        private const float MinNuclearChargeRate = PowerManager.MinimalPowerValue * 2;
        private const float ChargeMultiplier = 0.95f;
        private const float MaxHeat = 500f;

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
            // Cooldown
            heat = Mathf.Max(0f, heat - CooldownRate);

            if (!this.NuclearCharger.BatteryHasCharge)
            {
                NuclearState = NuclearState.None;
                chargeRate = MinNuclearChargeRate;
                return 0f;
            }
            else if (this.HasRenewablePower)
            {
                NuclearState = NuclearState.RenewableEnergyAvailable;
                chargeRate = MinNuclearChargeRate;
                return 0f;
            }
            else if (requestedPower < NuclearModuleConfig.MinimumEnergyDeficit)
            {
                NuclearState = NuclearState.ConservingNuclearEnergy;
                chargeRate = MinNuclearChargeRate;
                return 0f;
            }
            else if (heat > MaxHeat)
            {
                NuclearState = NuclearState.Overheated;
                chargeRate = MinNuclearChargeRate;
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

                heat += generatedPower * this.NuclearCharger.Count;
                return ChargeMultiplier * generatedPower;
            }
        }
    }
}
