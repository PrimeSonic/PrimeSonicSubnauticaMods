namespace MoreCyclopsUpgrades.Modules.Enhancement
{
    using SMLHelper.V2.Crafting;

    internal class BioReactorBooster : CyclopsModule
    {
        internal BioReactorBooster(bool fabModPresent) : this(fabModPresent ? null : new[] { "CyclopsMenu" })
        {
        }

        private BioReactorBooster(string[] tabs)
            : base("BioReactorBooster",
                   "Cyclops Bioreactor Booster",
                   "Enhances all bioreactors onboard the Cyclops.",
                   CraftTree.Type.CyclopsFabricator,
                   tabs,
                   TechType.BaseBioReactor)
        {
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.EnameledGlass, 1),
                    new Ingredient(TechType.HydrochloricAcid, 1),
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(TechType.TreeMushroomPiece, 4),
                    new Ingredient(TechType.SnakeMushroomSpore, 1), // 2x2  item
                    new Ingredient(TechType.PurpleRattleSpore, 4),
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            BioReactorBoosterID = techTypeID;
        }
    }
}
