namespace CustomBatteries.API
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CustomBatteries.Items;

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
        [Obsolete("This is now an old API. Use the CbItem class instead.", false)]
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
        [Obsolete("This is now an old API. Use the CbItem class instead.", false)]
        public CustomPack AddPluginPackFromMod(IModPluginPack modPluginPack, bool useIonCellSkins)
        {
            QuickLogger.Info($"Received PluginPack '{modPluginPack.PluginPackName}', from '{Assembly.GetCallingAssembly().GetName().Name}'");

            var pack = new ModPluginPack(modPluginPack, useIonCellSkins);
            pack.Patch();

            return pack;
        }

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
        public EquipmentType GetEquipmentType(TechType techType)
        {
            if (BatteryCharger.compatibleTech.Contains(techType))
            {
                return EquipmentType.BatteryCharger;
            }
            else if (PowerCellCharger.compatibleTech.Contains(techType))
            {
                return EquipmentType.PowerCellCharger;
            }
            else if (CbDatabase.TrackItems.Contains(techType))
            {
                if (CbDatabase.BatteryItems.FindIndex(cb => cb.TechType == techType) > -1)
                    return EquipmentType.BatteryCharger; // Batteries that do not go into chargers
                else if (CbDatabase.PowerCellItems.FindIndex(cb => cb.TechType == techType) > -1)
                    return EquipmentType.PowerCellCharger; // Power cells that do not go into chargers
            }

#if SUBNAUTICA
            return CraftData.GetEquipmentType(techType);
#elif BELOWZERO
            return TechData.GetEquipmentType(techType);
#endif
        }
    }
}
