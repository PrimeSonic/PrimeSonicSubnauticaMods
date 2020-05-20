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

        bool ContainsValue(UpgradeHandler value);

        UpgradeHandler this[TechType key] { get; set; }

        bool TryGetValue(TechType key, out UpgradeHandler value);
    }
}
