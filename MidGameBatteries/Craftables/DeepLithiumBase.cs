namespace MidGameBatteries.Craftables
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal abstract class DeepLithiumBase : Craftable
    {
        public static TechType BatteryID { get; protected set; }
        public static TechType PowerCellID { get; protected set; }

        internal static void PatchCraftables()
        {
            var lithiumBattery = new DeepLithiumBattery();
            lithiumBattery.Patch();

            var lithiumPowerCell = new DeepLithiumPowerCell(lithiumBattery);
            lithiumPowerCell.Patch();
        }
        protected abstract TechType BaseType { get; }
        protected abstract float PowerCapacity { get; }
        protected abstract EquipmentType ChargerType { get; }

        protected DeepLithiumBase(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            OnFinishedPatching += SetEquipmentType;
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Electronics;
        public override string AssetsFolder { get; } = @"MidGameBatteries/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "Resources", "Electronics" };
        public override TechType RequiredForUnlock { get; } = TechType.WhiteMushroom;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.BaseType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}Battery";

            return obj;
        }

        private void SetEquipmentType() => CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);
    }
}
