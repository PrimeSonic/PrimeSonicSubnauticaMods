namespace MoreCyclopsUpgrades.API.General
{
    /// <summary>
    /// Defines the interface needed by MoreCyclopsUpgrades to hook your own managers into the extisting Cyclops Manager system.
    /// </summary>
    public interface IAuxCyclopsManager
    {
        /// <summary>
        /// Gets the name of the Auxiliary Cyclops Manager.<para/>
        /// This acts an ID for the manager so it can be located when calling into <seealso cref="IMCUSearch.AuxCyclopsManager{T}(SubRoot, string)"/>.
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
