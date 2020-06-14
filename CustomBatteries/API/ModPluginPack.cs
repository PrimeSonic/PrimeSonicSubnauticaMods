namespace CustomBatteries.API
{
    internal class ModPluginPack : CustomPack
    {
        internal ModPluginPack(IModPluginPack pluginPack, bool ionCellSkin)
            : base(pluginPack, ionCellSkin, false)
        {
            _customBattery.Sprite = pluginPack.BatteryIcon;
            _customPowerCell.Sprite = pluginPack.PowerCellIcon;
        }

        internal ModPluginPack(IModPluginPackCustomSkin pluginPack)
            : base(pluginPack, false, true)
        {
            _customBattery.Sprite = pluginPack.BatteryIcon;
            _customPowerCell.Sprite = pluginPack.PowerCellIcon;

            _customBattery.CustomSkin = pluginPack.BatterySkin;
            _customPowerCell.CustomSkin = pluginPack.PowerCellSkin;
        }
    }
}
