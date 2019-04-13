namespace CyclopsNuclearReactor
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyNukReactorSMLHelper : Buildable
    {
        private static readonly CyNukReactorSMLHelper main = new CyNukReactorSMLHelper();

        private const string EquipmentLabelKey = "CyNukeRodsLabel";
        public static string EquipmentLabel()
        {
            return Language.main.Get(EquipmentLabelKey);
        }

        private const string DepletedMessageKey = "CyNukeRodDepleted";
        public static string DepletedMessage()
        {
            return Language.main.Get(DepletedMessageKey);
        }

        private const string OnHoverKey = "CyNukeOnHover";
        public static string OnHoverText()
        {
            return Language.main.Get(OnHoverKey);
        }

        public static TechType TechTypeID { get; private set; }

        public static void PatchSMLHelper()
        {
            main.Patch();
        }

        public CyNukReactorSMLHelper() : base("CyNukReactor", "Cyclops Nuclear Reactor", "A nuclear reactor re-designed to fit and function inside the Cyclops.")
        {
            OnFinishedPatching += AdditionalPatching;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.Cyclops;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Cyclops;

        // TODO This needs an icon sprite
        public override string AssetsFolder { get; } = "CyclopsNuclearReactor/Assets";

        public override GameObject GetGameObject()
        {
            // TODO - Replace this with the actual model we need.
            var consolePrefab = GameObject.Instantiate(Resources.Load<GameObject>("WorldEntities/Doodads/Debris/Wrecks/Decoration/submarine_engine_console_01_wide"));
            GameObject consoleWide = consolePrefab.FindChild("submarine_engine_console_01_wide");
            GameObject consoleModel = consoleWide.FindChild("console");

            // The LabTrashcan prefab was chosen because it is very similar in size, shape, and collision model to the upgrade console model
            var prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.LabTrashcan));

            prefab.FindChild("discovery_trashcan_01_d").SetActive(false); // Turn off this model
            GameObject.DestroyImmediate(prefab.GetComponent<Trashcan>()); // Don't need this
            GameObject.DestroyImmediate(prefab.GetComponent<StorageContainer>()); // Don't need this

            // Add the custom component


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

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Lead, 2),
                }
            };
        }

        private void AdditionalPatching()
        {
            TechTypeID = this.TechType;
            LanguageHandler.SetLanguageLine(EquipmentLabelKey, "Cyclops Nuclear Reactor Rods");
            LanguageHandler.SetLanguageLine(DepletedMessageKey, "A nuclear reactor rod has depleted in the Cyclops");
            LanguageHandler.SetLanguageLine(OnHoverKey, "Cyclops Nuclear Reactor");
        }
    }
}
