namespace CyclopsNuclearReactor
{
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyNukeEnhancerMk2 : Craftable
    {
        private static readonly CyNukeEnhancerMk2 main = new CyNukeEnhancerMk2();

        public CyNukeEnhancerMk2() 
            : base("CyNukeUpgrade2", "Cyclops Nuclear Enhancer Mk2", "Greatly increases the capacity of all nuclear reactors aboard the cyclops.\nDoes not stack with other similar upgrades..")
        {
            OnFinishedPatching += AdditionalPatching;
        }

        public static TechType TechTypeID { get; private set; }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override TechGroup GroupForPDA { get; } = TechGroup.Cyclops;
        public override TechCategory CategoryForPDA { get; } = TechCategory.CyclopsUpgrades;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock { get; } = TechType.Workbench;
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        public static void PatchSMLHelper()
        {
            main.Patch();
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsShieldModule);
            yield return task;
            GameObject prefab = task.GetResult();
            GameObject obj = Object.Instantiate(prefab);

            gameObject.Set(obj);
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(CyNukeEnhancerMk1.TechTypeID, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.Aerogel, 1),
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
