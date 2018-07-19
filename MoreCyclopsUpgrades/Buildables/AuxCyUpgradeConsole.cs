namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class AuxCyUpgradeConsole
    {
        public static TechType TechTypeID { get; private set; }

        public const string NameID = "AuxCyUpgradeConsole";
        public const string FriendlyName = "Auxiliary Upgrade Console";
        public const string HandOverText = "UseAuxConsole";
        public const string Description = "A secondary upgrade console to connect a greater number of upgrades to your Cyclops.";

        public static void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, false);
            KnownTechHandler.SetAnalysisTechEntry(TechType.CyclopsHullModule1, new TechType[1] { TechTypeID }, $"{FriendlyName} blueprint discovered!");
            LanguageHandler.SetLanguageLine(HandOverText, "Access Auxiliary Cyclops Upgrade Console");
            CraftDataHandler.AddBuildable(TechTypeID);
            CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, TechTypeID);

            PrefabHandler.RegisterPrefab(new AuxCyUpgradeConsolePreFab(NameID, TechTypeID));

            var recipe = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.AdvancedWiringKit, 1),
                                 new Ingredient(TechType.Titanium, 2),
                                 new Ingredient(TechType.Lead, 1),
                             })
            };

            CraftDataHandler.SetTechData(TechTypeID, recipe);
        }

        internal class AuxCyUpgradeConsolePreFab : ModPrefab
        {
            internal AuxCyUpgradeConsolePreFab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/Fabricator"));
                GameObject.DestroyImmediate(prefab.GetComponent<Fabricator>());

                var upConsole = prefab.AddComponent<AuxUpgradeConsole>();                

                var constructible = prefab.GetComponent<Constructable>();
                constructible.allowedInBase = false;
                constructible.allowedInSub = true;
                constructible.allowedOutside = false;
                constructible.allowedOnCeiling = false;
                constructible.allowedOnGround = false;
                constructible.allowedOnWall = true;
                constructible.allowedOnConstructables = false;
                constructible.controlModelState = false;
                constructible.rotationEnabled = false;
                constructible.techType = TechTypeID;

                return prefab;
            }
        }
    }
}
