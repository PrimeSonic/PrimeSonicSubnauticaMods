namespace MoreScannerRoomUpgrades.Craftables
{
    using System;
    using System.Collections.Generic;
    using Monobehaviors;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class VehicleMapScannerModule : Craftable
    {
        private static VehicleMapScannerModule PatchSingleton;
        internal static TechType ItemID { get; private set; }

        public static void PatchAll()
        {
            PatchSingleton = new VehicleMapScannerModule();

            PatchSingleton.Patch();
        }

        public VehicleMapScannerModule()
            : base(classId: "VehicleMapScanner",
                   friendlyName: "Vehicle Resource Scanner",
                   description: "A cut down but portable resource scanne that can be used from within vehicle. " + Environment.NewLine +
                                "CAUTION: Be mindful of energy consumption while using this device.")
        {
            OnFinishedPatching += PostPatchUpdates;
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.MapRoom;
        public override TechGroup GroupForPDA { get; } = TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA { get; } = TechCategory.VehicleUpgrades;
        public override string AssetsFolder { get; } = @"MoreScannerRoomUpgrades/Assets";

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.VehiclePowerUpgradeModule);
            var obj = GameObject.Instantiate(prefab);

            obj.AddComponent<VehicleMapScanner>();

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(4)
                {
                    new Ingredient(TechType.MapRoomCamera, 1),
                    new Ingredient(TechType.MapRoomUpgradeScanRange, 1),
                    new Ingredient(TechType.SeamothSonarModule, 1),
                    new Ingredient(TechType.MapRoomHUDChip, 1),
                },
                LinkedItems =
                {
                    TechType.MapRoomHUDChip
                }
            };
        }

        private void PostPatchUpdates()
        {
            ItemID = this.TechType;
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);
            CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Toggleable);
        }
    }
}
