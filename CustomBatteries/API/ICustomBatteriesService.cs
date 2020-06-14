namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;

    /// <summary>
    /// A simple interface that defines the services that CustomBatteries can provide to external mods.
    /// </summary>
    public interface ICustomBatteriesService
    {
        /// <summary>
        /// Gets the full collection of <see cref="TechType"/>s for all batteries, both vanilla and modded.
        /// </summary>
        /// <returns>
        /// The full collection of battery <see cref="TechType"/>s.
        /// </returns>
        /// <seealso cref="BatteryCharger"/>
        HashSet<TechType> GetAllBatteries();

        /// <summary>
        /// Gets the full collection of <see cref="TechType"/>s for all power cells, both vanilla and modded.
        /// </summary>
        /// <returns>
        /// The full collection of power cell <see cref="TechType"/>s.
        /// </returns>
        /// <seealso cref="PowerCellCharger"/>
        HashSet<TechType> GetAllPowerCells();

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack);

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        CustomPack AddPluginPackFromModV2(IModPluginPackV2 modPluginPack);
    }
}
