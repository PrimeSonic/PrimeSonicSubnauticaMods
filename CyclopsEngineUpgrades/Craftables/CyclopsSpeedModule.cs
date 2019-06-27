namespace CyclopsEngineUpgrades.Craftables
{
    using CyclopsEngineUpgrades.Handlers;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class CyclopsSpeedModule : CyclopsUpgrade
    {
        internal static TechType TechTypeID { get; private set; }

        private const string MaxRatingKey = "CySpeedMaxed";
        public static string MaxRatingAchived => Language.main.Get(MaxRatingKey);

        private const string SpeedRatingKey = "CySpeedCurrent";
        public static string SpeedRatingText(int boosterCount, int multiplier)
        {
            return Language.main.GetFormat(SpeedRatingKey, boosterCount, multiplier);
        }

        public CyclopsSpeedModule()
            : base("CyclopsSpeedModule",
                   "Cyclops Speed Boost Module",
                   "Increases the drive power of the cyclops engines, adding greater speeds at the cost of higher energy consumption.\n" +
                   $"Can stack up to {EngineManager.MaxSpeedBoosters} boosters for maximum effect at highest cost.")
        {
            OnFinishedPatching += () =>
            {
                TechTypeID = this.TechType;
                LanguageHandler.SetLanguageLine(MaxRatingKey, "Maximum speed rating reached");
                LanguageHandler.SetLanguageLine(SpeedRatingKey, "Speed rating is now at +{0} ({1}%).");                
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder { get; } = "CyclopsEngineUpgrades/Assets";
        public override string[] StepsToFabricatorTab { get; } = MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Lubricant, 1),
                }
            };
        }

        internal SpeedHandler CreateSpeedUpgradeHandler(SubRoot cyclops)
        {
            return new SpeedHandler(this, cyclops);
        }
    }
}
