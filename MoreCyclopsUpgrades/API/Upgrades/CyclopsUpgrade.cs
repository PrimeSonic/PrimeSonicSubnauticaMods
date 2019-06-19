namespace MoreCyclopsUpgrades.API.Upgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    /// <summary>
    /// Extends the <see cref="Craftable"/> class with handling and defaults specific for Cyclops upgrade modules.
    /// </summary>
    /// <seealso cref="SMLHelper.V2.Assets.Craftable" />
    public abstract class CyclopsUpgrade : Craftable
    {
        protected CyclopsUpgrade(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            base.OnFinishedPatching += MakeEquipable;
        }

        public sealed override TechGroup GroupForPDA { get; } = TechGroup.Cyclops;
        public sealed override TechCategory CategoryForPDA { get; } = TechCategory.CyclopsUpgrades;
        protected virtual TechType PrefabTemplate { get; } = TechType.CyclopsThermalReactorModule;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabTemplate);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        private void MakeEquipable()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, this.TechType);
        }

        public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
        {
            var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
