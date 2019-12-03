namespace CustomBatteries.Items
{
    using CustomBatteries.PackReading;
    using SMLHelper.V2.Assets;

    /// <summary>
    /// A container class that a holds the modded battery and power cell prefab objects.
    /// </summary>
    public class CustomPack
    {
        private readonly CustomBattery _customBattery;
        private readonly CustomPowerCell _customPowerCell;

        /// <summary>
        /// Gets the name of the plugin pack.
        /// </summary>
        /// <value>
        /// The name of the plugin pack.
        /// </value>
        public string PluginPackName { get; }

        /// <summary>
        /// Gets the custom battery.
        /// </summary>
        /// <value>
        /// The custom battery.
        /// </value>
        public ModPrefab CustomBattery => _customBattery;

        /// <summary>
        /// Gets the custom power cell.
        /// </summary>
        /// <value>
        /// The custom power cell.
        /// </value>
        public ModPrefab CustomPowerCell => _customPowerCell;

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomBattery"/> and <see cref="CustomPowerCell"/> have been patched.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this pack is patched; otherwise, <c>false</c>.
        /// </value>
        public bool IsPatched => _customBattery.IsPatched && _customPowerCell.IsPatched;

        private CustomPack(IPluginPack pluginPack, string packName)
        {
            this.PluginPackName = packName;
            _customBattery = new CustomBattery(pluginPack.BatteryID)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.BatteryName,
                Description = pluginPack.BatterFlavorText,
                IconFileName = pluginPack.BatteryIconFile,
                PowerCapacity = pluginPack.BatteryCapacity,
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.BatteryParts
            };

            _customPowerCell = new CustomPowerCell(pluginPack.PowerCellID, _customBattery)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.PowerCellName,
                Description = pluginPack.PowerCellFlavorText,
                IconFileName = pluginPack.PowerCellIconFile,
                PowerCapacity = pluginPack.BatteryCapacity * 2f, // Power Cell capacity is always 2x the battery capacity
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.PowerCellAdditionalParts
            };
        }

        internal CustomPack(IPluginPack pluginPack, Atlas.Sprite batterySprite, Atlas.Sprite powerCellSprite)
            : this(pluginPack, pluginPack.PluginPackName)
        {
            _customBattery.Sprite = batterySprite;
            _customPowerCell.Sprite = powerCellSprite;
        }

        internal CustomPack(IPluginDetails pluginPack)
            : this(pluginPack, pluginPack.PluginPackName)
        {
            _customBattery.PluginFolder = pluginPack.PluginPackFolder;
            _customPowerCell.PluginFolder = pluginPack.PluginPackFolder;
        }

        internal void Patch()
        {
            // Batteries must always patch before Power Cells
            _customBattery.Patch();
            _customPowerCell.Patch();
        }
    }
}
