namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;

    public interface IGroupHandler
    {
        /// <summary>
        /// Gets a readonly list of the <see cref="TechType"/>s managed by this group handler.
        /// </summary>
        /// <value>
        /// The upgrade tiers managed by this group handler.
        /// </value>
        IEnumerable<TechType> ManagedTiers { get; }

        /// <summary>
        /// Determines whether the specified tech type is managed by this group handler.
        /// </summary>
        /// <param name="techType">The TechTech to check.</param>
        /// <returns>
        ///   <c>true</c> if this group handler manages the specified TechTech; otherwise, <c>false</c>.
        /// </returns>
        bool IsManaging(TechType techType);
    }
}