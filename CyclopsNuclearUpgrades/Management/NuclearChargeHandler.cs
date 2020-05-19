namespace CyclopsNuclearUpgrades.Management
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class NuclearChargeHandler : CyclopsCharger
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
        private const float HeatModifier = 2.05f;
        internal const float MaxHeatLoad = 300f;

        private readonly TechType nuclearModuleID;
        private readonly Atlas.Sprite sprite;
        private float chargeRate = MinNuclearChargeRate;
        private NuclearState nuclearState = NuclearState.None;

        private NuclearUpgradeHandler upgradeHandler;
        private NuclearUpgradeHandler NuclearHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(base.Cyclops, nuclearModuleID));

        internal bool IsOverheated => nuclearState == NuclearState.Overheated;
        internal float HeatLevel { get; private set; } = 0f;

        public override float TotalReserveEnergy => this.NuclearHandler.TotalBatteryCharge;

        public NuclearChargeHandler(SubRoot cyclops, TechType nuclearModule) : base(cyclops)
        {
            sprite = SpriteManager.Get(nuclearModule);
            nuclearModuleID = nuclearModule;
        }

        public override Atlas.Sprite StatusSprite()
        {
            return sprite;
        }

        public override string StatusText()
        {
            return NumberFormatter.FormatValue(this.NuclearHandler.TotalBatteryCharge);
        }

        public override Color StatusTextColor()
        {
            // Use color to inform heat levels
            return NumberFormatter.GetNumberColor(MaxHeatLoad - this.HeatLevel, MaxHeatLoad, 0f);
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (nuclearState != NuclearState.NuclearPowerEngaged && this.HeatLevel > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                this.HeatLevel -= CooldownRate * DayNightCycle.main.deltaTime; // Cooldown
            }

            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (this.NuclearHandler.TotalBatteryCharge < MinimalPowerValue)
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

                chargeRate = Mathf.Min(MaxNuclearChargeRate, chargeRate + MinNuclearChargeRate * DayNightCycle.main.deltaTime);

                float generatedPower = this.NuclearHandler.GetBatteryPower(chargeRate, requestedPower);

                this.HeatLevel += generatedPower * HeatModifier;

                return generatedPower;
            }
        }
    }
}
