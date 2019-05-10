namespace IonCubeGenerator.Buildable
{
    using IonCubeGenerator.Display.Patching;
    using IonCubeGenerator.Mono;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using System.IO;
    using UnityEngine;

    internal partial class CubeGeneratorBuildable : Buildable
    {
        private static readonly CubeGeneratorBuildable singleton = new CubeGeneratorBuildable();

        public static void PatchSMLHelper()
        {
            if (!singleton.GetPrefabs())
            {
                throw new FileNotFoundException("Failed to retrieve the IonCubeGenerator prefab from the asset bundle");
            }

            singleton.Patch();
        }

        public CubeGeneratorBuildable()
            : base("CubeGenerator", "Ion Cube Generator", "Condenses large quantities of power into Ion Cubes")
        {
            OnFinishedPatching += AdditionalPatching;
            OnFinishedPatching += DisplayLanguagePatching.AdditionPatching;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.BasePieces;
        public override TechCategory CategoryForPDA { get; } = TechCategory.BasePiece;
        public override string AssetsFolder { get; } = "IonCubeGenerator/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorPrisonIonGenerator;

        public override GameObject GetGameObject()
        {
            var prefab = GameObject.Instantiate(_ionCubeGenPrefab);
            GameObject consoleModel = prefab.FindChild("model");

            // Update sky applier
            SkyApplier skyApplier = prefab.AddComponent<SkyApplier>();
            skyApplier.renderers = consoleModel.GetComponentsInChildren<MeshRenderer>();
            skyApplier.anchorSky = Skies.Auto;

            //Add the constructible component to the prefab
            Constructable constructible = prefab.AddComponent<Constructable>();

            constructible.allowedInBase = true; // Only allowed in Base
            constructible.allowedInSub = false; // Not allowed in Cyclops
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = true; // Only on ground
            constructible.allowedOnWall = false;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = true;
            constructible.techType = this.TechType;
            constructible.model = consoleModel;

            //Add the prefabIdentifier
            PrefabIdentifier prefabID = prefab.AddComponent<PrefabIdentifier>();
            prefabID.ClassId = this.ClassID;

            // Add the custom component
            CubeGeneratorMono cubeGenerator = prefab.AddComponent<CubeGeneratorMono>(); // Moved to the bottom to allow constructible to be added

            return prefab;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PrecursorIonCrystal, 4),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Benzene, 2),

                    new Ingredient(TechType.PlasteelIngot, 2),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.Magnetite, 2),

                    new Ingredient(TechType.PrecursorKey_Blue, 2),
                    new Ingredient(TechType.PrecursorKey_Purple, 2),
                    new Ingredient(TechType.PrecursorKey_Orange, 2),
                }
            };
        }
    }
}
