namespace MoreCyclopsUpgrades.API.General
{
    /// <summary>
    /// Defines an interface for a class capable of creating a new <see cref="IAuxCyclopsManager"/> on demand.
    /// </summary>
    public interface IAuxCyclopsManagerCreator
    {
        /// <summary>
        /// Creates a new <see cref="IAuxCyclopsManager" /> instance when a new Cyclops sub is initialized.
        /// </summary>
        /// <param name="cyclops">The cyclops that the <see cref="IAuxCyclopsManager" /> is tasked with keeping track of.</param>
        /// <returns>A newly created <see cref="IAuxCyclopsManager"/> ready to be initialized by <seealso cref="IAuxCyclopsManager.Initialize(SubRoot)"/>.</returns>
        IAuxCyclopsManager CreateAuxCyclopsManager(SubRoot cyclops);
    }

    /// <summary>
    /// Defines a method that creates a new <see cref="IAuxCyclopsManager" /> instance whenever a new Cyclops sub is spawned.
    /// </summary>
    /// <param name="cyclops">The cyclops that the <see cref="IAuxCyclopsManager" /> is tasked with keeping track of.</param>
    /// <returns>A newly created <see cref="IAuxCyclopsManager"/> ready to be initialized by <seealso cref="IAuxCyclopsManager.Initialize(SubRoot)"/>.</returns>
    public delegate IAuxCyclopsManager CreateAuxCyclopsManager(SubRoot cyclops);
}
