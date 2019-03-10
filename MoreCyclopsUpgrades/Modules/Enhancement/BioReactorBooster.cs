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
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Glass, 1),
                    new Ingredient(TechType.HydrochloricAcid, 2),
                    new Ingredient(TechType.TreeMushroomPiece, 3),
                    new Ingredient(TechType.PinkMushroomSpore, 3),
                    new Ingredient(TechType.SnakeMushroomSpore, 3),
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            BioReactorBoosterID = techTypeID;
        }
    }
}
