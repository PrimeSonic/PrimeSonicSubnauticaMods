namespace MoreCyclopsUpgrades.API.General
{
    /// <summary>
    /// Defines the interface needed by MoreCyclopsUpgrades to hook your own managers into the extisting Cyclops Manager system.
    /// </summary>
    public interface IAuxCyclopsManager
    {
        /// <summary>
        /// Initializes the auxiliary manager with the specified cyclops.<para/>
        /// This method is invoked only after all <see cref="IAuxCyclopsManager"/> instances have been created.<para/>
        /// Use this if you need to run any additional code after the constructor.
        /// </summary>
        /// <param name="cyclops">The cyclops this manager will handle.</param>
        /// <returns><c>True</c> if the initialization process succeeded; Otherwise returns <c>False</c>.</returns>
        bool Initialize(SubRoot cyclops);
    }
}
