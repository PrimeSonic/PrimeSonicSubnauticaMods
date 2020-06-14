namespace CustomBatteries.API
{
    public interface IModPluginPackV2 : IModPluginPack
    {
        /// <summary>
        /// Gets a value indicating whether the Ion battery and power cell skins should be used for this pack.
        /// </summary>
        /// <value><c>True</c> if the Ion cell skins should be used; Otherwise <c>false</c>.</value>
        bool UseIonCellSkins { get; }
    }
}
