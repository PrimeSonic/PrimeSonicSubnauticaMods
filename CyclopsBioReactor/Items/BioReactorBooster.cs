namespace CyclopsBioReactor.Items
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class BioReactorBooster : CyclopsUpgrade
    {
        private const string MaxBoostKey = "CyBioBoostMaxed";
        public static string MaxBoostAchived => Language.main.Get(MaxBoostKey);

        private const string CannotRemoveKey = "CyBioCannotShrink";
        public static string CannotRemove => Language.main.Get(CannotRemoveKey);

        public override TechType RequiredForUnlock { get; } = TechType.BaseBioReactor;
        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder { get; } = "CyclopsBioReactor/Assets";
        public override string[] StepsToFabricatorTab { get; } = MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;        

        private BioReactorBooster(string[] tabs)
            : base("BioReactorBooster",
                   "Cyclops Bioreactor Booster",
                   "Enhances all bioreactors onboard the Cyclops.")
        {
        }

        public BioReactorBooster() 
            : base("BioReactorBooster",
                   "Cyclops Bioreactor Booster",
                   "Enhances all bioreactors onboard the Cyclops.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(MaxBoostKey, "Maximum boost to cyclops bioreactors achieved");
                LanguageHandler.SetLanguageLine(CannotRemoveKey, "Cannot remove BioReactor Booster while bioreactor is too full");
            };
        }

        protected override TechData GetBlueprintRecipe()
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
    }
}
