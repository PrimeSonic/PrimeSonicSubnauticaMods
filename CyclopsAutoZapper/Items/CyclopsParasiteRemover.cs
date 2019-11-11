namespace CyclopsAutoZapper
{
    using System.IO;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class CyclopsParasiteRemover : CyclopsUpgrade
    {
        public CyclopsParasiteRemover()
            : base("CyclopsAntiLarvaModule",
                   "Cyclops Auto Parasite Remover",
                   "Automatically pulses the Cyclops shield at low power to detach parasites.")
        {
        }

        public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder => Path.Combine("CyclopsAutoZapper", "Assets");
        public override TechType RequiredForUnlock => TechType.CyclopsShieldModule;
        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;
        public override string IconFileName => "CyclopsAntiParasite.png";

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.Titanium, 1),
                }
            };
        }
    }
}
