namespace MidGameBatteries.Craftables
{
    using System.IO;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal abstract class DeepBatteryCellBase : Craftable
    {
        private const string BatteryPowerCraftingTab = "BatteryPower";
        private const string ElectronicsCraftingTab = "Electronics";
        private const string ResourcesCraftingTab = "Resources";
        private const string MgBatteryAssets = @"MidGameBatteries/Assets";

        private static readonly string[] PathToNewTab = new[] { ResourcesCraftingTab, ElectronicsCraftingTab, BatteryPowerCraftingTab };

        // Class level elements

        public static TechType BatteryID { get; protected set; }
        public static TechType PowerCellID { get; protected set; }

        internal static void PatchAll()
        {
            string mainDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Create a new crafting tree tab for batteries and power cells
            string assetsFolder = Path.Combine(mainDirectory, "Assets");
            string pathToIcon = Path.Combine(assetsFolder, @"CraftingTabIcon.png");

            Atlas.Sprite tabIcon = ImageUtils.LoadSpriteFromFile(pathToIcon);
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, BatteryPowerCraftingTab, "Batteries and Power Cells", tabIcon, ResourcesCraftingTab, ElectronicsCraftingTab);

            // Remove the original batteries from the Electronics tab
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.Battery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PowerCell.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PrecursorIonBattery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResourcesCraftingTab, ElectronicsCraftingTab, TechType.PrecursorIonPowerCell.ToString());

            // And add them back in on the new Batteries and PowerCells tab
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Battery, PathToNewTab);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PowerCell, PathToNewTab);

            var config = new DeepConfig();
            config.ReadConfigFile(mainDirectory);

            QuickLogger.Info($"Selected PowerStyle in config: {config.SelectedPowerStyle} - (Battery Capacity:{Mathf.RoundToInt(config.BatteryCapacity)})");

            QuickLogger.Info("Patching DeepBattery");
            var lithiumBattery = new DeepBattery(config.BatteryCapacity);
            lithiumBattery.Patch();

            QuickLogger.Info("Patching DeepPowerCell");
            var lithiumPowerCell = new DeepPowerCell(lithiumBattery);
            lithiumPowerCell.Patch();

            // Add the Ion Batteries after the Deep Batteries
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonBattery, PathToNewTab);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonPowerCell, PathToNewTab);
        }

        protected abstract TechType BaseType { get; } // Should only ever be Battery or PowerCell
        protected abstract float PowerCapacity { get; }
        protected abstract EquipmentType ChargerType { get; } // Should only ever be BatteryCharger or PowerCellCharger

        // Instance level elements

        protected DeepBatteryCellBase(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            // This event will be invoked after all patching done by the Craftable class is complete
            OnFinishedPatching += SetEquipmentType;
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Electronics;
        public override string AssetsFolder { get; } = MgBatteryAssets;
        public override string[] StepsToFabricatorTab { get; } = PathToNewTab;
        public override TechType RequiredForUnlock { get; } = TechType.WhiteMushroom; // These will unlock once the player acquires a Deep Shroom

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.BaseType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}BatteryCell";

            return obj;
        }

        private void SetEquipmentType()
        {
            // This is necessary to allow the new battery and power cell to be added to their respective charging stations
            CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);
        }
    }
}
