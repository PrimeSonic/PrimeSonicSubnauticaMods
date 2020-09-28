namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using UnityEngine;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#endif

    internal enum ItemTypes
    {
        Battery,
        PowerCell
    }

    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries plugin pack.
    /// </summary>
    public interface IPackItem
    {
        /// <summary>
        /// The full capacity of energy of the item.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// The internal ID for the custom item.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The display name of the custom item shown in-game.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The flavor text for the custom item shown in-game when viewing it from a PDA screen.
        /// </summary>
        string FlavorText { get; }

        /// <summary>
        /// The materials required to craft the item.<para/>
        /// If you want multiple copies of the same material, include multiple entries of that <see cref="TechType"/>.<para/>
        /// If this list is empty, a default recipe of a single <see cref="TechType.Titanium"/> will be applied instead.
        /// </summary>
        IList<TechType> CraftingMaterials { get; }

        /// <summary>
        /// What item must be obtained, scanned, or built to unlock the battery and power cell.<para/>
        /// If you want them unlocked at the start of the game, set this to <see cref="TechType.None"/>.
        /// </summary>
        TechType UnlocksWith { get; }

        /// <summary>
        /// The custom sprite for the item's icon.<br/>
        /// Return <see langword="null"/> to use a default icon.
        /// </summary>
        Sprite CustomIcon { get; }

        /// <summary>
        /// The custom skin for the item.<br/>
        /// Return <see langword="null"/> to use a default skin.
        /// </summary>
        Texture2D CustomSkin { get; }
    }
}
