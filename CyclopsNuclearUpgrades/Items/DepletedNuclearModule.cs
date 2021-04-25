namespace CyclopsNuclearUpgrades
{
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class DepletedNuclearModule : Spawnable
    {
        private readonly CyclopsNuclearModule nuclearModule;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public DepletedNuclearModule(CyclopsNuclearModule module)
            : base("DepletedCyclopsNuclearModule",
                   "Depleted Cyclops Nuclear Reactor Module",
                   "Nuclear waste.")
        {
            nuclearModule = module;

            OnStartedPatching += () =>
            {
                if (!nuclearModule.IsPatched)
                    nuclearModule.Patch();
            };
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DepletedReactorRod);
            yield return task;
            GameObject prefab = task.GetResult();
            GameObject obj = Object.Instantiate(prefab);

            gameObject.Set(obj);
        }
    }
}
