namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using UnityEngine;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#endif

    /// <summary>
    /// An interface that defines all the necessary elements of a CustomBatteries plugin pack.
    /// </summary>
    public abstract class CbItem
    {
        /// <summary>
        /// The full capacity of energy of the item.
        /// </summary>
        public abstract int Capacity { get; }

        /// <summary>
        /// The internal ID for the custom item.
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// The display name of the custom item shown in-game.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The flavor text for the custom item shown in-game when viewing it from a PDA screen.
        /// </summary>
        public abstract string FlavorText { get; }

        /// <summary>
        /// The materials required to craft the item.<para/>
        /// If you want multiple copies of the same material, include multiple entries of that <see cref="TechType"/>.<para/>
        /// If this list is empty, a default recipe of a single <see cref="TechType.Titanium"/> will be applied instead.
        /// </summary>
        public abstract IList<TechType> CraftingMaterials { get; }

        /// <summary>
        /// What item must be obtained, scanned, or built to unlock the battery and power cell.<para/>
        /// This property is optional. By default, the item will be unlocked at the start of the game.
        /// </summary>
        public virtual TechType UnlocksWith => TechType.None;

        /// <summary>
        /// The custom sprite for the item's icon.<br/>
        /// This property is optional.
        /// </summary>
        public virtual Sprite CustomIcon => null;

        /// <summary>
        /// The custom skin for the item.<br/>
        /// This property is optional.
        /// </summary>
        public virtual Texture2D CustomSkin => null;


        public virtual bool ExcludeFromChargers => false;
    }
}
