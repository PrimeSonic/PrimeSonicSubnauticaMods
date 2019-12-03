namespace CustomBatteries.API
{
    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries mod plugin pack.
    /// </summary>
    /// <seealso cref="IPluginPack" />
    public interface IModPluginPack : IPluginPack
    {
        /// <summary>
        /// Gets the battery icon sprite.
        /// </summary>
        /// <value>
        /// The battery icon.
        /// </value>
        Atlas.Sprite BatteryIcon { get; }

        /// <summary>
        /// Gets the power cell icon sprite.
        /// </summary>
        /// <value>
        /// The power cell icon.
        /// </value>
        Atlas.Sprite PowerCellIcon { get; }
    }
}
