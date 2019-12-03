namespace CustomBatteries.API
{
    internal class ModPluginPack : CustomPack
    {
        internal ModPluginPack(IModPluginPack pluginPack)
            : base(pluginPack)
        {
            _customBattery.Sprite = pluginPack.BatteryIcon;
            _customPowerCell.Sprite = pluginPack.PowerCellIcon;
        }
    }
}
