namespace MoreCyclopsUpgrades.API.Buildables
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for an upgrade console that can report on its upgrade slots.
    /// </summary>
    public interface IUpgradeSlots
    {
        /// <summary>
        /// Gets the upgrade slots for this upgrade console.
        /// </summary>
        /// <value>
        /// The upgrade slots.
        /// </value>
        IEnumerable<UpgradeSlot> UpgradeSlots { get; }
    }
}
