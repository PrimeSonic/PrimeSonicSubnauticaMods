namespace CyclopsEngineUpgrades.Craftables
{
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Crafting;

    internal class PowerUpgradeModuleMk2 : CyclopsUpgrade
    {
        public PowerUpgradeModuleMk2() 
            : base("PowerUpgradeModuleMk2",
                  "Cyclops Engine Efficiency Module MK2",
                  "Additional enhancement to engine efficiency.\n" +
                  "Silent running, Sonar, and Shield optimized.\n" +
                  "Does not stack with other engine upgrades.")
        {
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder { get; } = "CyclopsEngineUpgrades/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.PowerUpgradeModule, 1),
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Sulphur, 2) // Did you make it to the Lost River yet?
                }
            };
        }
    }
}
