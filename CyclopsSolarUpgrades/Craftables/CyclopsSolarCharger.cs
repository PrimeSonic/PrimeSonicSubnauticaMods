namespace CyclopsSolarUpgrades.Craftables
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class CyclopsSolarCharger : CyclopsUpgrade
    {
        private const string MaxSolarReachedKey = "MaxSolarMsg";
        internal static string MaxSolarReached()
        {
            return Language.main.Get(MaxSolarReachedKey);
        }

        public CyclopsSolarCharger()
            : base("CyclopsSolarCharger",
                   "Cyclops Solar Charger",
                   "Recharges the Cyclops power cells while in sunlight.\nStacks with other solar chargers.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(MaxSolarReachedKey, "Max number of solar chargers reached.");
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override string[] StepsToFabricatorTab { get; } = MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;
        public override TechType RequiredForUnlock { get; } = TechType.SeamothSolarCharge;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Glass, 1),
                    new Ingredient(TechType.EnameledGlass, 1),
                    new Ingredient(TechType.Titanium, 1),
                }
            };
        }
    }
}
