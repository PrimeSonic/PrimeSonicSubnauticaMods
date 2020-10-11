namespace CustomBatteries.API
{
    using System.Collections.Generic;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;
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
        public abstract int EnergyCapacity { get; }

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
        /// This property is optional and will default to the standard icon for batteries or power cells.
        /// </summary>
        public virtual Sprite CustomIcon => null;

        /// <summary>
        /// The custom skin for the item.<br/>
        /// This property is optional and will default to the standard texture for batteries or power cells.
        /// </summary>
        public virtual Texture2D CustomSkin => null;

        /// <summary>
        /// Override this value if you want your item to not be allowed in Battery and Power Cell chargers.
        /// </summary>
        public virtual bool ExcludeFromChargers => false;

        /// <summary>
        /// Override this optional method if you want to make changes to the your item's <see cref="GameObject"/> as it is being spawned from prefab.<br/>
        /// Use this if you want to add or modify components of your item.
        /// </summary>
        /// <param name="gameObject">The item's gameobject.</param>
        public virtual void EnhanceGameObject(GameObject gameObject)
        {
        }

        /// <summary>
        /// Allows mods to adds their own custom batteries directly. The plugin pack will be patched and the modded battery data returned.
        /// </summary>
        /// <param name="packItem">The battery data.</param>
        /// <returns>
        /// A <see cref="CbItemPack" /> containing the patched <see cref="SMLHelper.V2.Assets.ModPrefab" /> intance for the battery requested.
        /// </returns>
        public CbItemPack PatchAsBattery()
        {
            string name = this.GetType().Assembly.GetName().Name;
            Logger.Log(Logger.Level.Info, $"Received Custom Battery pack from '{name}'");

            var pack = new CbItemPack(name, this, ItemTypes.Battery);

            pack.Patch();

            return pack;
        }

        /// <summary>
        /// Allows mods to adds their own custom power cells directly. The plugin pack will be patched and the modded power cell data returned.
        /// </summary>
        /// <param name="packItem">The power cell data.</param>
        /// <returns>
        /// A <see cref="CbItemPack" /> containing the patched <see cref="SMLHelper.V2.Assets.ModPrefab" /> intance for the power cell requested.
        /// </returns>
        public CbItemPack PatchAsPowerCell()
        {
            string name = this.GetType().Assembly.GetName().Name;
            Logger.Log(Logger.Level.Info, $"Received Custom Power Cell pack from '{name}'");

            var pack = new CbItemPack(name, this, ItemTypes.PowerCell);

            pack.Patch();

            return pack;
        }
    }
}
