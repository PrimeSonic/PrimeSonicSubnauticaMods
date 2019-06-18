namespace MoreCyclopsUpgrades.API
{
    public interface IGroupedUpgradeHandler
    {
        /// <summary>
        /// The parent <see cref="UpgradeHandler"/> of this tier.
        /// </summary>
        IGroupHandler GroupHandler { get; }
    }
}