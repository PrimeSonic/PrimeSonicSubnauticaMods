namespace CyclopsNuclearReactor
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using System.IO;
    using System.Reflection;
    using UnityEngine;

    internal class CyNukeEnhancerMk1 : Craftable
    {
        private static readonly CyNukeEnhancerMk1 main = new CyNukeEnhancerMk1();

        public CyNukeEnhancerMk1() 
            : base("CyNukeUpgrade1", "Cyclops Nuclear Enhancer Mk1", "Increases the capacity of all nuclear reactors aboard the cyclops.\nDoes not stack with other similar upgrades.")
        {
            OnFinishedPatching += AdditionalPatching;
        }

        public static TechType TechTypeID { get; private set; }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Cyclops;
        public override TechCategory CategoryForPDA { get; } = TechCategory.CyclopsUpgrades;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock { get; } = TechType.Cyclops;
        public override string[] StepsToFabricatorTab
        {
            get
            {
                if (Directory.Exists(@"./QMods/VehicleUpgradesInCyclops"))
                {
                    return new[] { "CyclopsModules" };
                }

                return new string[0];
            }
        }

        public static void PatchSMLHelper()
        {            
            main.Patch();
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsShieldModule);

            return GameObject.Instantiate(prefab);
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.Benzene, 1),
                    new Ingredient(TechType.Lead, 1),
                }
            };
        }

        private void AdditionalPatching()
        {
            TechTypeID = this.TechType;
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.CyclopsModule);
        }
    }
}
