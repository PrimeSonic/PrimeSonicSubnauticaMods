namespace MoreCyclopsUpgrades.Modules.Recharging.Solar
{
    using SMLHelper.V2.Crafting;

    internal class SolarCharger : CyclopsModule
    {
        internal SolarCharger(bool fabModPresent) : this(fabModPresent ? new[] { "CyclopsModules" } : new string[0])
        {
        }

        private SolarCharger(string[] tabs)
            : base("CyclopsSolarCharger",
                  "Cyclops Solar Charger",
                  "Recharge your Cyclops with the plentiful power of the sun itself.",
                  CraftTree.Type.CyclopsFabricator,
                  tabs,
                  TechType.Cyclops)
        {
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.EnameledGlass, 1),
                    new Ingredient(TechType.Quartz, 2),
                    new Ingredient(TechType.Titanium, 2)
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            SolarChargerID = techTypeID;
        }
    }

}
