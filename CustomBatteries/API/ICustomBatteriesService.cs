namespace CustomBatteries.API
{
    using System;
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
        [Obsolete("This is now an old API. Use the CbItem class instead.", false)]
        CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack);

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <param name="useIonCellSkins">If these batteries should use the ion cell textures.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        [Obsolete("This is now an old API. Use the CbItem class instead.", false)]
        CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack, bool useIonCellSkins);

        /// <summary>
        /// Returns the <see cref="EquipmentType"/> associated to the provided <see cref="TechType"/>.<br/>
        /// This is intended to identify if a given <see cref="TechType"/> is a Battery, Power Cell, or something else.
        /// </summary>
        /// <param name="techType">The item techtype to check</param>
        /// <returns>
        /// <see cref="EquipmentType.BatteryCharger"/> if the TechType is a Battery,
        /// <see cref="EquipmentType.PowerCellCharger"/> if the TechType is a Power Cell,
        /// or the resulting value from the game's GetEquipmentType method.
        /// </returns>
        EquipmentType GetEquipmentType(TechType techType);
    }
}
