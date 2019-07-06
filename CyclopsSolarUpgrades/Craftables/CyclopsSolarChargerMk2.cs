namespace CyclopsSolarUpgrades.Craftables
{
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.PDA;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CyclopsSolarChargerMk2 : CyclopsUpgrade, IUpgradeHandlerCreator, ICyclopsChargerCreator, IIconOverlayCreator
    {
        internal const float BatteryCapacity = 120f;

        private readonly CyclopsSolarCharger previousTier;
        public CyclopsSolarChargerMk2(CyclopsSolarCharger cyclopsSolarCharger)
            : base("CyclopsSolarChargerMk2",
                   "Cyclops Solar Charger Mk2",
                   "Improved solar charging for the Cyclops with additional backup power.\nStacks with other solar chargers.")
        {
            previousTier = cyclopsSolarCharger;
            OnStartedPatching += () =>
            {
                if (!previousTier.IsPatched)
                    previousTier.Patch();
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder { get; } = "CyclopsSolarUpgrades/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };
        public override TechType RequiredForUnlock => previousTier.TechType;

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

        private SolarUpgradeHandler CreateSolarUpgrade(SubRoot cyclops)
        {
            return new SolarUpgradeHandler(previousTier.TechType, this.TechType, cyclops);
        }

        public UpgradeHandler CreateUpgradeHandler(SubRoot cyclops)
        {
            return CreateSolarUpgrade(cyclops);
        }

        public ICyclopsCharger CreateCyclopsCharger(SubRoot cyclops)
        {
            return new SolarCharger(previousTier.TechType, this.TechType, cyclops);
        }

        public IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new SolarIconOverlay(icon, upgradeModule);
        }
    }
}
