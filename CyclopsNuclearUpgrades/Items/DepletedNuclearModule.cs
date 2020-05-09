namespace CyclopsNuclearUpgrades
{
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class DepletedNuclearModule : Spawnable
    {
        private readonly CyclopsNuclearModule nuclearModule;

        public override string AssetsFolder { get; } = "CyclopsNuclearUpgrades/Assets";

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

        public override GameObject GetGameObject()
        {
            return GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.DepletedReactorRod));
        }
    }
}
