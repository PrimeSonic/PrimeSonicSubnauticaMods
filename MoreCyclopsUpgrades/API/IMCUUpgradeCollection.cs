namespace MoreCyclopsUpgrades.API
{
    using System.Collections;
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.Upgrades;

    /// <summary>
    /// A read-only dictionary collection of Cyclops <see cref="UpgradeHandler"/>s.
    /// </summary>
    public interface IMCUUpgradeCollection : IEnumerable<KeyValuePair<TechType, UpgradeHandler>>, IEnumerable
    {
        /// <summary>
        /// Get the total count of entries in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value indicating if 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(TechType key);

        /// <summary>
        /// Checks if a specifics upgrade handler exists in the collection.
        /// </summary>
        /// <param name="value">The upgrade handler being searched for.</param>
        /// <returns><c>True</c> if t he UpgradeHandler is part of the collection; Otherwise <c>false</c>.</returns>
        bool ContainsValue(UpgradeHandler value);

        /// <summary>
        /// Returns the <see cref="UpgradeHandler"/> for the specified upgrade module corresponding to the provided <see cref="TechType"/>.
        /// </summary>
        /// <param name="key">The TechType of the upgrade module.</param>
        /// <returns>The <see cref="UpgradeHandler"/> managing the upgrade module.</returns>
        /// <exception cref="KeyNotFoundException"/>
        UpgradeHandler this[TechType key] { get; }

        /// <summary>
        /// Performs a safe lookup of the <see cref="UpgradeHandler"/> that corresponds with the provided <see cref="TechType"/>.
        /// </summary>
        /// <param name="key">The TechType of the upgrade module.</param>
        /// <param name="value">The upgrade handler that manages this upgrade.</param>
        /// <returns><c>True</c> if the upgrade module is in the collection; Otherwise <c>false</c>.</returns>
        bool TryGetValue(TechType key, out UpgradeHandler value);
    }
}
