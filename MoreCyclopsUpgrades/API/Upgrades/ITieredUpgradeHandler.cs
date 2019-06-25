namespace MoreCyclopsUpgrades.API.Upgrades
{
    /// <summary>
    /// Defines an interface for an upgrade handler that is part of a grouped collection.<para/>
    /// You will see this on <see cref="TieredUpgradeHandler{T}"/> and <see cref="StackingUpgradeHandler"/>.
    /// </summary>
    public interface IGroupedUpgradeHandler
    {
        /// <summary>
        /// The parent <see cref="UpgradeHandler"/> of this tier.
        /// </summary>
        IGroupHandler GroupHandler { get; }
    }
}
