namespace MoreCyclopsUpgrades.API.Upgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    /// <summary>
    /// Extends the <see cref="Craftable"/> class with handling and defaults specific for Cyclops upgrade modules.
    /// </summary>
    /// <seealso cref="Craftable" />
    public abstract class CyclopsUpgrade : Equipable
    {
        /// <summary>
        /// Initializes a new instance of the <seealso cref="Craftable"/> <see cref="CyclopsUpgrade"/> class.<para/>
        /// Any item created with this class with automatically be equipable in the Cyclops.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="T:TechType" /> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected CyclopsUpgrade(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            base.OnFinishedPatching += () =>
            {
                if (this.SortAfter == TechType.None)
                    CraftDataHandler.AddToGroup(this.GroupForPDA, this.CategoryForPDA, this.TechType);
                else
                    CraftDataHandler.AddToGroup(this.GroupForPDA, this.CategoryForPDA, this.TechType, this.SortAfter);
            };
        }

        /// <summary>
        /// Gets the type of equipment slot this item can fit into.
        /// </summary>
        public sealed override EquipmentType EquipmentType => EquipmentType.CyclopsModule;

        /// <summary>
        /// Overriden to ensure this item appearas within the <see cref="TechGroup.Cyclops"/> group in the PDA blurprints menu.
        /// </summary>
        public sealed override TechGroup GroupForPDA => TechGroup.Cyclops;

        /// <summary>
        /// Overrides to ensure this item appears within the <see cref="TechCategory.CyclopsUpgrades"/> category in the PDA blueprints menu.
        /// </summary>
        public sealed override TechCategory CategoryForPDA => TechCategory.CyclopsUpgrades;

        /// <summary>
        /// Gets the prefab template used to clone new instances of this upgrade module.<para/>
        /// Defaults to <see cref="TechType.CyclopsThermalReactorModule"/> which is enough for most any Cyclops upgrade module.
        /// </summary>
        /// <value>
        /// The prefab template.
        /// </value>
        protected virtual TechType PrefabTemplate { get; } = TechType.CyclopsThermalReactorModule;

        /// <summary>
        /// Overriden to set to have the <see cref="TechType.Cyclops" /> be required before this upgrade module can be unlocked.
        /// If not overriden, it this item will be unlocked from the start of the game.
        /// </summary>
        public override TechType RequiredForUnlock => TechType.Cyclops; // Default can be overridden by child classes

        /// <summary>
        /// Override this to set which other module in the PDA this upgrade module should be sorted after.
        /// </summary>
        public virtual TechType SortAfter => TechType.None;

        /// <summary>
        /// Gets the prefab game object. Set up your prefab components here.<para/>
        /// A default implementation is already provided which creates the new item by modifying a clone of the item defined in <see cref="PrefabTemplate"/>.
        /// </summary>
        /// <returns>
        /// The game object to be instantiated into a new in-game entity.
        /// </returns>
        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabTemplate);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        /// <summary>
        /// A utility method that spawns a cyclops upgrade module by TechType ID.
        /// </summary>
        /// <param name="techTypeID">The tech type ID.</param>
        /// <returns>A new <see cref="InventoryItem"/> that wraps up a <see cref="Pickupable"/> game object.</returns>
        public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
        {
            try
            {
                GameObject prefab = CraftData.GetPrefabForTechType(techTypeID);

                if (prefab == null)
                    return null;

                var gameObject = GameObject.Instantiate(prefab);

                Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
                return new InventoryItem(pickupable);
            }
            catch
            {
                return null;
            }
        }
    }
}
