namespace CyclopsSolarUpgrades.Craftables
{
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Crafting;

    internal class CyclopsSolarCharger : CyclopsUpgrade
    {
        public CyclopsSolarCharger()
            : base("CyclopsSolarCharger",
                   "Cyclops Solar Charger",
                   "Allows your cyclops to recharge plentiful power of the sun itself.\n" +
                  $"Stacks with other solar chargers up to a maximum of {Solar.MaxSolarChargers} total solar chargers.")
        {
            OnFinishedPatching += () =>
            {
                Solar.CyclopsSolarCharger = this.TechType;
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder { get; } = "CyclopsSolarUpgrades/Assets";
        public override string[] StepsToFabricatorTab
        {
            get
            {
                if (MCUServices.Client.CyclopsFabricatorHasCyclopsModulesTab)
                    return MCUServices.Client.StepsToCyclopsModulesTab;

                return base.StepsToFabricatorTab;
            }
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Quartz, 3),
                    new Ingredient(TechType.Titanium, 1)
                }
            };
        }
    }
}
