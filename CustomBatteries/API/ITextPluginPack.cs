namespace CustomBatteries.API
{
    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries text plugin pack.
    /// </summary>
    /// <seealso cref="IPluginPack" />
    internal interface ITextPluginPack : IPluginPack
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

        /// <summary>
        /// Gets a value indicating whether the Ion battery and power cell skins should be used for this pack.
        /// </summary>
        /// <value><c>True</c> if the Ion cell skins should be used; Otherwise <c>false</c>.</value>
        bool UseIonCellSkins { get; }
    }
}
