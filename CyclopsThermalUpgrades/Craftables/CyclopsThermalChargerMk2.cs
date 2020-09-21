namespace CyclopsThermalUpgrades.Craftables
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyclopsThermalChargerMk2 : CyclopsUpgrade
    {
        internal const float BatteryCapacity = 120f;

        private const string MaxThermalReachedKey = "MaxThermalMsg";
        internal static string MaxThermalReached()
        {
            return Language.main.Get(MaxThermalReachedKey);
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock { get; } = TechType.Workbench;
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };
        public override TechType SortAfter { get; } = TechType.CyclopsThermalReactorModule;

        public CyclopsThermalChargerMk2()
            : base("CyclopsThermalChargerMk2",
                   "Cyclops Thermal Reactor Mk2",
                   "Improved thermal charging with additional backup power.\nStacks with other thermal reactors.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(MaxThermalReachedKey, "Max number of thermal chargers reached.");
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "ThermalBackupBattery";
            pCell._capacity = BatteryCapacity;

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.CyclopsThermalReactorModule, 1),
                    new Ingredient(TechType.Battery, 2),
                    new Ingredient(TechType.WiringKit, 1)
                }
            };
        }
    }
}
