namespace MoreCyclopsUpgrades.Modules.Recharging.Nuclear
{
    using Buildables;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

    internal class DepletedNuclearModule : CyclopsModule
    {
        internal const string DepletedNameID = "DepletedCyclopsNuclearModule";
        internal const string RefillNameID = "CyclopsNuclearModuleRefil";

        private const string DepletedEventKey = "CyNukeDepleted";
        public static string DepletedEvent => Language.main.Get(DepletedEventKey);

        internal NuclearFabricator NukFabricator { get; } = new NuclearFabricator();

        internal DepletedNuclearModule()
            : base(DepletedNameID,
                  "Depleted Cyclops Nuclear Reactor Module",
                  "Nuclear waste.",
                  CyclopsModule.NuclearChargerID,
                  TechType.DepletedReactorRod)
        {
        }

        protected override void Patch()
        {
            DepletedNuclearModuleID = TechTypeHandler.AddTechType(DepletedNameID, FriendlyName, Description, false);

            if (CyclopsModule.ModulesEnabled) // Even if the options have this be disabled,
            {// we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

                SpriteHandler.RegisterSprite(DepletedNuclearModuleID, $"./QMods/MoreCyclopsUpgrades/Assets/DepletedCyclopsNuclearModule.png");

                PrefabHandler.RegisterPrefab(this);

                LanguageHandler.SetLanguageLine(DepletedEventKey, "Nuclear Reactor Module depleted");
            }

            this.NukFabricator.Patch(CyclopsModule.ModulesEnabled);
        }

        protected override TechData GetRecipe()
        {
            return null;
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
        }
    }
}
