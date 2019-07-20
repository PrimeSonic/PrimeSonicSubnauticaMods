namespace MoreCyclopsUpgrades.API.Upgrades
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a common interface for upgade handler classes that manage multiple different upgrades as once.<para/>
    /// You will see this on <see cref="StackingGroupHandler"/> and <see cref="TieredGroupHandler{T}"/>.
    /// </summary>
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
