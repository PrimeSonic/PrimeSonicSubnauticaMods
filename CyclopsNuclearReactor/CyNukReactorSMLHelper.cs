namespace CyclopsNuclearReactor
{
    using CyclopsNuclearReactor.Helpers;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using System;
    using UnityEngine;

    internal class CyNukReactorSMLHelper : Buildable
    {
        private static readonly CyNukReactorSMLHelper main = new CyNukReactorSMLHelper();
        private static GameObject _cyNukReactorPrefab;
        private AssetBundle _assetBundle;

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
        public static string OnHoverText(int currentPower)
        {
            return Language.main.GetFormat(OnHoverKey, currentPower);
        }

        private const string OverLimitKey = "CyNukeOverLimit";
        public static string OverLimit()
        {
            return Language.main.Get(OverLimitKey);
        }

        public static TechType TechTypeID { get; private set; }

        public static void PatchSMLHelper()
        {

            if (!main.GetPrefabs())
            {
                throw new NullReferenceException("CyNukReactor has failed to retrieve the prefab from the asset bundle");
            }

            main.Patch();
        }

        public CyNukReactorSMLHelper() : base("CyNukReactor", "Cyclops Nuclear Reactor", "A nuclear reactor re-designed to fit and function inside the Cyclops.")
        {
            OnFinishedPatching += AdditionalPatching;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder { get; } = "CyclopsNuclearReactor/Assets";

        public override GameObject GetGameObject()
        {
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
            constructible.techType = this.TechType;
            constructible.model = consoleModel;

            //Add the prefabIdentifier
            prefab.AddComponent<PrefabIdentifier>().ClassId = this.ClassID;

            //Add the techType to this custom prefab
            TechTag techTag = prefab.AddComponent<TechTag>();
            techTag.type = this.TechType;

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

            AssetBundle assetBundle = AssetHelper.Asset("CyclopsNuclearReactor", "CyNukReactorbundle");

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
                    new Ingredient(TechType.Lead, 2),
                }
            };
        }

        private void AdditionalPatching()
        {
            TechTypeID = this.TechType;
            LanguageHandler.SetLanguageLine(EquipmentLabelKey, "Cyclops Nuclear Reactor Rods");
            LanguageHandler.SetLanguageLine(DepletedMessageKey, "A nuclear reactor rod has depleted in the Cyclops");
            LanguageHandler.SetLanguageLine(OnHoverKey, "Cyclops Nuclear Reactor\n {0} ");
            LanguageHandler.SetLanguageLine(OverLimitKey, "Disabled! Too many active reactors");

            // Map new slots
            foreach (string slot in CyNukeReactorMono.SlotNames)
                Equipment.slotMapping.Add(slot, EquipmentType.NuclearReactor);
        }
    }
}
