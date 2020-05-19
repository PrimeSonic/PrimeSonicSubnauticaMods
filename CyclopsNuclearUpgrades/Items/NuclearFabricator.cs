namespace CyclopsNuclearUpgrades
{
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class NuclearFabricator : CustomFabricator
    {
        private readonly CyclopsNuclearModule nuclearModule;

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;

        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        public override TechType RequiredForUnlock => TechType.BaseNuclearReactor;

        public override Models Model => Models.Custom; // Using Custom so the texture can be altered

        private static Texture2D customTexture;
        private static Atlas.Sprite sprite;

        internal NuclearFabricator(CyclopsNuclearModule module)
            : base("NuclearFabricator",
                   "Nuclear Fabricator",
                   "A specialized fabricator for safe handling of radioactive energy sources.")
        {
            nuclearModule = module;

            OnStartedPatching += () =>
            {
                if (!nuclearModule.IsPatched)
                    nuclearModule.Patch();

                // Load the custom texture
                string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string folderPath = Path.Combine(executingLocation, "Assets");

                string textureLocation = Path.Combine(folderPath, "NuclearFabricatorT.png");
                customTexture = ImageUtils.LoadTextureFromFile(textureLocation);

                string spriteLocation = Path.Combine(folderPath, "NuclearFabricator.png");
                sprite = ImageUtils.LoadSpriteFromFile(spriteLocation) ?? SpriteManager.Get(TechType.Fabricator);
            };
        }

        protected override GameObject GetCustomCrafterPreFab()
        {
            // Instantiate Fabricator object
            var gObj = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Fabricator));

            // Set the custom texture
            if (customTexture != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material.mainTexture = customTexture;
            }

            // Change size
            Vector3 scale = gObj.transform.localScale;
            const float factor = 0.90f;
            gObj.transform.localScale = new Vector3(scale.x * factor, scale.y * factor, scale.z * factor);

            return gObj;
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return sprite;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Lead, 2),
                }
            };
        }
    }
}
