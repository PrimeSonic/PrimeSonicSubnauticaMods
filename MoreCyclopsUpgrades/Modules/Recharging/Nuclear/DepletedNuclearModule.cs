namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DepletedNuclearModule : CyclopsModule
    {
        internal const string DepletedNameID = "DepletedCyclopsNuclearModule";

        internal DepletedNuclearModule()
            : base(DepletedNameID,
                  "Depleted Cyclops Nuclear Reactor Module",
                  "Bring to a specialized fabricator for safe extraction of the depleted reactor rod inside.",
                  CyclopsModule.NuclearChargerID,
                  TechType.DepletedReactorRod)
        {

        }

        public override ModuleTypes ModuleID => ModuleTypes.DepletedNuclear;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod);
            GameObject gameObject = GameObject.Instantiate(prefab);

            return gameObject;
        }

        protected override void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(DepletedNameID, FriendlyName, Description, false);

            RefillNuclearModuleID = TechTypeHandler.AddTechType("CyclopsNuclearModuleRefil",
                                                                 "Reload Cyclops Nuclear Module",
                                                                 "Reload a Depleted Cyclops Nuclear Module with a Reactor Rod",
                                                                 false);

            if (CyclopsModule.ModulesEnabled) // Even if the options have this be disabled,                
            {// we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

                SpriteHandler.RegisterSprite(this.TechType, $"./QMods/MoreCyclopsUpgrades/Assets/DepletedCyclopsNuclearModule.png");
                SpriteHandler.RegisterSprite(RefillNuclearModuleID, $"./QMods/MoreCyclopsUpgrades/Assets/CyclopsNuclearModule.png");

                CraftDataHandler.SetTechData(RefillNuclearModuleID, GetRecipe());
                KnownTechHandler.SetAnalysisTechEntry(TechType.BaseNuclearReactor, new TechType[1] { RefillNuclearModuleID }, "Reload of cyclops nuclear module available.");

                PrefabHandler.RegisterPrefab(this);

                SetStaticTechTypeID(this.TechType);
            }
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 0,
                Ingredients = new List<Ingredient>()
                    {
                        new Ingredient(this.TechType, 1),
                        new Ingredient(TechType.ReactorRod, 1)
                    },
                LinkedItems = new List<TechType>()
                    {
                        NuclearChargerID,
                        TechType.DepletedReactorRod
                    }
            };
        }

        protected override void SetStaticTechTypeID(TechType techTypeID)
        {
            DepletedNuclearModuleID = techTypeID;
        }
    }
}
