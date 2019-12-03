namespace CustomBatteries.API
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries plugin pack.
    /// </summary>
    public interface IPluginPack
    {
        /// <summary>
        /// The name of the pack. Currently has no impact beyond logging.
        /// </summary>
        string PluginPackName { get; }

        /// <summary>
        /// The full capacity of energy of the battery.<para/>
        /// The standard, vanilla battery is 50 energy.
        /// </summary>
        int BatteryCapacity { get; }

        /// <summary>
        /// What item must be obtained, scanned, or built to unlock the battery and power cell.<para/>
        /// If you want them unlocked at the start of the game, set this to <see cref="TechType.None"/>.
        /// </summary>
        TechType UnlocksWith { get; }

        /// <summary>
        /// The internal ID for the custom battery.
        /// </summary>
        string BatteryID { get; }

        /// <summary>
        /// The display name of the custom battery shown in-game.
        /// </summary>
        string BatteryName { get; }

        /// <summary>
        /// The flavor text for the custom battery shown in-game when viewing it from a PDA screen.
        /// </summary>
        string BatterFlavorText { get; }

        /// <summary>
        /// The materials required to craft the battery.<para/>
        /// If you want multiple copies of the same material, include multiple entries of that <see cref="TechType"/>.
        /// </summary>
        IList<TechType> BatteryParts { get; }        

        /// <summary>
        /// The internal ID for the custom power cell.
        /// </summary>
        string PowerCellID { get; }

        /// <summary>
        /// The display name of the custom power cell shown in-game.
        /// </summary>
        string PowerCellName { get; }

        /// <summary>
        /// The flavor text for the custom power cell shown in-game when viewing it from a PDA screen.
        /// </summary>
        string PowerCellFlavorText { get; }

        IList<TechType> PowerCellAdditionalParts { get; }        
    }
}
