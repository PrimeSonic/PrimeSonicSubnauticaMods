namespace MoreCyclopsUpgrades.API.Charging
{    
    using UnityEngine;

    internal interface ICyclopsCharger
    {
        bool ShowStatusIcon { get; }
        float TotalReserveEnergy { get; }
        float Generate(float requestedPower);
        float Drain(float requestedPower);
        Atlas.Sprite StatusSprite();
        string StatusText();
        Color StatusTextColor();
    }

    /// <summary>
    /// Defines all the behaviors for a cyclops charger that handles a particular type of energy recharging.<para/>
    /// Recharging may be part of an update module or it might be a new buidable.<para/>
    /// Whatever the case, it is up to you to ensure you have all your references set and ready.<para/>
    /// DO NOT recharge the Cyclops PowerRelay yourself from this class!!! The MoreCyclopsUpgrades PowerManager will handle that.<para/>
    /// </summary>
    public abstract class CyclopsCharger : ICyclopsCharger
    {
        public readonly SubRoot Cyclops;

        protected CyclopsCharger(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        private bool showStatus;
        bool ICyclopsCharger.ShowStatusIcon => showStatus;

        float ICyclopsCharger.Generate(float requestedPower)
        {
            float energy = GenerateNewEnergy(requestedPower);
            showStatus = energy > 0f;
            return energy;
        }

        float ICyclopsCharger.Drain(float requestedPower)
        {
            float energy = DrainReserveEnergy(requestedPower);
            showStatus |= energy > 0f;
            return energy;
        }

        /// <summary>
        /// If the charger has its own store of energy, return the total available reserve power.
        /// </summary>
        /// <returns>The total power the charger is capable of providing over time; Return <c>0f</c> if there are no power reserves.</returns>
        public abstract float TotalReserveEnergy { get; }

        /// <summary>
        /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para />
        /// Use this for method rechargable energy drawn from the environment is isn't limited by a material resource.<para />
        /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para/>
        /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para/>
        /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.<para/>
        /// </summary>
        /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
        /// <returns>The amount of power produced by this cyclops charger.</returns>
        protected abstract float GenerateNewEnergy(float requestedPower);

        /// <summary>
        /// Produces power for the Cyclops during the RechargeCyclops update cycle.<para/>
        /// Use this for method energy from batteries, reactor rods, biomass, or anything that can otherwise run out.<para />
        /// This method should return <c>0f</c> if there is no power avaiable from this charging handler.<para/>
        /// You may limit the amount of power produced to only what the cyclops needs or you may return more.<para/>
        /// DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.<para/>
        /// </summary>
        /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
        /// <returns>The amount of power produced by this cyclops charger.</returns>
        protected abstract float DrainReserveEnergy(float requestedPower);

        /// <summary>
        /// Gets the sprite to use for the power indicator. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A new <see cref="Atlas.Sprite"/> to be used in the Cyclops Helm and Holographic HUDs.</returns>
        public abstract Atlas.Sprite StatusSprite();

        /// <summary>
        /// Gets the text to use under the power indicator icon. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="string"/>, ready to use for in-game text. Should be limited to numeric values if possible.</returns>
        public abstract string StatusText();

        /// <summary>
        /// Gets the color of the text used under the power indicator icon. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A Unity <see cref="Color"/> value for the text. When in doubt, just set this to white.</returns>
        public abstract Color StatusTextColor();
    }
}