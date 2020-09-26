namespace CyclopsSolarUpgrades.Craftables
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CyclopsSolarChargerMk2 : CyclopsUpgrade
    {
        internal const float BatteryCapacity = 120f;

        private readonly CyclopsSolarCharger previousTier;
        public CyclopsSolarChargerMk2(CyclopsSolarCharger cyclopsSolarCharger)
            : base("CyclopsSolarChargerMk2",
                   "Cyclops Solar Charger Mk2",
                   "Improved solar charging for the Cyclops with additional backup power.\nStacks with other solar chargers.")
        {
            previousTier = cyclopsSolarCharger;
            OnStartedPatching += () =>
            {
                if (!previousTier.IsPatched)
                    previousTier.Patch();
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };
        public override TechType RequiredForUnlock => TechType.Workbench;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(previousTier.TechType, 1),
                    new Ingredient(TechType.Battery, 2),
                    new Ingredient(TechType.WiringKit, 1)
                }
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "SolarBackupBattery2";
            pCell._capacity = BatteryCapacity;

            return obj;
        }
    }
}
