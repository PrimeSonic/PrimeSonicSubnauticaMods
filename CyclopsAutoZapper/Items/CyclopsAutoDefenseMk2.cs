namespace CyclopsAutoZapper
{
    using System.IO;
    using System.Reflection;
    using CyclopsAutoZapper.Managers;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using SMLHelper.V2.Crafting;

    internal class CyclopsAutoDefenseMk2 : CyclopsUpgrade
    {
        private readonly TechType autoDefenseMk1;

        public CyclopsAutoDefenseMk2(CyclopsAutoDefense zapperMk1)
            : base("CyclopsZapperModuleMk2",
                   "Cyclops Auto Defense System Mk2",
                   "Self contained, automated, anti-predator electrical defense system for the Cyclops.")
        {
            autoDefenseMk1 = zapperMk1.TechType;
            OnFinishedPatching += () =>
            {
                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                { return new UpgradeHandler(this.TechType, cyclops) { MaxCount = 1 }; });

                MCUServices.Register.PdaIconOverlay(this.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
                { return new AutoDefenseMk2IconOverlay(icon, upgradeModule); });

                MCUServices.Register.AuxCyclopsManager<AutoDefenserMk2>((SubRoot cyclops) =>
                { return new AutoDefenserMk2(this.TechType, cyclops); });
            };
        }

        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock => autoDefenseMk1;
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(autoDefenseMk1, 1),
                    new Ingredient(TechType.SeamothElectricalDefense, 1),
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.Magnetite, 1)
                }
            };
        }
    }
}
