namespace MoreCyclopsUpgrades.Buildables
{
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Monobehaviors;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class CyBioReactor : Buildable
    {
        internal const string CyBioReactorID = "CyBioReactor";

        private static readonly CyBioReactor main = new CyBioReactor();

        public static void PatchCyBioReactor(bool enableBioreactor)
        {
            main.Patch(enableBioreactor);
        }

        public static TechType TechTypeID { get; private set; } = TechType.UnusedOld;

        private const string OnHoverFormatKey = "CyBioOnHover";
        public static string OnHoverFormatString(int charge, float capacity, string stillProducing)
        {
            return Language.main.GetFormat(OnHoverFormatKey, charge, capacity, stillProducing);
        }

        private const string OverLimitKey = "CyBioOverLimit";
        public static string OverLimitString()
        {
            return Language.main.Get(OverLimitKey);
        }

        private const string StorageLabelKey = "CyBioReactorLabel";
        public static string StorageLabel => Language.main.Get(StorageLabelKey);

        public CyBioReactor()
            : base(CyBioReactorID, "Cyclops Bioreactor", "Composts organic matter into electrical energy.")
        {
            OnFinishedPatching += HandleLanguageLines;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder { get; } = "MoreCyclopsUpgrades/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.BaseBioReactor;

        public override GameObject GetGameObject()
        {
            SubRoot cyclops = Player.main.currentSub;
            if (cyclops != null)
            {
                var mgr = CyclopsManager.GetAllManagers(cyclops);

                if (mgr.BioReactors.Count >= mgr.ChargeManager.MaxBioReactors)
                {
                    ErrorMessage.AddMessage(OverLimitString());
                    return null;
                }
            }

            // Instantiate Fabricator object
            var prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.SpecimenAnalyzer));
            GameObject.DestroyImmediate(prefab.GetComponentInChildren<SpecimenAnalyzerBase>()); // Don't need this
            GameObject.DestroyImmediate(prefab.GetComponent<SpecimenAnalyzer>()); // Don't need this
            GameObject model = prefab.FindChild("model");

            const float modelScaling = 0.18f;
            model.transform.localScale -= new Vector3(modelScaling, modelScaling, modelScaling);

            // Update sky applier
            SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            // Modify existing constructable - This is just a modified SpecimenAnalyzer which already had a Constructible component.
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
            constructible.model = model;

            // Set the custom texture
            Texture2D customTexture = ImageUtils.LoadTextureFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/CyBioReactorT.png");
            SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = customTexture;

            CyBioReactorMono bioReactorComponent = prefab.AddComponent<CyBioReactorMono>(); // The component that makes the magic happen
            bioReactorComponent.Battery = prefab.AddComponent<Battery>();

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

        private void Patch(bool fullPatch)
        {
            if (fullPatch)
            {
                base.Patch();
            }
            else
            {
                this.TechType = TechTypeHandler.AddTechType(this.ClassID, this.FriendlyName, this.Description, false);
            }

            TechTypeID = this.TechType;
        }

        private void HandleLanguageLines()
        {
            LanguageHandler.SetLanguageLine(StorageLabelKey, "Cyclops Bioreactor Materials");
            LanguageHandler.SetLanguageLine(OnHoverFormatKey, "Use Cyclops Bioreactor {0}/{1}{2} ");
            LanguageHandler.SetLanguageLine(OverLimitKey, "Too many active Bioreactors.");            
        }
    }
}
