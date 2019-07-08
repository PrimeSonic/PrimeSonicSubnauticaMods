namespace CyclopsNuclearUpgrades.Management
{
    using CommonCyclopsUpgrades;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class NuclearChargeHandler : ICyclopsCharger
    {
        internal enum NuclearState
        {
            None,
            ConservingNuclearEnergy,
            NuclearPowerEngaged,
            Overheated
        }

        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float MaxNuclearChargeRate = MinimalPowerValue * 20000f;
        private const float MinNuclearChargeRate = MinimalPowerValue * 100f;
        private const float CooldownRate = MinNuclearChargeRate * 1.5f;
        private const float HeatModifier = 2.5f;
        private const float MaxHeatLoad = 3000f;
        private const float DisplayNormalizer = 10f; // This makes the max heat look like 300°C
        internal const float MaxHeat = MaxHeatLoad / DisplayNormalizer;

        private readonly TechType nuclearModuleID;
        private readonly Atlas.Sprite sprite;
        private readonly SubRoot cyclopsSub;
        private float chargeRate = MinNuclearChargeRate;
        private NuclearState nuclearState = NuclearState.None;

        private NuclearUpgradeHandler upgradeHandler;
        private float _heatLevel = 0f;

        private NuclearUpgradeHandler NuclearHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(cyclopsSub, nuclearModuleID));

        internal bool IsOverheated => nuclearState == NuclearState.Overheated;
        internal float DisplayedHeatLevel => _heatLevel / DisplayNormalizer;

        public NuclearChargeHandler(SubRoot cyclops, TechType nuclearModule)
        {
            sprite = SpriteManager.Get(nuclearModule);
            cyclopsSub = cyclops;
            nuclearModuleID = nuclearModule;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return sprite;
        }

        public string GetIndicatorText()
        {
            return NumberFormatter.FormatValue(this.NuclearHandler.TotalBatteryCharge);
        }

        public Color GetIndicatorTextColor()
        {
            // Use color to inform heat levels
            return NumberFormatter.GetNumberColor(MaxHeatLoad - _heatLevel, MaxHeatLoad, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return nuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (nuclearState != NuclearState.NuclearPowerEngaged && _heatLevel > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                _heatLevel -= CooldownRate; // Cooldown
            }

            if (this.NuclearHandler == null || this.NuclearHandler.TotalBatteryCharge <= MinimalPowerValue)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.None;
                return 0f;
            }
            else if (_heatLevel >= MaxHeatLoad)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.Overheated;
                this.NuclearHandler.TooHotToHandle = true;
                return 0f;
            }
            else if (nuclearState == NuclearState.Overheated)
            {
                if (_heatLevel <= 0) // Do not allow nuclear power to charge again until heat has returned to zero
                {
                    nuclearState = NuclearState.None;
                    this.NuclearHandler.TooHotToHandle = false;
                }
                return 0f;
            }
            else
            {
                nuclearState = NuclearState.NuclearPowerEngaged;

                chargeRate = Mathf.Min(MaxNuclearChargeRate, chargeRate + MinNuclearChargeRate);

                float generatedPower = this.NuclearHandler.GetBatteryPower(chargeRate, requestedPower);

                _heatLevel += generatedPower * HeatModifier;

                return generatedPower;
            }
        }

        public float TotalReservePower()
        {
            return this.NuclearHandler.TotalBatteryCharge;
        }
    }
}
