namespace CyclopsSpeedUpgrades
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyclopsSpeedModule : CyclopsUpgrade
    {
        private const string MaxRatingKey = "CySpeedMaxed";
        public static string MaxRatingAchived => Language.main.Get(MaxRatingKey);

        private const string SpeedRatingKey = "CySpeedCurrent";
        public static string SpeedRatingText(int boosterCount, float multiplier)
        {
            return Language.main.GetFormat(SpeedRatingKey, boosterCount, Mathf.RoundToInt(multiplier * 100f));
        }

        public CyclopsSpeedModule()
            : base("CyclopsSpeedModule",
                   "Cyclops Speed Boost Module",
                   "Increases the drive power of the cyclops engines, adding greater speeds at the cost of higher energy consumption.\n" +
                   $"Can stack up to {SpeedHandler.MaxSpeedBoosters} boosters for maximum effect at highest cost.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(MaxRatingKey, "Maximum speed rating reached");
                LanguageHandler.SetLanguageLine(SpeedRatingKey, "Speed rating is now at +{0} ({1}%).");                
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
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

        internal SpeedOverlay CreateSpeedIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new SpeedOverlay(icon, upgradeModule, this);
        }
    }
}
