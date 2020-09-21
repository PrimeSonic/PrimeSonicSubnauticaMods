namespace CyclopsAutoZapper
{
    using System.IO;
    using System.Reflection;
    using CyclopsAutoZapper.Managers;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class CyclopsParasiteRemover : CyclopsUpgrade
    {
        public CyclopsParasiteRemover()
            : base("CyclopsAntiLarvaModule",
                   "Cyclops Auto Parasite Remover",
                   "Automatically pulses the Cyclops shield at low power to detach parasites.")
        {
            OnFinishedPatching += () =>
            {
                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                { return new UpgradeHandler(this.TechType, cyclops) { MaxCount = 1 }; });

                MCUServices.Register.PdaIconOverlay(this.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
                { return new AntiParasiteIconOverlay(icon, upgradeModule); });

                MCUServices.Register.AuxCyclopsManager<ShieldPulser>((SubRoot cyclops) =>
                { return new ShieldPulser(this.TechType, cyclops); });
            };
        }

        public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock => TechType.CyclopsShieldModule;
        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;
        public override string IconFileName => "CyclopsAntiParasite.png";

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.Titanium, 1),
                }
            };
        }
    }
}
