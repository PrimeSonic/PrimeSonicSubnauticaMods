namespace CustomCraft2SML.Fabricators
{
    using System;
    using System.Reflection;
    using Common;
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class CustomFabricatorBuildable : Buildable
    {
        protected readonly CustomFabricator FabricatorDetails;

        public CraftTree.Type TreeTypeID { get; private set; }

        public CustomFabricatorBuildable(CustomFabricator customFabricator)
            : base(customFabricator.ItemID, customFabricator.DisplayName, customFabricator.Tooltip)
        {
            FabricatorDetails = customFabricator;
            OnStartedPatching += PatchCustomTree;
        }

        public override TechGroup GroupForPDA { get; } = TechGroup.Miscellaneous;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Misc;
        public override string AssetsFolder { get; } = FileReaderWriter.RootModName + "/Assets";

        private void PatchCustomTree()
        {
            ModCraftTreeRoot rootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(this.ClassID, out CraftTree.Type craftType);

            this.TreeTypeID = craftType;


        }

        public override GameObject GetGameObject()
        {
            GameObject prefab;
            Constructable constructible = null;
            PrefabIdentifier prefabId = null;
            TechTag techTag = null;
            switch (FabricatorDetails.Model)
            {
                case ModelTypes.Fabricator:
                    prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Fabricator));
                    break;
                case ModelTypes.Workbench:
                    prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Workbench));
                    break;
                case ModelTypes.MoonPool:
                    prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator"));

                    // Add prefab ID
                    prefabId = prefab.AddComponent<PrefabIdentifier>();

                    // Add tech tag
                    techTag = prefab.AddComponent<TechTag>();

                    // Retrieve sub game objects
                    GameObject cyclopsFabLight = prefab.FindChild("fabricatorLight");
                    GameObject cyclopsFabModel = prefab.FindChild("submarine_fabricator_03");
                    // Translate CyclopsFabricator model and light
                    prefab.transform.localPosition = new Vector3(
                                                                cyclopsFabModel.transform.localPosition.x, // Same X position
                                                                cyclopsFabModel.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                cyclopsFabModel.transform.localPosition.z); // Same Z position
                    prefab.transform.localPosition = new Vector3(
                                                                cyclopsFabLight.transform.localPosition.x, // Same X position
                                                                cyclopsFabLight.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                cyclopsFabLight.transform.localPosition.z); // Same Z position
                    // Add constructable - This prefab normally isn't constructed.
                    constructible = prefab.AddComponent<Constructable>();
                    constructible.model = cyclopsFabModel;
                    break;
                default:
                    throw new InvalidOperationException("ModelType in CustomFabricator does not correspond to a valid fabricator type");
            }

            // Update prefab name
            prefab.name = this.ClassID;

            // Update prefab ID
            if (prefabId is null)
                prefabId = prefab.GetComponent<PrefabIdentifier>();

            if (prefabId != null)
            {
                prefabId.ClassId = this.ClassID;
                prefabId.name = this.FriendlyName;
            }

            // Update tech tag            
            if (techTag is null)
                techTag = prefab.GetComponent<TechTag>();

            if (techTag != null)
                techTag.type = this.TechType;

            // Associate custom craft tree to the fabricator
            Fabricator fabricator = prefab.GetComponent<Fabricator>();
            fabricator.craftTree = this.TreeTypeID;
            fabricator.handOverText = this.HandOverText;

            if (constructible is null)
                constructible = prefab.GetComponent<Constructable>();

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
            //Texture2D customTexture = ImageUtils.LoadTextureFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorT.png");
            //SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            //skinnedMeshRenderer.material.mainTexture = customTexture;

            // Associate power relay
            var powerRelay = new PowerRelay();

            // This is actually a dirty hack
            // The problem is that the parent SubRoot isn't correctly associated at this time.
            // The power relay should be getting set in the GhostCrafter Start() method.
            // But the parent components are coming up null.
            (fabricator as GhostCrafter).SetPrivateField("powerRelay", powerRelay, BindingFlags.Instance);

            return prefab;
        }


        protected override TechData GetBlueprintRecipe() => FabricatorDetails.CreateRecipeTechData();
    }
}
