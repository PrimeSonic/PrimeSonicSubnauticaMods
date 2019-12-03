namespace CustomBatteries.API
{
    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries text plugin pack.
    /// </summary>
    /// <seealso cref="IPluginPack" />
    public interface ITextPluginPack : IPluginPack
    {
        /// <summary>
        /// Gets the battery icon file name. This should be a PNG image file.
        /// </summary>
        /// <value>
        /// The battery icon file name.
        /// </value>
        string BatteryIconFile { get; }

        /// <summary>
        /// Gets the battery icon file name. This should be a PNG image file.
        /// </summary>
        /// <value>
        /// The battery icon file name.
        /// </value>
        string PowerCellIconFile { get; }
    }
}
