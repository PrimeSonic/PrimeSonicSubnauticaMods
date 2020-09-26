namespace CyclopsNuclearUpgrades
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyclopsNuclearModule : CyclopsUpgrade
    {
        internal const float NuclearEnergyPotential = 6000f; // Less than the normal 20k for balance

        private const string MaxNuclearReachedKey = "MaxNuclearMsg";
        internal static string MaxNuclearReachedMsg => Language.main.Get(MaxNuclearReachedKey);

        private const string OverheatKey = "CyNukOverheat";
        internal static string OverheatMsg => Language.main.Get(OverheatKey);

        private const string DepletedEventKey = "CyNukeDepleted";
        internal static string DepletedEventMsg => Language.main.Get(DepletedEventKey);

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public override TechType RequiredForUnlock { get; } = TechType.BaseNuclearReactor;

        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        public override float CraftingTime => 12f;

        internal CyclopsNuclearModule()
            : base("CyclopsNuclearModule",
                   "Cyclops Nuclear Reactor Module",
                   "Recharge your Cyclops using this portable nuclear reactor.\n" +
                   "Warning! Prolonged use will overheat the module, causing temporary shutdown.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(MaxNuclearReachedKey, "Max number of nuclear chargers.");
                LanguageHandler.SetLanguageLine(OverheatKey, "SHUTDOWN");
                LanguageHandler.SetLanguageLine(DepletedEventKey, "Nuclear Reactor Module depleted");
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            // The battery component makes it easy to track the charge and saving the data is automatic.
            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "NuclearBattery";
            pCell._capacity = NuclearEnergyPotential;

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.UraniniteCrystal, 1),
                    new Ingredient(TechType.Lead, 1),
                    new Ingredient(TechType.Titanium, 3),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Aerogel, 1),
                }
            };
        }
    }
}
