namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
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

        public override CyclopsModules ModuleID => CyclopsModules.DepletedNuclear;

        public override void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(DepletedNameID, FriendlyName, Description, false);

            RefillNuclearModuleID = TechTypeHandler.AddTechType("CyclopsNuclearModuleRefil",
                                                                 "Reload Cyclops Nuclear Module",
                                                                 "Reload a Depleted Cyclops Nuclear Module with a Reactor Rod",
                                                                 false);

            if (CyclopsModule.ModulesEnabled) // Even if the options have this be disabled,                
            {// we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

                SpriteHandler.RegisterSprite(TechTypeID, $"./QMods/MoreCyclopsUpgrades/Assets/DepletedCyclopsNuclearModule.png");
                SpriteHandler.RegisterSprite(RefillNuclearModuleID, $"./QMods/MoreCyclopsUpgrades/Assets/CyclopsNuclearModule.png");

                CraftDataHandler.SetTechData(RefillNuclearModuleID, GetRecipe());
                KnownTechHandler.SetAnalysisTechEntry(TechType.BaseNuclearReactor, new TechType[1] { RefillNuclearModuleID }, "Reload of cyclops nuclear module available.");

                PrefabHandler.RegisterPrefab(new DepletedNuclearModulePreFab(DepletedNameID, TechTypeID));

                SetStaticTechTypeID(TechTypeID);
            }

            NuclearFabricator.Patch();
        }

        protected override ModPrefab GetPrefab()
        {
            return new DepletedNuclearModulePreFab(DepletedNameID, TechTypeID);
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 0,
                Ingredients = new List<Ingredient>()
                    {
                        new Ingredient(TechTypeID, 1),
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

        internal class DepletedNuclearModulePreFab : ModPrefab
        {
            internal DepletedNuclearModulePreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = Resources.Load<GameObject>("WorldEntities/Natural/DepletedReactorRod");
                GameObject gameObject = GameObject.Instantiate(prefab);

                return gameObject;
            }
        }
    }
}
