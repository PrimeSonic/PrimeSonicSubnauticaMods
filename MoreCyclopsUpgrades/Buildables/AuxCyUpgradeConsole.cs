namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
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
                GameObject consolePrefab = GameObject.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Debris/Wrecks/Decoration/submarine_engine_console_01_wide"));
                GameObject consoleWide = consolePrefab.FindChild("submarine_engine_console_01_wide");
                GameObject consoleModel = consoleWide.FindChild("console");

                GameObject prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Workbench));
                prefab.FindChild("model").SetActive(false); // Turn off the model
                GameObject.DestroyImmediate(prefab.GetComponent<Workbench>());

                var auxConsole = prefab.AddComponent<AuxUpgradeConsole>();
                
                consoleModel.transform.parent = auxConsole.transform;

                //auxConsole.Module1 = consoleWide.FindChild("engine_console_key_01_01");
                //auxConsole.Module2 = consoleWide.FindChild("engine_console_key_01_02");
                //auxConsole.Module3 = consoleWide.FindChild("engine_console_key_01_03");
                //auxConsole.Module4 = consoleWide.FindChild("engine_console_key_01_04");
                //auxConsole.Module5 = consoleWide.FindChild("engine_console_key_01_05");
                //auxConsole.Module6 = consoleWide.FindChild("engine_console_key_01_06");

                // Rotate to the correct orientation
                consoleModel.transform.rotation *= Quaternion.Euler(180f, 180f, 180f);

                // Update sky applier
                var skyApplier = consolePrefab.GetComponent<SkyApplier>();
                skyApplier.renderers = consolePrefab.GetComponentsInChildren<Renderer>();
                skyApplier.anchorSky = Skies.Auto;

                var constructible = prefab.GetComponent<Constructable>();
                constructible.allowedInBase = false;
                constructible.allowedInSub = true; // Only allowed in Cyclops
                constructible.allowedOutside = false;
                constructible.allowedOnCeiling = false;
                constructible.allowedOnGround = true; // Allowed on floor
                constructible.allowedOnWall = true; // Allowed on walls
                constructible.allowedOnConstructables = false;
                constructible.controlModelState = true;
                constructible.rotationEnabled = false;
                constructible.techType = TechTypeID;
                constructible.model = consoleModel;

                constructible.transform.parent = auxConsole.transform;

                return prefab;
            }
        }
    }
}
