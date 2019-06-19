namespace MoreCyclopsUpgrades.API.Upgrades
{
    public interface IUpgradeHandlerCreator
    {
        /// <summary>
        /// Creates a new <see cref="UpgradeHandler"/> when the Cyclops is initialized.
        /// </summary>
        /// <returns>A newly instantiated <see cref="UpgradeHandler"/> ready to handle upgrade events.</returns>
        UpgradeHandler CreateUpgradeHandler(SubRoot cyclops);
    }

    /// <summary>
    /// Defines a method that creates a new <see cref="UpgradeHandler"/> when needed.
    /// </summary>
    /// <returns>A newly instantiated <see cref="UpgradeHandler"/> ready to handle upgrade events.</returns>
    public delegate UpgradeHandler CreateUpgradeHandler(SubRoot cyclops);
}
