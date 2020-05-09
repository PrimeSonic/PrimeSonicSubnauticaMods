namespace CyclopsNuclearUpgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DepletedNuclearModule : Spawnable
    {
        private readonly CyclopsNuclearModule nuclearModule;

        private const string DepletedEventKey = "CyNukeDepleted";
        internal static string DepletedEventMsg => Language.main.Get(DepletedEventKey);

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

            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(DepletedEventKey, "Nuclear Reactor Module depleted");
            };
        }

        public override GameObject GetGameObject()
        {
            return GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.DepletedReactorRod));
        }
    }
}
