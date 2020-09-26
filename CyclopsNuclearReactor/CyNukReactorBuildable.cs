namespace CyclopsNuclearReactor
{
    using System.IO;
    using System.Reflection;
    using System;
    using FCStudioHelpers;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CyNukReactorBuildable : Buildable
    {
        private static readonly CyNukReactorBuildable main = new CyNukReactorBuildable();
        private static GameObject _cyNukReactorPrefab;
        private AssetBundle _assetBundle;

        private const string StorageLabelKey = "CyNukeRodsLabel";
        public static string StorageLabel()
        {
            return Language.main.Get(StorageLabelKey);
        }

        private const string DepletedMessageKey = "CyNukeRodDepleted";
        public static string DepletedMessage()
        {
            return Language.main.Get(DepletedMessageKey);
        }

        private const string OnHoverPoweredKey = "CyNukeOnHover";
        public static string OnHoverPoweredText(string currentPower, int rods, int maxRods)
        {
            return Language.main.GetFormat<string, int, int>(OnHoverPoweredKey, currentPower, rods, maxRods);
        }

        private const string OnHoverNoPowerKey = "CyNukeHoverUnpowered";
        public static string OnHoverNoPowerText()
        {
            return Language.main.Get(OnHoverNoPowerKey);
        }

        private const string OverLimitKey = "CyNukeOverLimit";
        public static string OverLimitMessage()
        {
            return Language.main.Get(OverLimitKey);
        }

        private const string NoPowerKey = "CyNukeNoPower";
        public static string NoPoweMessage()
        {
            return Language.main.Get(NoPowerKey);
        }

        private const string CannotRemoveKey = "CyNukeNoDowngrade";
        internal static string CannotRemoveMsg()
        {
            return Language.main.Get(CannotRemoveKey);
        }

        private const string UpgradedMsgKey = "CyNukeUpgradedKey";
        public static string UpgradedMsg()
        {
            return Language.main.Get(UpgradedMsgKey);
        }

        private const string InactiveRodKey = "CyNukeInactiveRod";

        internal const string PowerIndicatorIconID = "CyNukReactorHUD";

        public static string InactiveRodMsg()
        {
            return Language.main.Get(InactiveRodKey);
        }

        public static TechType TechTypeID { get; private set; }

        public static void PatchSMLHelper()
        {
            if (!main.GetPrefabs())
            {
                MCUServices.Logger.Error("CyNukReactor has failed to retrieve the prefab from the asset bundle");
                throw new NullReferenceException("CyNukReactor has failed to retrieve the prefab from the asset bundle");
            }

            main.Patch();
        }

        public CyNukReactorBuildable() : base("CyNukReactor", "Cyclops Nuclear Reactor", "A nuclear reactor re-designed to fit and function inside the Cyclops.")
        {
            OnFinishedPatching += AdditionalPatching;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public override TechType RequiredForUnlock { get; } = TechType.BaseNuclearReactor;

        public override GameObject GetGameObject()
        {
            SubRoot cyclops = Player.main.currentSub;
            if (cyclops != null && cyclops.isCyclops)
            {
                CyNukeManager mgr = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(cyclops);

                if (mgr != null && mgr.TrackedBuildablesCount >= CyNukeChargeManager.MaxReactors)
                {
                    ErrorMessage.AddMessage(OverLimitMessage());
                    return null;
                }
            }

            var prefab = GameObject.Instantiate(_cyNukReactorPrefab);
            GameObject consoleModel = prefab.FindChild("model");

            // Update sky applier
            SkyApplier skyApplier = prefab.AddComponent<SkyApplier>();
            skyApplier.renderers = consoleModel.GetComponentsInChildren<MeshRenderer>();
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
            constructible.techType = TechTypeID;
            constructible.model = consoleModel;

            //Add the prefabIdentifier
            PrefabIdentifier prefabID = prefab.AddComponent<PrefabIdentifier>();
            prefabID.ClassId = main.ClassID;

            // Add the custom component
            CyNukeReactorMono auxConsole = prefab.AddComponent<CyNukeReactorMono>(); // Moved to the bottom to allow constructible to be added

            return prefab;
        }

        /// <summary>
        /// Applies the shader to the materials of the reactor
        /// </summary>
        /// <param name="prefab"></param>
        private void ApplyShaders(GameObject prefab)
        {
            #region SystemLights_BaseColor
            MaterialHelpers.ApplyEmissionShader("SystemLights_BaseColor", "SystemLights_OnMode_Emissive", prefab, _assetBundle, new Color(0.08235294f, 1f, 1f));
            MaterialHelpers.ApplyNormalShader("SystemLights_BaseColor", "SystemLights_Norm", prefab, _assetBundle);
            MaterialHelpers.ApplyAlphaShader("SystemLights_BaseColor", prefab);
            #endregion

            #region FCS_SUBMods_GlobalDecals
            MaterialHelpers.ApplyAlphaShader("FCS_SUBMods_GlobalDecals", prefab);
            #endregion

            #region NuclearPowerStorage
            MaterialHelpers.ApplyEmissionShader("NuclearPowerStorage", "NuclearPowerStorage_Emissive", prefab, _assetBundle, new Color(0.08235294f, 0.7686275f, 0.227451f));
            MaterialHelpers.ApplyNormalShader("NuclearPowerStorage", "NuclearPowerStorage_Norm", prefab, _assetBundle);
            #endregion

            #region Glass
            // MaterialHelpers.ApplyGlassShader("glass", prefab, 0.1f); //Temperately disabled until solution is found
            #endregion
        }

        /// <summary>
        /// Finds the prefab in the asset bundle
        /// </summary>
        /// <returns></returns>
        public bool GetPrefabs()
        {
            // == Get the prefab == //
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = Path.Combine(executingLocation, "Assets");
            string bundlePath = Path.Combine(folderPath, "CyNukReactorbundle");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);

            //If the result is null return false.
            if (assetBundle == null)
                return false;

            _assetBundle = assetBundle;

            //We have found the asset bundle and now we are going to continue by looking for the model.
            GameObject cyNukReactorPrefab = assetBundle.LoadAsset<GameObject>("CyNukReactor");

            //If the prefab isn't null lets add the shader to the materials
            if (cyNukReactorPrefab != null)
            {
                _cyNukReactorPrefab = cyNukReactorPrefab;

                //Lets apply the material shader
                ApplyShaders(_cyNukReactorPrefab);
            }
            else
            {
                return false;
            }

            return true;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                Ingredients =
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Lead, 3),
                }
            };
        }

        private void AdditionalPatching()
        {
            TechTypeID = this.TechType;
            LanguageHandler.SetLanguageLine(StorageLabelKey, "Cyclops Nuclear Reactor Rods");
            LanguageHandler.SetLanguageLine(DepletedMessageKey, "A nuclear reactor rod has depleted in the Cyclops");
            LanguageHandler.SetLanguageLine(OnHoverPoweredKey, "Cyclops Nuclear Reactor\n {0} ({1}/{2})");
            LanguageHandler.SetLanguageLine(OnHoverNoPowerKey, "Cyclops Nuclear Reactor\nNo Power");
            LanguageHandler.SetLanguageLine(OverLimitKey, "Too many active nuclear reactors");
            LanguageHandler.SetLanguageLine(NoPowerKey, "No Power");
            LanguageHandler.SetLanguageLine(UpgradedMsgKey, "Cyclops nuclear reactor has been enhanced");
            LanguageHandler.SetLanguageLine(CannotRemoveKey, "Too many active rods in nuclear reactor to remove enhancement");
            LanguageHandler.SetLanguageLine(InactiveRodKey, "Inactive");

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetsFolder = Path.Combine(executingLocation, "Assets");
            string filePath = Path.Combine(assetsFolder, "CyNukReactorHUD.png");

            SpriteHandler.RegisterSprite(SpriteManager.Group.Category, PowerIndicatorIconID, filePath);
        }
    }
}
