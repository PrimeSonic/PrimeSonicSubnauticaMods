namespace CustomBatteries.API
{
    using CustomBatteries.PackReading;

    internal class TextPluginPack : CustomPack
    {
        internal TextPluginPack(IParsedPluginPack pluginPack)
            : base(pluginPack, pluginPack.UseIonCellSkins, false)
        {
            _customBattery.PluginFolder = pluginPack.PluginPackFolder;
            _customBattery.IconFileName = pluginPack.BatteryIconFile;

            _customPowerCell.PluginFolder = pluginPack.PluginPackFolder;
            _customPowerCell.IconFileName = pluginPack.PowerCellIconFile;
        }
    }
}
