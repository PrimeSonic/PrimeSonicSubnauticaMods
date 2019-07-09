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
        private const float MaxNuclearChargeRate = 20.0f;
        private const float MinNuclearChargeRate = 0.12f;
        private const float CooldownRate = 5.0f;
        private const float HeatModifier = 2.0f;
        internal const float MaxHeatLoad = 300f;

        private readonly TechType nuclearModuleID;
        private readonly Atlas.Sprite sprite;
        private readonly SubRoot cyclopsSub;
        private float chargeRate = MinNuclearChargeRate;
        private NuclearState nuclearState = NuclearState.None;

        private NuclearUpgradeHandler upgradeHandler;
        private NuclearUpgradeHandler NuclearHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(cyclopsSub, nuclearModuleID));

        internal bool IsOverheated => nuclearState == NuclearState.Overheated;
        internal float HeatLevel { get; private set; } = 0f;

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
            return NumberFormatter.GetNumberColor(MaxHeatLoad - this.HeatLevel, MaxHeatLoad, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return nuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (nuclearState != NuclearState.NuclearPowerEngaged && this.HeatLevel > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                this.HeatLevel -= CooldownRate * Time.deltaTime; // Cooldown
            }

            if (requestedPower < MinimalPowerValue || this.NuclearHandler == null || this.NuclearHandler.TotalBatteryCharge <= MinimalPowerValue)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.None;
                return 0f;
            }
            else if (this.HeatLevel >= MaxHeatLoad)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.Overheated;
                this.NuclearHandler.TooHotToHandle = true;
                return 0f;
            }
            else if (nuclearState == NuclearState.Overheated)
            {
                if (this.HeatLevel <= 0) // Do not allow nuclear power to charge again until heat has returned to zero
                {
                    nuclearState = NuclearState.None;
                    this.NuclearHandler.TooHotToHandle = false;
                }
                return 0f;
            }
            else
            {
                nuclearState = NuclearState.NuclearPowerEngaged;

                chargeRate = Mathf.Min(MaxNuclearChargeRate, chargeRate + MinNuclearChargeRate * Time.deltaTime);

                float generatedPower = this.NuclearHandler.GetBatteryPower(chargeRate, requestedPower);

                this.HeatLevel += generatedPower * HeatModifier;

                return generatedPower;
            }
        }

        public float TotalReservePower()
        {
            return this.NuclearHandler.TotalBatteryCharge;
        }
    }
}
