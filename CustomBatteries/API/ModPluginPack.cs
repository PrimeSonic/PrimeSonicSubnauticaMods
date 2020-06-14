namespace CustomBatteries.API
{
    internal class ModPluginPack : CustomPack
    {
        private ModPluginPack(IModPluginPack pluginPack, bool ionCellSkin)
            : base(pluginPack, ionCellSkin)
        {
            _customBattery.Sprite = pluginPack.BatteryIcon;
            _customPowerCell.Sprite = pluginPack.PowerCellIcon;
        }

        internal ModPluginPack(IModPluginPack pluginPack)
            : this(pluginPack, false)
        {
        }

        internal ModPluginPack(IModPluginPackV2 pluginPack)
            : this(pluginPack, pluginPack.UseIonCellSkins)
        {
        }
    }
}
