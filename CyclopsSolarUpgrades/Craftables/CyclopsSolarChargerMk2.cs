namespace CyclopsSolarUpgrades.Craftables
{
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CyclopsSolarChargerMk2 : CyclopsUpgrade, IUpgradeHandlerCreator, ICyclopsChargerCreator
    {
        internal const float BatteryCapacity = 100f;

        private readonly CyclopsSolarCharger previousTier;
        public CyclopsSolarChargerMk2(CyclopsSolarCharger cyclopsSolarCharger)
            : base("CyclopsSolarChargerMk2",
                   "Cyclops Solar Charger Mk2",
                   "Improved solar charging for the Cyclops with additional backup power.\n" +
                  $"Stacks with other solar chargers up to a maximum of {AmbientEnergyUpgradeHandler.MaxChargers} total solar chargers.")
        {
            previousTier = cyclopsSolarCharger;
            OnStartedPatching += () =>
            {
                if (!previousTier.IsPatched)
                    previousTier.Patch();
            };

            OnFinishedPatching += () =>
            {
                MCUServices.Register.CyclopsCharger(this);
                MCUServices.Register.CyclopsUpgradeHandler(this);
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder { get; } = "CyclopsSolarUpgrades/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(previousTier.TechType, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 1),
                    new Ingredient(TechType.Diamond, 1),
                    new Ingredient(TechType.Lithium, 1),
                }
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "SolarBackupBattery2";
            pCell._capacity = BatteryCapacity;

            return obj;
        }

        private SolarUpgrade CreateSolarUpgrade(SubRoot cyclops)
        {
            return new SolarUpgrade(previousTier.TechType, this.TechType, cyclops);
        }

        public UpgradeHandler CreateUpgradeHandler(SubRoot cyclops)
        {
            return CreateSolarUpgrade(cyclops);
        }

        private SolarCharger CreateSolarCharger(SubRoot cyclops)
        {
            return new SolarCharger(previousTier.TechType, this.TechType, cyclops);
        }

        public ICyclopsCharger CreateCyclopsCharger(SubRoot cyclops)
        {
            return CreateSolarCharger(cyclops);
        }
    }
}
