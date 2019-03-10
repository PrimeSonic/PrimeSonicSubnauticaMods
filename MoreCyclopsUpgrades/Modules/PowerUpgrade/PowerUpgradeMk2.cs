namespace MoreCyclopsUpgrades.Modules.PowerUpgrade
{
    using SMLHelper.V2.Crafting;

    internal class PowerUpgradeMk2 : CyclopsModule
    {
        internal PowerUpgradeMk2()
            : base("PowerUpgradeModuleMk2",
                  "Cyclops Engine Efficiency Module MK2",
                  "Additional enhancement to engine efficiency. Silent running, Sonar, and Shield optimized. Does not stack.",
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
                    new Ingredient(TechType.PowerUpgradeModule, 1),
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Sulphur, 2) // Did you make it to the Lost River yet?
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            PowerUpgradeMk2ID = techTypeID;
        }
    }
}
