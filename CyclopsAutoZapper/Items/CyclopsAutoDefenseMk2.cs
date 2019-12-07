namespace CyclopsAutoZapper
{
    using System.IO;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class CyclopsAutoDefenseMk2 : CyclopsUpgrade
    {
        private readonly TechType autoDefenseMk1;

        public CyclopsAutoDefenseMk2(CyclopsAutoDefense zapperMk1)
            : base("CyclopsZapperModuleMk2",
                   "Cyclops Auto Defense System Mk2",
                   "Self contained, automated, anti-predator electrical defense system for the Cyclops.")
        {
            autoDefenseMk1 = zapperMk1.TechType;
        }

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string AssetsFolder => Path.Combine("CyclopsAutoZapper", "Assets");
        public override TechType RequiredForUnlock => autoDefenseMk1;
        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(autoDefenseMk1, 1),
                    new Ingredient(TechType.SeamothElectricalDefense, 1),
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }
    }
}
