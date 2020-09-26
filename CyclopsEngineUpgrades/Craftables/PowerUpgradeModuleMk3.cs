namespace CyclopsEngineUpgrades.Craftables
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class PowerUpgradeModuleMk3 : CyclopsUpgrade
    {
        private readonly PowerUpgradeModuleMk2 previousTier;

        public PowerUpgradeModuleMk3(PowerUpgradeModuleMk2 mk2Upgrade)
            : base("PowerUpgradeModuleMk3",
                  "Cyclops Engine Efficiency Module MK3",
                  "Maximum engine efficiency. Silent running, Sonar, and Shield greatly optimized.\n" +
                  "Does not stack with other engine upgrades.")
        {
            previousTier = mk2Upgrade;

            OnStartedPatching += () =>
            {
                if (!previousTier.IsPatched)
                    previousTier.Patch();
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(previousTier.TechType, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Nickel, 1),
                }
            };
        }
    }
}
