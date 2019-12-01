namespace CustomBatteries.Items
{
    using CustomBatteries.PackReading;

    internal class CustomPack
    {
        private readonly CustomBattery _customBattery;
        private readonly CustomPowerCell _customPowerCell;

        internal string PluginPackName { get; }

        public CustomPack(IPluginDetails pluginPack)
        {
            this.PluginPackName = pluginPack.PluginPackName;

            _customBattery = new CustomBattery(pluginPack.BatteryID)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.BatteryName,
                Description = pluginPack.BatterFlavorText,
                IconFileName = pluginPack.BatteryIconFile,
                PowerCapacity = pluginPack.BatteryCapacity,
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.BatteryParts,
                PluginFolder = pluginPack.PluginPackFolder
            };

            _customPowerCell = new CustomPowerCell(pluginPack.PowerCellID, _customBattery)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.PowerCellName,
                Description = pluginPack.PowerCellFlavorText,
                IconFileName = pluginPack.PowerCellIconFile,
                PowerCapacity = pluginPack.BatteryCapacity * 2f,
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.PowerCellAdditionalParts,
                PluginFolder = pluginPack.PluginPackFolder
            };
        }

        public void Patch()
        {
            // Batteries must always patch before Power Cells
            _customBattery.Patch();
            _customPowerCell.Patch();
        }
    }
}
