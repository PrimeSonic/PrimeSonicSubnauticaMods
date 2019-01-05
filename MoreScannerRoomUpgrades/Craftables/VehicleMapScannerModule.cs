namespace MoreScannerRoomUpgrades.Craftables
{
    using System;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class VehicleMapScannerModule : Craftable
    {
        internal static TechType ItemID { get; private set; }

        public VehicleMapScannerModule()
            : base(classId: "VehicleMapScanner",
                   friendlyName: "Vehicle Resource Scanner",
                   description: "A cut down but portable resource scanne that can be used from within vehicle. " + Environment.NewLine +
                                "CAUTION: Be mindful of energy consumption while using this device.")
        {
            OnFinishedPatching += SetStaticTechType;
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.MapRoom;
        public override TechGroup GroupForPDA { get; } = TechGroup.MapRoomUpgrades;
        public override TechCategory CategoryForPDA { get; } = TechCategory.MapRoomUpgrades;
        public override string AssetsFolder { get; } = @".QMods/MoreScannerRoomUpgrades";

        public override GameObject GetGameObject() => throw new NotImplementedException(); // TODO
        protected override TechData GetBlueprintRecipe() => throw new NotImplementedException(); // TODO

        private void SetStaticTechType() => ItemID = this.TechType;
    }
}
