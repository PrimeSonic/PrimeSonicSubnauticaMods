namespace CustomBatteries.API
{
    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries mod plugin pack.
    /// </summary>
    /// <seealso cref="IPluginPack" />
    public interface IModPluginPack : IPluginPack
    {
        Atlas.Sprite BatteryIcon { get; }

        Atlas.Sprite PowerCellIcon { get; }
    }
}
