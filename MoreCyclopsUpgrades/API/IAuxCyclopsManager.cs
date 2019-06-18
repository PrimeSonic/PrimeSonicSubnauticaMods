namespace MoreCyclopsUpgrades.API
{

    /// <summary>
    /// Defines a method that creates a new <see cref="IAuxCyclopsManager" /> instance whenever a new Cyclops sub is spawned.
    /// </summary>
    /// <param name="cyclops">The cyclops that the <see cref="IAuxCyclopsManager" /> is tasked with keeping track of.</param>
    /// <returns>A newly created <see cref="IAuxCyclopsManager"/> ready to be initialized by <seealso cref="IAuxCyclopsManager.Initialize(SubRoot)"/>.</returns>
    public delegate IAuxCyclopsManager AuxManagerCreator(SubRoot cyclops);

    /// <summary>
    /// Defines the itnerface needed by MoreCyclopsUpgrades to hook your own managers into the extisting Cyclops Manager system.
    /// </summary>
    public interface IAuxCyclopsManager
    {
        /// <summary>
        /// Gets the name of the Auxiliary Cyclops Manager.<para/>
        /// This acts an ID for the manager so it can be located when calling into <seealso cref="IMCUServices.FindManager{T}(SubRoot, string)"/>.
        /// </summary>
        /// <value>
        /// The name of this type of manager.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Initializes the auxiliary manager with the specified cyclops.
        /// </summary>
        /// <param name="cyclops">The cyclops this manager will handle.</param>
        /// <returns><c>True</c> if the initialization process succeeded; Otherwise returns <c>False</c>.</returns>
        bool Initialize(SubRoot cyclops);
    }
}
