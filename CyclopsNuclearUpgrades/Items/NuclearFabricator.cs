namespace CyclopsNuclearUpgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class NuclearFabricator : Buildable
    {
        private const string NameID = "NuclearFabricator";
        private readonly CyclopsNuclearModule nuclearModule;

        public CraftTree.Type TreeTypeID { get; private set; }
        public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
        public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
        public override string AssetsFolder { get; } = "CyclopsNuclearUpgrades/Assets";
        public override TechType RequiredForUnlock { get; } = TechType.BaseNuclearReactor;

        private const string HandOverKey = "UseNukFabricator";

        internal NuclearFabricator(CyclopsNuclearModule module)
            : base(NameID,
                   "Nuclear Fabricator",
                   "A specialized fabricator for safe handling of radioactive energy sources.")
        {
            nuclearModule = module;

            OnStartedPatching += () =>
            {
                if (!nuclearModule.IsPatched)
                    nuclearModule.Patch();
            };

            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(HandOverKey, "Use Nuclear Fabricator");
                this.TreeTypeID = CreateCustomTree();
            };
        }

        private CraftTree.Type CreateCustomTree()
        {
            ModCraftTreeRoot rootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(NameID, out CraftTree.Type craftType);

            rootNode.AddCraftingNode(TechType.ReactorRod);
            rootNode.AddCraftingNode(nuclearModule.TechType);
            rootNode.AddModdedCraftingNode("RReactorRodDUMMY"); // Refill nuclear reactor rod
            rootNode.AddModdedCraftingNode("CyNukeUpgrade1"); // Cyclops Nuclear Reactor Enhancer Mk1
            rootNode.AddModdedCraftingNode("CyNukeUpgrade2"); // Cyclops Nuclear Reactor Enhancer Mk2

            return craftType;
        }

        public override GameObject GetGameObject()
        {
            // Instantiate Fabricator object
            var prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Fabricator));

            // Update prefab name
            prefab.name = NameID;

            // Add prefab ID
            PrefabIdentifier prefabId = prefab.GetComponent<PrefabIdentifier>();
            if (prefabId != null)
            {
                prefabId.ClassId = NameID;
                prefabId.name = this.FriendlyName;
            }

            // Add tech tag
            TechTag techTag = prefab.GetComponent<TechTag>();
            if (techTag != null)
            {
                techTag.type = this.TechType;
            }

            // Update sky applier
            SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            // Associate custom craft tree to the fabricator
            GhostCrafter fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = this.TreeTypeID;
            fabricator.handOverText = Language.main.Get(HandOverKey);

            // Modify existing constructable - This is just a modified Fabricator which already had a Constructible component.
            Constructable constructible = prefab.GetComponent<Constructable>();

            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = false;
            constructible.allowedOnWall = true;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = this.TechType; // This was necessary to correctly associate the recipe at building time

            // Set the custom texture
            Texture2D customTexture = ImageUtils.LoadTextureFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorT.png");
            SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = customTexture;

            // Associate power relay
            var powerRelay = new PowerRelay();

            // This is actually a dirty hack
            // The problem is that the parent SubRoot isn't correctly associated at this time.
            // The power relay should be getting set in the GhostCrafter Start() method.
            // But the parent components are coming up null.
            fabricator.powerRelay = powerRelay;

            return prefab;
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
