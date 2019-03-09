namespace MoreCyclopsUpgrades.Modules.PowerUpgrade
{
    using SMLHelper.V2.Crafting;

    internal class PowerUpgradeMk3 : CyclopsModule
    {
        internal PowerUpgradeMk3()
            : base("PowerUpgradeModuleMk3",
                  "Cyclops Engine Efficiency Module MK3",
                  "Maximum engine efficiency. Silent running, Sonar, and Shield greatly optimized. Does not stack.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        {
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(PowerUpgradeMk2ID, 1),
                    new Ingredient(TechType.Kyanite, 1), // More uses for Kyanite!
                    new Ingredient(TechType.Diamond, 1),
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            PowerUpgradeMk3ID = techTypeID;
        }
    }
}
