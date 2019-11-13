namespace CustomBatteries.Items
{
    using Common;
    using CustomBatteries.PackReading;

    internal class CustomPack
    {
        private readonly CustomBattery _customBattery;

        private readonly CustomPowerCell _customPowerCell;

        private readonly IPluginPack _pluginPack;

        public CustomPack(IPluginPack pluginPack)
        {
            _pluginPack = pluginPack;

            _customBattery = new CustomBattery(pluginPack.BatteryID)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.BatteryName,
                Description = pluginPack.BatterFlavorText,
                IconFileName = pluginPack.BatteryIconFile,
                PowerCapacity = pluginPack.BatteryCapacity
            };
            _customBattery.CreateBlueprintData(pluginPack.BatteryParts);

            _customPowerCell = new CustomPowerCell(pluginPack.PowerCellID, _customBattery)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.PowerCellName,
                Description = pluginPack.PowerCellFlavorText,
                IconFileName = pluginPack.PowerCellIconFile,
                PowerCapacity = pluginPack.BatteryCapacity * 2f
            };
        }

        public void Patch()
        {
            QuickLogger.Info($"Patching plugin pack '{_pluginPack.PluginPackName}'");

            _customBattery.Patch();

            _customPowerCell.CreateBlueprintData(_pluginPack.PowerCellAdditionalParts);

            _customPowerCell.Patch();
        }
    }
}
