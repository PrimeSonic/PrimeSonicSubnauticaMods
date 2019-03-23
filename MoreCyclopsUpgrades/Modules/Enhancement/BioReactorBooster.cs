namespace MoreCyclopsUpgrades.Modules.Enhancement
{
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class BioReactorBooster : CyclopsModule
    {
        private const string MaxBoostKey = "CyBioBoostMaxed";
        public static string MaxBoostAchived => Language.main.Get(MaxBoostKey);

        private const string CannotRemoveKey = "CyBioCannotShrink";
        public static string CannotRemove => Language.main.Get(CannotRemoveKey);

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

        protected override void Patch()
        {
            base.Patch();
            LanguageHandler.SetLanguageLine(MaxBoostKey, "Maximum boost to cyclops bioreactors achieved");
            LanguageHandler.SetLanguageLine(CannotRemoveKey, "Cannot remove BioReactor Booster while bioreactor is too full");
        }
    }
}
