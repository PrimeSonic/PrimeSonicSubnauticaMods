namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;

    /// <summary>
    /// A simple service to allow other mods to use CustomBatteries as a service to quickly add in their own modded batteries.
    /// </summary>
    public interface ICustomBatteriesService
    {
        /// <summary>
        /// Gets the full collection of <see cref="TechType"/>s for all batteries, both vanilla and modded.
        /// </summary>
        /// <value>
        /// The full collection of battery <see cref="TechType"/>s.
        /// </value>
        /// <seealso cref="BatteryCharger"/>
        HashSet<TechType> AllBatteries { get; }

        /// <summary>
        /// Gets the full collection of <see cref="TechType"/>s for all power cells, both vanilla and modded.
        /// </summary>
        /// <value>
        /// The full collection of power cell <see cref="TechType"/>s.
        /// </value>
        /// <seealso cref="PowerCellCharger"/>
        HashSet<TechType> AllPowerCells { get; }

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <param name="batterySprite">The battery sprite.</param>
        /// <param name="powerCellSprite">The power cell sprite.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        CustomPack AddPluginPackFromMod(IPluginPack modPluginPack, Atlas.Sprite batterySprite, Atlas.Sprite powerCellSprite);
    }
}
