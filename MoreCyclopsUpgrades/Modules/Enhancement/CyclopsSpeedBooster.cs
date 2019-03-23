namespace MoreCyclopsUpgrades.Modules.Enhancement
{
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class CyclopsSpeedBooster : CyclopsModule
    {
        private const string MaxRatingKey = "CySpeedMaxed";
        public static string MaxRatingAchived => Language.main.Get(MaxRatingKey);

        private const string SpeedRatingKey = "CySpeedMaxed";
        public static string SpeedRatingText(int boosterCount, int multiplier)
        {
            return Language.main.GetFormat(SpeedRatingKey, boosterCount, multiplier);
        }

        internal CyclopsSpeedBooster(bool fabModPresent) : this(fabModPresent ? null : new[] { "CyclopsMenu" })
        {
        }

        private CyclopsSpeedBooster(string[] tabs)
            : base("CyclopsSpeedModule",
                  "Cyclops Speed Boost Module",
                  "Allows the cyclops engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.",
                  CraftTree.Type.CyclopsFabricator,
                  tabs,
                  TechType.CyclopsHullModule1)
        {
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Magnetite, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            SpeedBoosterModuleID = techTypeID;
        }

        protected override void Patch()
        {
            base.Patch();
            LanguageHandler.SetLanguageLine(MaxRatingKey, "Maximum speed rating reached");
            LanguageHandler.SetLanguageLine(SpeedRatingKey, "Speed rating is now at +{0} : {1}");
        }
    }
}
