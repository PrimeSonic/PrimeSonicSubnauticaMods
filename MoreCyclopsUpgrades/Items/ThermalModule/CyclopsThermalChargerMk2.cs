namespace MoreCyclopsUpgrades.Items.ThermalModule
{
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyclopsThermalChargerMk2 : CyclopsUpgrade, ICyclopsChargerCreator, IUpgradeHandlerCreator
    {
        internal const float BatteryCapacity = 120f;

        private const string MaxThermalReachedKey = "MaxThermalMsg";
        internal static string MaxThermalReached()
        {
            return Language.main.Get(MaxThermalReachedKey);
        }

        public const TechType RequiredAnalysisItem = TechType.CyclopsThermalReactorModule;

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder { get; } = "MoreCyclopsUpgrades/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.Workbench;
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        public CyclopsThermalChargerMk2()
            : base("CyclopsThermalChargerMk2",
                   "Cyclops Thermal Reactor Mk2",
                   "Improved thermal charging and with integrated batteries to store a little extra power for when it get cold.")
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
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.Benzene, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.CopperWire, 1),
                }
            };
        }

        public ICyclopsCharger CreateCyclopsCharger(SubRoot cyclops)
        {
            return new ThermalCharger(cyclops, this.TechType);
        }

        public UpgradeHandler CreateUpgradeHandler(SubRoot cyclops)
        {
            return new ThermalUpgradeHandler(TechType.CyclopsThermalReactorModule, this.TechType, cyclops);
        }
    }
}
