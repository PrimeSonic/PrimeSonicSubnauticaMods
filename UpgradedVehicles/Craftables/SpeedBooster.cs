namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class SpeedBooster : Craftable
    {
        private static SpeedBooster Singleton { get; } = new SpeedBooster();

        internal static void HandlePatching() => Singleton.Patch();

        internal static TechType SpeedBoosterTechType => Singleton.TechType;

        public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override string[] StepsToFabricatorTab => new[] { "CommonModules" };
        public override TechType RequiredForUnlock => TechType.PowerUpgradeModule;

        public SpeedBooster()
            : base(classId: "SpeedModule",
                   friendlyName: "Speed Boost Module",
                   description: "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.")
        {
            base.OnFinishedPatching += PostPatch;
        }

        private void PostPatch() => CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);

        protected override TechData GetBlueprintRecipe() => new TechData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.Aerogel, 1),
                                 new Ingredient(TechType.Magnetite, 1),
                                 new Ingredient(TechType.Titanium, 2),
                             })
        };

        public override GameObject GetGameObject() => GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.VehicleArmorPlating));
    }
}
