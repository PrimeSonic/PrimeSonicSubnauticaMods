namespace MoreCyclopsUpgrades.AuxConsole
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class AuxCyUpgradeConsole : Buildable
    {
        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder { get; } = "MoreCyclopsUpgrades/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.CyclopsHullModule1;

        private const string OnHoverKey = "CyUpgradeOnHover";
        public static string OnHoverText => Language.main.Get(OnHoverKey);

        public AuxCyUpgradeConsole()
            : base(classId: "AuxCyUpgradeConsole",
                   friendlyName: "Auxiliary Upgrade Console",
                   description: "A secondary upgrade console to connect a greater number of upgrades to your Cyclops.")
        {
            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(OnHoverKey, "Use Auxiliary Cyclop Upgrade Console");
            };
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Titanium, 5),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Lead, 1),
                }
            };
        }

        public override GameObject GetGameObject()
        {
            // We'll use this for the actual model
            var consolePrefab = GameObject.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Debris/Wrecks/Decoration/submarine_engine_console_01_wide"));
            GameObject consoleWide = consolePrefab.FindChild("submarine_engine_console_01_wide");
            GameObject consoleModel = consoleWide.FindChild("console");

            // The LabTrashcan prefab was chosen because it is very similar in size, shape, and collision model to the upgrade console model
            var prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.LabTrashcan));

            prefab.FindChild("discovery_trashcan_01_d").SetActive(false); // Turn off this model
            GameObject.DestroyImmediate(prefab.GetComponent<Trashcan>()); // Don't need this
            GameObject.DestroyImmediate(prefab.GetComponent<StorageContainer>()); // Don't need this

            // Add the custom component
            AuxCyUpgradeConsoleMono auxConsole = prefab.AddComponent<AuxCyUpgradeConsoleMono>();

            // This is to tie the model to the prefab
            consoleModel.transform.SetParent(prefab.transform);
            consoleWide.SetActive(false);
            consolePrefab.SetActive(false);

            // Rotate to the correct orientation
            consoleModel.transform.rotation *= Quaternion.Euler(180f, 180f, 180f);

            // Update sky applier
            SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = consoleModel.GetComponentsInChildren<MeshRenderer>();
            skyApplier.anchorSky = Skies.Auto;

            Constructable constructible = prefab.GetComponent<Constructable>();

            constructible.allowedInBase = false;
            constructible.allowedInSub = true; // Only allowed in Cyclops
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = true; // Only on ground
            constructible.allowedOnWall = false;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = true;
            constructible.techType = this.TechType;
            constructible.model = consoleModel;

            return prefab;
        }
    }
}
