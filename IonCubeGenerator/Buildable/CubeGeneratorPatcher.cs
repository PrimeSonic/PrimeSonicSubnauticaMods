namespace IonCubeGenerator.Buildable
{
    using System.IO;
    using System.Reflection;
    using Common;
    using IonCubeGenerator.Craftables;
    using IonCubeGenerator.Display;
    using IonCubeGenerator.Display.Patching;
    using IonCubeGenerator.Mono;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal partial class CubeGeneratorBuildable : Buildable
    {
        private static readonly CubeGeneratorBuildable singleton = new CubeGeneratorBuildable();

        public static void PatchSMLHelper()
        {
            if (!singleton.GetPrefabs())
            {
                throw new FileNotFoundException("Failed to retrieve the IonCubeGenerator prefab from the asset bundle");
            }

            var alienIngot = new AlienIngot();

            if (!alienIngot.GetPrefabs(singleton._assetBundle))
            {
                throw new FileNotFoundException("Failed to retrieve the Alien Ingot Case prefab from the asset bundle");
            }

            alienIngot.Patch();

            var alienCase = new AlienEletronicsCase();

            if (!alienCase.GetPrefabs(singleton._assetBundle))
            {
                throw new FileNotFoundException("Failed to retrieve the Alien Eletronics Case prefab from the asset bundle");
            }

            alienCase.Patch();

            singleton.Patch();

        }

        public CubeGeneratorBuildable()
            : base("CubeGenerator", "Ion Cube Generator", "Condenses large quantities of power into Ion Cubes")
        {
            OnFinishedPatching += AdditionalPatching;
            OnFinishedPatching += DisplayLanguagePatching.AdditionPatching;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorPieces;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorPiece;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
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

            CreateDisplayedIonCube(prefab);

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

            CubeGeneratorAnimator cubeGeneratorAnimator = prefab.AddComponent<CubeGeneratorAnimator>();

            IonGeneratorDisplay cubeGeneratorDisplay = prefab.AddComponent<IonGeneratorDisplay>();

            FMOD_CustomLoopingEmitter FMOD_CustomLoopingEmitter = prefab.AddComponent<FMOD_CustomLoopingEmitter>();

            return prefab;
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData
            {
                Ingredients =
                {
                    new Ingredient(AlienIngot.TechTypeID, 2),
                    new Ingredient(AlienEletronicsCase.TechTypeID, 1),

                    new Ingredient(TechType.Glass, 1),
                    new Ingredient(TechType.Lubricant, 1),
                    new Ingredient(TechType.Kyanite, 2),
                }
            };
        }

        private void CreateDisplayedIonCube(GameObject prefab)
        {
            GameObject ionSlot = prefab.FindChild("model")
                .FindChild("Platform_Lifter")
                .FindChild("Ion_Lifter")
                .FindChild("IonCube")
                .FindChild("precursor_crystal")?.gameObject;

            if (ionSlot != null)
            {
                QuickLogger.Debug("Ion Cube Display Object Created", true);
                var displayedIonCube = GameObject.Instantiate<GameObject>(CubeGeneratorContainer.CubePrefab);
                Pickupable pickupable = displayedIonCube.GetComponent<Pickupable>();
                GameObject.Destroy(pickupable);
                Collider collider = displayedIonCube.GetComponent<Collider>();
                GameObject.Destroy(collider);
                Rigidbody rigidbody = displayedIonCube.GetComponent<Rigidbody>();
                GameObject.Destroy(rigidbody);
                WorldForces worldForces = displayedIonCube.GetComponent<WorldForces>();
                GameObject.Destroy(worldForces);
                TechTag techTag = displayedIonCube.GetComponent<TechTag>();
                GameObject.Destroy(techTag);

                displayedIonCube.transform.SetParent(ionSlot.transform);
                displayedIonCube.transform.localPosition =
                    new Vector3(-0.1152f, 0.05f, 0f); // Is to high maybe the axis is flipped
                displayedIonCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                displayedIonCube.transform.Rotate(new Vector3(0, 0, 90));
            }
            else
            {
                QuickLogger.Error("Cannot Find IonCube in the prefab");
            }
        }

    }
}
