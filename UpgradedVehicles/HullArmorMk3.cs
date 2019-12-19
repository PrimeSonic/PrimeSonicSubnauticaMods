namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class HullArmorMk3 : Craftable
    {
        internal static Craftable GetHullArmorMk3Craftable()
        {
            Singleton.Patch();

            return Singleton;
        }

        internal static TechType HullArmorMk3TechType => Singleton.TechType;

        private static HullArmorMk3 Singleton { get; } = new HullArmorMk3();

        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override string[] StepsToFabricatorTab => new[] { "CommonModules" };
        public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;

        public override string AssetsFolder => "UpgradedVehicles/Assets";

        protected HullArmorMk3()
            : base(classId: "HullArmorMk3",
                   friendlyName: "Hull Reinforcement Mk III",
                   description: "An even better hull upgrade. Equivalent to 3 regular Hull Reinforcements")
        {
            base.OnFinishedPatching += PostPatch;
        }

        private void PostPatch()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);
            CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Passive);
        }

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[1]
                             {
                                 new Ingredient(TechType.VehicleArmorPlating, 3)
                             })
        };

        public override GameObject GetGameObject() => GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.VehicleArmorPlating));
    }
}
