namespace CustomBatteries.API
{
    using Common;
    using CustomBatteries.Items;
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
        public IPluginPack OriginalPlugInPack { get; }

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

        private CustomPack(IPluginPack pluginPack)
        {
            this.OriginalPlugInPack = pluginPack;
            _customBattery = new CustomBattery(pluginPack.BatteryID)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.BatteryName,
                Description = pluginPack.BatterFlavorText,
                
                PowerCapacity = pluginPack.BatteryCapacity,
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.BatteryParts
            };

            _customPowerCell = new CustomPowerCell(pluginPack.PowerCellID, _customBattery)
            {
                PluginPackName = pluginPack.PluginPackName,
                FriendlyName = pluginPack.PowerCellName,
                Description = pluginPack.PowerCellFlavorText,
                
                PowerCapacity = pluginPack.BatteryCapacity * 2f, // Power Cell capacity is always 2x the battery capacity
                RequiredForUnlock = pluginPack.UnlocksWith,
                Parts = pluginPack.PowerCellAdditionalParts
            };
        }

        internal CustomPack(IModPluginPack pluginPack)
            : this((IPluginPack)pluginPack)
        {
            _customBattery.Sprite = pluginPack.BatteryIcon;
            _customPowerCell.Sprite = pluginPack.PowerCellIcon;
        }

        internal CustomPack(IParsedPluginPack pluginPack)
            : this((IPluginPack)pluginPack)
        {
            _customBattery.PluginFolder = pluginPack.PluginPackFolder;
            _customBattery.IconFileName = pluginPack.BatteryIconFile;

            _customPowerCell.PluginFolder = pluginPack.PluginPackFolder;
            _customPowerCell.IconFileName = pluginPack.PowerCellIconFile;
        }

        internal void Patch()
        {
            QuickLogger.Info($"Patching plugin pack '{this.OriginalPlugInPack.PluginPackName}'");
            // Batteries must always patch before Power Cells
            _customBattery.Patch();
            _customPowerCell.Patch();
        }
    }
}
