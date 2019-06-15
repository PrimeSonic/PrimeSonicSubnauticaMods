namespace MoreCyclopsUpgrades.API
{
    using MoreCyclopsUpgrades.Managers;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    public abstract class CyclopsUpgrade : Craftable
    {
        protected CyclopsUpgrade(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            base.OnFinishedPatching += MakeEquipable;
            base.OnFinishedPatching += RegisterWithUpgradeManager;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.Cyclops;
        public override TechCategory CategoryForPDA { get; } = TechCategory.CyclopsUpgrades;
        protected virtual TechType PrefabTemplate { get; } = TechType.CyclopsThermalReactorModule;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabTemplate);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected abstract UpgradeHandler CreateUpgradeHandler(SubRoot cyclops);

        private void MakeEquipable()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, this.TechType);
        }

        private void RegisterWithUpgradeManager()
        {
            UpgradeManager.RegisterHandlerCreator(CreateUpgradeHandler);
        }
    }
}
