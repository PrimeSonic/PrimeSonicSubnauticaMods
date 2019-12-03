namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;

    /// <summary>
    /// An API class that handles services from CustomBatteries for external mods.<para/>
    /// </summary>
    /// <seealso cref="ICustomBatteriesService" />
    public class CustomBatteriesService : ICustomBatteriesService
    {
        /// <summary>
        /// Gets the full collection of <see cref="TechType" />s for all batteries, both vanilla and modded.
        /// </summary>
        /// <value>
        /// The full collection of battery <see cref="TechType" />s.
        /// </value>
        /// <seealso cref="BatteryCharger" />
        public HashSet<TechType> AllBatteries => BatteryCharger.compatibleTech;

        /// <summary>
        /// Gets the full collection of <see cref="TechType" />s for all power cells, both vanilla and modded.
        /// </summary>
        /// <value>
        /// The full collection of power cell <see cref="TechType" />s.
        /// </value>
        /// <seealso cref="PowerCellCharger" />
        public HashSet<TechType> AllPowerCells => PowerCellCharger.compatibleTech;

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <param name="batterySprite">The battery sprite.</param>
        /// <param name="powerCellSprite">The power cell sprite.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        public CustomPack AddPluginPackFromMod(IPluginPack modPluginPack, Atlas.Sprite batterySprite, Atlas.Sprite powerCellSprite)
        {
            QuickLogger.Info($"Received PluginPack '{modPluginPack.PluginPackName}' from '{Assembly.GetCallingAssembly().GetName().Name}'");

            var pack = new CustomPack(modPluginPack, batterySprite, powerCellSprite);
            pack.Patch();

            return pack;
        }
    }
}
