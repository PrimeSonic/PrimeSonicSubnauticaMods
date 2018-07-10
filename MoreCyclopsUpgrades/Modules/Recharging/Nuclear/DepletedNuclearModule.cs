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
                  TechType.ReactorRod)
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

            SpriteHandler.RegisterSprite(TechTypeID, $"./QMods/MoreCyclopsUpgrades/Assets/{DepletedNameID}.png");
            SpriteHandler.RegisterSprite(RefillNuclearModuleID, $"./QMods/MoreCyclopsUpgrades/Assets/CyclopsNuclearModule.png");

            CraftDataHandler.SetTechData(RefillNuclearModuleID, GetRecipe());
            KnownTechHandler.SetAnalysisTechEntry(NuclearChargerID, new TechType[1] { RefillNuclearModuleID }, "Reload of cyclops nuclear module available.");

            //CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, dummy, "CyclopsMenu");

            PrefabHandler.RegisterPrefab(new DepletedNuclearModulePreFab(DepletedNameID, TechTypeID));

            SetStaticTechTypeID(TechTypeID);
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
