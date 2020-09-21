namespace UpgradedVehicles
{
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    public abstract class VehicleUpgradeModule : Craftable
    {
        protected VehicleUpgradeModule(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            base.OnFinishedPatching += PostPatch;
        }

        public sealed override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public sealed override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        protected virtual TechType PrefabTemplate { get; } = TechType.SeamothSonarModule;
        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;
        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override string[] StepsToFabricatorTab => new[] { "CommonModules" };
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabTemplate);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        private void PostPatch()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);
            CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Passive);
        }
    }
}