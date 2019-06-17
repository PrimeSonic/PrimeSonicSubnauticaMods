namespace MoreCyclopsUpgrades.API
{
    using UnityEngine;

    /// <summary>
    /// <para>Defines a method that creates a new <see cref="ICyclopsCharger"/> when needed by the <seealso cref="PowerManager"/>.</para>
    /// <para>DO NOT recharge the Cyclops PowerRelay yourself from the instantiated <see cref="ICyclopsCharger"/>!!! The MoreCyclopsUpgrades PowerManager will handle that.</para>
    /// </summary>
    /// <param name="cyclops">The cyclops that the <see cref="ICyclopsCharger"/> is tasked with recharging.</param>
    /// <returns>A new <see cref="ICyclopsCharger"/> ready to produce power for the Cyclops.</returns>
    public delegate ICyclopsCharger ChargerCreator(SubRoot cyclops);

    /// <summary>
    /// <para>Defines all the behaviors for a cyclops charger that handles a particular type of energy recharging.</para>
    /// <para>Recharging may be part of an update module or it might be a new buidable.</para>
    /// <para>Whatever the case, it is up to you to ensure you have all your references set and ready.</para>
    /// <para>DO NOT recharge the Cyclops PowerRelay yourself from this class!!! The MoreCyclopsUpgrades PowerManager will handle that.</para>
    /// </summary>
    public interface ICyclopsCharger
    {
        /// <summary>
        /// <para>Produces power for the Cyclops during the RechargeCyclops update cycle.</para>
        /// <para>This method should return <c>0f</c> if there is no power avaiable from this charging handler.</para>
        /// <para>You may limit the amount of power produced to only what the cyclops needs or you may return more.</para>
        /// <para>DO NOT recharge the Cyclops PowerRelay yourself from this method!!! The MoreCyclopsUpgrades PowerManager will handle that.</para>
        /// </summary>
        /// <param name="requestedPower">The amount of power being requested by the cyclops; This is the current Power Deficit of the cyclops.</param>
        /// <returns>The amount of power produced by this cyclops charger.</returns>
        float ProducePower(float requestedPower);

        /// <summary>
        /// Gets a value indicating if this type of cyclops energy source is renewable.</param>
        /// Use <c>true</c> for rechargable batteries and energy drawn from the environment.</param>
        /// Use <c>false</c> for depletable sources like nuclear reactor rods.
        /// </summary>
        bool IsRenewable { get; }

        /// <summary>
        /// Gets the name that identifies this <see cref="ICyclopsCharger"/> among all others in the Cyclops sub.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// <para>Determines whether this charger should display any power indicator info.</para>
        /// <para>This method is called roughly every 3 seconds when the Cyclops HUD info is updated.</para>
        /// </summary>
        /// <returns>
        ///   <c>true</c> if a power indicator should be shown; otherwise, <c>false</c>.
        /// </returns>
        bool HasPowerIndicatorInfo();

        /// <summary>
        /// Gets the sprite to use for the power indicator. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A new <see cref="Atlas.Sprite"/> to be used in the Cyclops Helm and Holographic HUDs.</returns>
        Atlas.Sprite GetIndicatorSprite();

        /// <summary>
        /// Gets the text to use under the power indicator icon. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="string"/>, ready to use for in-game text. Should be limited to numeric values if possible.</returns>
        string GetIndicatorText();

        /// <summary>
        /// Gets the color of the text used under the power indicator icon. This will only be called when <see cref="HasPowerIndicatorInfo"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A Unity <see cref="Color"/> value for the text. When in doubt, just set this to white.</returns>
        Color GetIndicatorTextColor();

        /// <summary>
        /// If the charger has its own store of energy, return the total available reserve power.
        /// </summary>
        /// <returns>The total power the charger is capable of providing over time; Return 0f if there are no power reserves.</returns>
        float TotalReservePower();
    }
}