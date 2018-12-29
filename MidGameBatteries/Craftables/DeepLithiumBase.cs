namespace MidGameBatteries.Craftables
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal abstract class DeepLithiumBase : Craftable
    {
        private const string BatteryPowerCraftingTab = "BatteryPower";
        private const string ElectronicsCraftingTab = "Electronics";
        private const string ResourcesCraftingTab = "Resources";
        private const string MgBatteryAssets = @"MidGameBatteries/Assets";

        public static TechType BatteryID { get; protected set; }
        public static TechType PowerCellID { get; protected set; }

        internal static void PatchAll()
        {
            AdjustCraftingTree();
            PatchCraftables();
        }

        private static void PatchCraftables()
        {
            var lithiumBattery = new DeepLithiumBattery();
            lithiumBattery.Patch();

            var lithiumPowerCell = new DeepLithiumPowerCell(lithiumBattery);
            lithiumPowerCell.Patch();
        }

        private static void AdjustCraftingTree()
        {
            var tabIcon = ImageUtils.LoadSpriteFromFile(MgBatteryAssets + @"/CraftingTabIcon.png");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, BatteryPowerCraftingTab, "Batteries and Power Cells", tabIcon, ResourcesCraftingTab);

            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.Battery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PowerCell.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PrecursorIonBattery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PrecursorIonPowerCell.ToString());

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Battery, ResourcesCraftingTab, BatteryPowerCraftingTab);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonBattery, ResourcesCraftingTab, BatteryPowerCraftingTab);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PowerCell, ResourcesCraftingTab, BatteryPowerCraftingTab);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonPowerCell, ResourcesCraftingTab, BatteryPowerCraftingTab);
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
        public override string AssetsFolder { get; } = MgBatteryAssets;
        public override string[] StepsToFabricatorTab { get; } = new[] { ResourcesCraftingTab, BatteryPowerCraftingTab };
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
