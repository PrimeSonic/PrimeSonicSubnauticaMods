namespace MoreCyclopsUpgrades.API.Buildables
{
    /// <summary>
    /// A simple interface used to provide status details to the <see cref="BuildableManager{BuildableMono}"/>.
    /// </summary>
    public interface ICyclopsBuildable
    {
        /// <summary>
        /// Gets a value indicating whether this buildable is connected to the Cyclops.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this buildable is connected to cyclops; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="BuildableManager{BuildableMono}.ConnectWithManager(BuildableMono)"/>
        bool IsConnectedToCyclops { get; }
    }
}
