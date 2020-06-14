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
        /// <returns>
        /// The full collection of battery <see cref="TechType" />s.
        /// </returns>
        /// <seealso cref="BatteryCharger" />
        public HashSet<TechType> GetAllBatteries()
        {
            return new HashSet<TechType>(BatteryCharger.compatibleTech);
        }

        /// <summary>
        /// Gets the full collection of <see cref="TechType" />s for all power cells, both vanilla and modded.
        /// </summary>
        /// <returns>
        /// The full collection of power cell <see cref="TechType" />s.
        /// </returns>
        /// <seealso cref="PowerCellCharger" />
        public HashSet<TechType> GetAllPowerCells()
        {
            return new HashSet<TechType>(PowerCellCharger.compatibleTech);
        }

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

            var pack = new ModPluginPack(modPluginPack, false);
            pack.Patch();

            return pack;
        }

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded items returned.
        /// </summary>
        /// <param name="modPluginPack">The mod plugin pack.</param>
        /// <param name="useIonCellSkins">If these batteries should use the ion cell textures.</param>
        /// <returns>
        /// A <see cref="CustomPack" /> containing the patched <see cref="ModPrefab" /> intances for both the <see cref="CustomPack.CustomBattery" /> and <see cref="CustomPack.CustomPowerCell" />.
        /// </returns>
        public CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack, bool useIonCellSkins)
        {
            QuickLogger.Info($"Received PluginPack '{modPluginPack.PluginPackName}', from '{Assembly.GetCallingAssembly().GetName().Name}'");

            var pack = new ModPluginPack(modPluginPack, useIonCellSkins);
            pack.Patch();

            return pack;
        }

        public CustomPack AddSkinedPluginPackFromMod(IModPluginPackCustomSkin modPluginPack)
        {
            QuickLogger.Info($"Received PluginPack '{modPluginPack.PluginPackName}', from '{Assembly.GetCallingAssembly().GetName().Name}'");

            var pack = new ModPluginPack(modPluginPack);
            pack.Patch();

            return pack;
        }
    }
}
