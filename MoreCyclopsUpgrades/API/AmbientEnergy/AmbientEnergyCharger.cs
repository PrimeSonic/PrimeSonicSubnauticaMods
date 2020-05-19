namespace MoreCyclopsUpgrades.API.AmbientEnergy
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    /// <summary>
    /// A generic <see cref="CyclopsCharger"/> for upgrade modules that draw in ambient energy like the solar or thermal.
    /// </summary>
    /// <typeparam name="T">The upgrade handler that implements <see cref="AmbientEnergyUpgradeHandler"/></typeparam>
    /// <seealso cref="CyclopsCharger" />
    public abstract class AmbientEnergyCharger<T> : CyclopsCharger
        where T : AmbientEnergyUpgradeHandler
    {
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 2.0f;

        private bool ambientEnergyAvailable = false;

        /// <summary>
        /// Gets the currently available ambient power as a string in percent notation.
        /// </summary>
        /// <value>
        /// The percent notation of the available ambient power.
        /// </value>
        protected abstract string PercentNotation { get; }

        /// <summary>
        /// Gets the highest possible value of available ambient energy. Used for text color calculation.
        /// </summary>
        /// <value>
        /// The maximum energy status.
        /// </value>
        protected abstract float MaximumEnergyStatus { get; }

        /// <summary>
        /// Gets the minimum ambient energy available required before energy production can take place. Used for text color calculation.
        /// </summary>
        /// <value>
        /// The minimum energy status.
        /// </value>
        protected abstract float MinimumEnergyStatus { get; }

        /// <summary>
        /// Returns the total charge available across all batteries for this charger.
        /// </summary>
        public override float TotalReserveEnergy => this.AmbientEnergyUpgrade.TotalBatteryCharge;

        private T energyUpgrade;

        /// <summary>
        /// Gets the ambient energy upgrade handler.
        /// </summary>
        /// <value>
        /// The ambient energy upgrade handler.
        /// </value>
        protected T AmbientEnergyUpgrade => energyUpgrade ?? (energyUpgrade = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(Cyclops, tier1Id, tier2Id2));

        private readonly Atlas.Sprite tier1Sprite;
        private readonly Atlas.Sprite tier2Sprite;

        private readonly TechType tier1Id;
        private readonly TechType tier2Id2;

        private float energyStatus = 0f;
        private float resultingEnergy = 0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientEnergyCharger{T}"/> class.
        /// </summary>
        /// <param name="tier1TechType">TechType value for the tier 1 upgrade module for this charger.</param>
        /// <param name="tier2TechType">TechType value for the tier 2 upgrade module for this charger.</param>
        /// <param name="cyclops">The cyclops reference.</param>
        protected AmbientEnergyCharger(TechType tier1TechType, TechType tier2TechType, SubRoot cyclops) : base(cyclops)
        {
            tier1Id = tier1TechType;
            tier2Id2 = tier2TechType;
            tier1Sprite = SpriteManager.Get(tier1TechType);
            tier2Sprite = SpriteManager.Get(tier2TechType);
        }

        /// <summary>
        /// Determines whether there is any ambient energy available, setting the parameter by reference to the current ambient energy level.
        /// </summary>
        /// <param name="ambientEnergyStatus">The current ambient energy status.</param>
        /// <returns>
        ///   <c>true</c> if there is enough ambient energy to start producing power; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool HasAmbientEnergy(ref float ambientEnergyStatus);

        /// <summary>
        /// Gets the raw amount of ambient energy.
        /// </summary>
        /// <returns>The raw amount of ambient energy.</returns>
        protected abstract float GetAmbientEnergy();

        /// <summary>
        /// Gets the sprite to use for the power indicator. This will only be called when <see cref="CyclopsCharger.ShowStatusIcon" /> returns <c>true</c>.
        /// </summary>
        /// <returns>
        /// A new <see cref="Atlas.Sprite" /> to be used in the Cyclops Helm and Holographic HUDs.
        /// </returns>
        public override Atlas.Sprite StatusSprite()
        {
            return ambientEnergyAvailable ? tier1Sprite : tier2Sprite;
        }

        /// <summary>
        /// Gets the text to use under the power indicator icon. This will only be called when <see cref="CyclopsCharger.ShowStatusIcon" /> returns <c>true</c>.
        /// </summary>
        /// <returns>
        /// A <see cref="string" />, ready to use for in-game text. Should be limited to numeric values if possible.
        /// </returns>
        public override string StatusText()
        {
            return ambientEnergyAvailable ? EnergyStatusText() : ReservePowerText();
        }

        internal string EnergyStatusText()
        {
            return NumberFormatter.FormatValue(energyStatus) + this.PercentNotation;
        }

        internal string ReservePowerText()
        {
            return NumberFormatter.FormatValue(this.AmbientEnergyUpgrade.TotalBatteryCharge);
        }

        /// <summary>
        /// Gets the color of the text used under the power indicator icon. This will only be called when <see cref="CyclopsCharger.ShowStatusIcon" /> returns <c>true</c>.
        /// </summary>
        /// <returns>
        /// A Unity <see cref="Color" /> value for the text. When in doubt, just set this to white.
        /// </returns>
        public override Color StatusTextColor()
        {
            return ambientEnergyAvailable
                ? NumberFormatter.GetNumberColor(energyStatus, this.MaximumEnergyStatus, this.MinimumEnergyStatus)
                : NumberFormatter.GetNumberColor(this.AmbientEnergyUpgrade.TotalBatteryCharge, this.AmbientEnergyUpgrade.TotalBatteryCapacity, 0f);
        }

        /// <summary>
        /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para />
        /// Use this for method rechargable energy drawn from the environment is isn't limited by a material resource.<para />
        /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para />
        /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para />
        /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.<para /></summary>
        /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
        /// <returns>
        /// The amount of power produced by this cyclops charger.
        /// </returns>
        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.AmbientEnergyUpgrade == null || this.AmbientEnergyUpgrade.Count == 0)
            {
                ambientEnergyAvailable = false;
                return 0f;
            }

            ambientEnergyAvailable = HasAmbientEnergy(ref energyStatus);

            if (ambientEnergyAvailable)
            {
                resultingEnergy = this.AmbientEnergyUpgrade.ChargeMultiplier * GetAmbientEnergy();

                if (requestedPower < resultingEnergy)
                    this.AmbientEnergyUpgrade.RechargeBatteries(resultingEnergy - requestedPower);

                return resultingEnergy;
            }

            return 0f;
        }

        /// <summary>
        /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para />
        /// This method is only invoked if no chargers returned any energy from <see cref="GenerateNewEnergy(float)" />.<para />
        /// Use this for method energy from batteries, reactor rods, biomass, or anything that can otherwise run out.<para />
        /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para />
        /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para />
        /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.<para />
        /// </summary>
        /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
        /// <returns>
        /// The amount of power produced by this cyclops charger.
        /// </returns>
        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (!ambientEnergyAvailable && this.AmbientEnergyUpgrade.TotalBatteryCharge > MinimalPowerValue)
            {
                return this.AmbientEnergyUpgrade.GetBatteryPower(BatteryDrainRate, requestedPower);
            }

            return 0f;
        }
    }
}
