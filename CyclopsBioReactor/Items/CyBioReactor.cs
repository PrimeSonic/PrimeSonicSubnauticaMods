namespace CyclopsBioReactor.Items
{
    using System.IO;
    using System.Reflection;
    using CyclopsBioReactor.Management;
    using FCStudioHelpers;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyBioReactor : Buildable
    {
        private const string OverLimitKey = "CyBioOverLimit";
        public static string OverLimitString => Language.main.Get(OverLimitKey);

        private const string StorageLabelKey = "CyBioReactorLabel";
        public static string StorageLabel => Language.main.Get(StorageLabelKey);

        private const string ChargingKey = "CyBioCharging";
        public static string ChargingCyclopsText => Language.main.Get(ChargingKey);


        private GameObject _prefab;

        private AssetBundle _assetBundle;

        private readonly BioReactorBooster bioBooster;

        public CyBioReactor(BioReactorBooster booster)
            : base("CyBioReactor", "Cyclops Bioreactor", "Composts organic matter into electrical energy.")
        {
            bioBooster = booster;

            if (!GetPrefabs())
            {
                MCUServices.Logger.Error("Error in asset bundle!");
            }

            OnStartedPatching += () =>
            {
                if (!bioBooster.IsPatched)
                    bioBooster.Patch();
            };

            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(StorageLabelKey, "Cyclops Bioreactor Materials");
                LanguageHandler.SetLanguageLine(OverLimitKey, "Too many active Bioreactors.");
                LanguageHandler.SetLanguageLine(ChargingKey, "Charging Sub");
            };

        }

        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock { get; } = TechType.BaseBioReactor;

        public override GameObject GetGameObject()
        {
            SubRoot cyclops = Player.main.currentSub;
            if (cyclops != null && cyclops.isCyclops)
            {
                BioAuxCyclopsManager mgr = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(cyclops);

                if (mgr != null && mgr.TrackedBuildablesCount >= BioChargeHandler.MaxBioReactors)
                {
                    ErrorMessage.AddMessage(OverLimitString);
                    return null;
                }
            }

            if (_prefab == null)
            {
                MCUServices.Logger.Error("_prefab is null", true);
            }

            var prefab = GameObject.Instantiate(_prefab);

            if (prefab == null)
            {
                MCUServices.Logger.Error("Prefab is null", true);
            }

            GameObject model = prefab.FindChild("model");

            // Update sky applier
            SkyApplier skyApplier = prefab.AddComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            //Add the constructible component to the prefab
            Constructable constructible = prefab.AddComponent<Constructable>();

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
            constructible.model = model;

            prefab.AddComponent<PrefabIdentifier>().ClassId = this.ClassID;

            prefab.AddComponent<CyBioReactorMono>(); // The component that makes the magic happen

            return prefab;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Glass, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.Lubricant, 1),
                }
            };
        }
        public bool GetPrefabs()
        {
            MCUServices.Logger.Debug("GetPrefabs");

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = Path.Combine(executingLocation, "Assets");
            string bundlePath = Path.Combine(folderPath, "cyclopsbioreactormodbundle");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);

            //If the result is null return false.
            if (assetBundle == null)
            {
                MCUServices.Logger.Error($"AssetBundle is Null!");
                return false;
            }

            _assetBundle = assetBundle;

            MCUServices.Logger.Debug($"AssetBundle Set");

            //We have found the asset bundle and now we are going to continue by looking for the model.
            GameObject prefab = assetBundle.LoadAsset<GameObject>("CyclopsBioreactor");

            //If the prefab isn't null lets add the shader to the materials
            if (prefab != null)
            {
                _prefab = prefab;

                //Lets apply the material shader
                ApplyShaders(_prefab);

                MCUServices.Logger.Debug($"{this.FriendlyName} Prefab Found!");
            }
            else
            {
                MCUServices.Logger.Error($"{this.FriendlyName} Prefab Not Found!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies the shader to the materials of the reactor
        /// </summary>
        /// <param name="prefab">The prefab to apply shaders.</param>
        private void ApplyShaders(GameObject prefab)
        {
            #region Bioreactor_Base
            MaterialHelpers.ApplyNormalShader("Bioreactor_Base", "Bioreactor_Bioreactor_Normal", prefab, _assetBundle);
            MaterialHelpers.ApplyEmissionShader("Bioreactor_Base", "Bioreactor_Bioreactor_Emissive", prefab, _assetBundle, new Color(0.08235294f, 1f, 1f));
            MaterialHelpers.ApplyAlphaShader("Bioreactor_Base", prefab);
            MaterialHelpers.ApplySpecShader("Bioreactor_Base", "Bioreactor_Bioreactor_SpecularGloss", prefab, 1, 6f, _assetBundle);
            #endregion
        }
    }
}
