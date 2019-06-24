namespace MoreCyclopsUpgrades.API.PDA
{
    public interface IOverlayCreator
    {
        /// <summary>
        /// Creates a new child of <see cref="IconOverlay" /> when the Cyclops is initialized.
        /// </summary>
        /// <param name="icon">The uGUI icon where all new elements are anchored to.</param>
        /// <param name="upgradeModule">The reference to the upgrade module gameobject.</param>
        /// <returns>
        /// A newly instantiated class that inherits from <see cref="IconOverlay" />.
        /// </returns>
        IconOverlay CreateOverlayText(uGUI_ItemIcon icon, InventoryItem upgradeModule);
    }

    /// <summary>
    /// Defines a method that creates a new <see cref="IconOverlay" /> when needed.
    /// </summary>
    /// <param name="icon">The uGUI icon where all new elements are anchored to.</param>
    /// <param name="upgradeModule">The reference to the upgrade module gameobject.</param>
    /// <returns>
    /// A newly instantiated class that inherits from <see cref="IconOverlay" />.
    /// </returns>
    public delegate IconOverlay CreateOverlayText(uGUI_ItemIcon icon, InventoryItem upgradeModule);
}
