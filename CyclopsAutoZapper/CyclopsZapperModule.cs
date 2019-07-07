namespace CyclopsAutoZapper
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class CyclopsZapperModule : CyclopsUpgrade
    {
        internal static TechType TechTypeID { get; private set; }

        public CyclopsZapperModule()
            : base("CyclopsZapperModule",
                   "Cyclops Auto Defense System",
                   "Extends and automates the Perimeter Defense System of a docked Seamoth to protect the Cyclops from aggressive fauna.")
        {
            OnFinishedPatching += () =>
            {
                TechTypeID = this.TechType;
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder { get; } = "CyclopsAutoZapper/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.SeamothElectricalDefense;
        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                Ingredients =
                {
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.Polyaniline, 1),
                }
            };
        }
    }
}
