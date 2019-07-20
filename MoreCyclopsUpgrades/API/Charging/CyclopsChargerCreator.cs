namespace MoreCyclopsUpgrades.API.Charging
{
    /// <summary>
    /// Defines an interface for a class that is capable of creating a new <see cref="CyclopsCharger"/> on demand.
    /// </summary>
    public interface ICyclopsChargerCreator
    {
        /// <summary>
        /// Creates a new <see cref="CyclopsCharger"/> when the Cyclops is initialized.
        /// </summary>
        /// <param name="cyclops">The cyclops that the <see cref="CyclopsCharger"/> is tasked with recharging.</param>
        /// <returns>A new <see cref="CyclopsCharger"/> ready to produce power for the Cyclops.</returns>
        CyclopsCharger CreateCyclopsCharger(SubRoot cyclops);
    }

    /// <summary>
    /// Defines a method that creates a new <see cref="CyclopsCharger"/> when needed.<para/>
    /// DO NOT recharge the Cyclops PowerRelay yourself from the instantiated <see cref="CyclopsCharger"/>!!! MoreCyclopsUpgrades will handle that.<para/>
    /// </summary>
    /// <param name="cyclops">The cyclops that the <see cref="CyclopsCharger"/> is tasked with recharging.</param>
    /// <returns>A new <see cref="CyclopsCharger"/> ready to produce power for the Cyclops.</returns>
    public delegate CyclopsCharger CreateCyclopsCharger(SubRoot cyclops);
}
