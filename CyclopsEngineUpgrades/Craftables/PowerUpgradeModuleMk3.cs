namespace CyclopsEngineUpgrades.Craftables
{
    using CyclopsEngineUpgrades.Handlers;
    using MoreCyclopsUpgrades.API;
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

            OnFinishedPatching += () =>
            {
                MCUServices.Client.RegisterUpgradeCreator(CreateEngineHandler);
            };
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
                    new Ingredient(previousTier.TechType, 1),
                    new Ingredient(TechType.Kyanite, 1), // More uses for Kyanite!
                    new Ingredient(TechType.Diamond, 1),
                }
            };
        }

        private EngineHandler CreateEngineHandler(SubRoot cyclops)
        {
            return new EngineHandler(previousTier, this, cyclops);
        }
    }
}
