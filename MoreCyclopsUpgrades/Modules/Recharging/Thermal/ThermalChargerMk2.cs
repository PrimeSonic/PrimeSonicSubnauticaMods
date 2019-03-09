namespace MoreCyclopsUpgrades.Modules.Recharging.Thermal
{
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class ThermalChargerMk2 : CyclopsModule
    {
        internal const float BatteryCapacity = 100f;

        public const TechType RequiredAnalysisItem = TechType.CyclopsThermalReactorModule;

        internal ThermalChargerMk2()
            : base("CyclopsThermalChargerMk2",
                  "Cyclops Thermal Reactor Mk2",
                  "Improved thermal charging and with integrated batteries to store a little extra power for when it get cold.",
                  CraftTree.Type.Workbench,
                  new[] { "CyclopsMenu" },
                  TechType.Workbench)
        {
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.CyclopsThermalReactorModule, 1),
                    new Ingredient(TechType.Battery, 2),
                    new Ingredient(TechType.Sulphur, 1),
                    new Ingredient(TechType.Kyanite, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.CopperWire, 1),
                }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            ThermalChargerMk2ID = techTypeID;
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "ThermalBackupBattery";
            pCell._capacity = BatteryCapacity;

            return obj;
        }
    }
}
