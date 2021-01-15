namespace MoreCyclopsUpgrades.API.StatusIcons
{
    /// <summary>
    /// An interface for a class that implements a <see cref="CyclopsStatusIconCreator"/> method.
    /// </summary>
    public interface ICyclopsStatusIconCreator
    {
        /// <summary>
        /// Creates a new <see cref="CyclopsStatusIcon"/> with its associated <see cref="SubRoot"/> instance on demand.
        /// </summary>
        /// <param name="cyclops">The Cyclops sub instance.</param>
        /// <returns>A new new <see cref="CyclopsStatusIcon"/> instance.</returns>
        CyclopsStatusIcon CreateCyclopsStatusIcon(SubRoot cyclops);
    }

    /// <summary>
    /// Defines a method that accepts a <see cref="SubRoot"/> instance and returns a new <see cref="CyclopsStatusIcon"/>.
    /// </summary>
    /// <param name="cyclops">The Cyclops sub instance.</param>
    /// <returns>A new new <see cref="CyclopsStatusIcon"/> instance.</returns>
    public delegate CyclopsStatusIcon CyclopsStatusIconCreator(SubRoot cyclops);
}
