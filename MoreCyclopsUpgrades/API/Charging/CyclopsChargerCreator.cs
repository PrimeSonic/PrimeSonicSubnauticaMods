namespace MoreCyclopsUpgrades.API.Charging
{
    public interface ICyclopsChargerCreator
    {
        /// <summary>
        /// Creates a new <see cref="ICyclopsCharger"/> when the Cyclops is initialized.
        /// </summary>
        /// <param name="cyclops">The cyclops that the <see cref="ICyclopsCharger"/> is tasked with recharging.</param>
        /// <returns>A new <see cref="ICyclopsCharger"/> ready to produce power for the Cyclops.</returns>
        ICyclopsCharger CreateCyclopsCharger(SubRoot cyclops);
    }

    /// <summary>
    /// Defines a method that creates a new <see cref="ICyclopsCharger"/> when needed.<para/>
    /// DO NOT recharge the Cyclops PowerRelay yourself from the instantiated <see cref="ICyclopsCharger"/>!!! MoreCyclopsUpgrades will handle that.<para/>
    /// </summary>
    /// <param name="cyclops">The cyclops that the <see cref="ICyclopsCharger"/> is tasked with recharging.</param>
    /// <returns>A new <see cref="ICyclopsCharger"/> ready to produce power for the Cyclops.</returns>
    public delegate ICyclopsCharger CreateCyclopsCharger(SubRoot cyclops);
}
