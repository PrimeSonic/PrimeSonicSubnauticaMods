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

        internal const string ChargerName = "CyNukeMChgr";
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float MaxNuclearChargeRate = 0.15f;
        private const float MinNuclearChargeRate = MinimalPowerValue * 3;
        private const float CooldownRate = MaxNuclearChargeRate * 6f;
        private const float HeatModifier = 1.05f;
        private const float MaxHeatLoad = 1500f;
        internal const float MaxHeat = MaxHeatLoad / 500f; // This makes the max heat look like 300°C

        private readonly TechType nuclearModuleID;
        private readonly Atlas.Sprite sprite;
        private readonly SubRoot cyclopsSub;
        private float chargeRate = MinNuclearChargeRate;
        private NuclearState nuclearState = NuclearState.None;

        private NuclearUpgradeHandler upgradeHandler;
        private NuclearUpgradeHandler NuclearHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(cyclopsSub, nuclearModuleID));

        private float heatLevel = 0f;

        internal bool IsOverheated => nuclearState == NuclearState.Overheated;
        internal float HeatLevel => heatLevel / 500f;  // This makes the max heat look like 300°C

        public NuclearChargeHandler(SubRoot cyclops, TechType nuclearModule)
        {
            sprite = SpriteManager.Get(nuclearModule);
            cyclopsSub = cyclops;
            nuclearModuleID = nuclearModule;
        }

        public bool IsRenewable { get; } = false;
        public string Name { get; } = ChargerName;

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
            return NumberFormatter.GetNumberColor(MaxHeatLoad - heatLevel, MaxHeatLoad, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return nuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (nuclearState != NuclearState.NuclearPowerEngaged && heatLevel > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                heatLevel -= CooldownRate; // Cooldown
            }

            if (this.NuclearHandler == null || this.NuclearHandler.TotalBatteryCharge <= MinimalPowerValue)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.None;
                return 0f;
            }
            else if (heatLevel >= MaxHeatLoad)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.Overheated;
                this.NuclearHandler.TooHotToHandle = true;
                return 0f;
            }
            else if (nuclearState == NuclearState.Overheated)
            {
                if (heatLevel <= 0) // Do not allow nuclear power to charge again until heat has returned to zero
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

                heatLevel += generatedPower * HeatModifier;

                return generatedPower;
            }
        }

        public float TotalReservePower()
        {
            return this.NuclearHandler.TotalBatteryCharge;
        }
    }
}
