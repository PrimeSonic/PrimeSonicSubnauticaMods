namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;

    /// <summary>
    /// An API service class that handles requests for CustomBatteries from external mods.
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
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        public CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack)
        {
            QuickLogger.Info($"Received PluginPack '{modPluginPack.PluginPackName}' from '{Assembly.GetCallingAssembly().GetName().Name}'");

            var pack = new ModPluginPack(modPluginPack);
            pack.Patch();

            return pack;
        }
    }
}
