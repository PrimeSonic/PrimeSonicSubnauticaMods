namespace CustomCraft2SML.Fabricators
{
    using System;
    using System.Reflection;
    using Common;
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CustomFabricatorBuildable : Buildable
    {
        protected readonly CustomFabricator FabricatorDetails;

        public CustomFabricatorBuildable(CustomFabricator customFabricator)
            : base(customFabricator.ItemID, customFabricator.DisplayName, customFabricator.Tooltip)
        {
            FabricatorDetails = customFabricator;
            OnStartedPatching += FabricatorDetails.StartCustomCraftingTree;
            OnFinishedPatching += FabricatorDetails.FinishCustomCraftingTree;
        }

        public override TechGroup GroupForPDA => FabricatorDetails.PdaGroup;
        public override TechCategory CategoryForPDA => FabricatorDetails.PdaCategory;
        public override string AssetsFolder { get; } = FileLocations.RootModName + "/Assets";
        public override string IconFileName => $"{FabricatorDetails.Model}.png";
        public override string HandOverText => FabricatorDetails.HandOverText;

        public override GameObject GetGameObject()
        {
            GameObject prefab;
            Constructable constructible = null;
            GhostCrafter crafter;
            switch (FabricatorDetails.Model)
            {
                case ModelTypes.Fabricator:
                    prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Fabricator));
                    crafter = prefab.GetComponent<Fabricator>();
                    break;
                case ModelTypes.Workbench:
                    prefab = GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Workbench));
                    crafter = prefab.GetComponent<Workbench>();
                    break;
                case ModelTypes.MoonPool:
                    prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator"));
                    crafter = prefab.GetComponent<Fabricator>();

                    // Retrieve sub game objects
                    GameObject cyclopsFabLight = prefab.FindChild("fabricatorLight");
                    GameObject cyclopsFabModel = prefab.FindChild("submarine_fabricator_03");
                    // Translate CyclopsFabricator model and light
                    prefab.transform.localPosition = new Vector3(cyclopsFabModel.transform.localPosition.x, // Same X position
                                                                 cyclopsFabModel.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabModel.transform.localPosition.z); // Same Z position
                    prefab.transform.localPosition = new Vector3(cyclopsFabLight.transform.localPosition.x, // Same X position
                                                                 cyclopsFabLight.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabLight.transform.localPosition.z); // Same Z position
                    // Add constructable - This prefab normally isn't constructed.
                    constructible = prefab.AddComponent<Constructable>();
                    constructible.model = cyclopsFabModel;
                    break;
                default:
                    throw new InvalidOperationException("ModelType in CustomFabricator does not correspond to a valid fabricator type");
            }

            crafter.craftTree = FabricatorDetails.TreeTypeID;
            crafter.handOverText = FabricatorDetails.HandOverText;

            if (constructible is null)
                constructible = prefab.GetComponent<Constructable>();

            constructible.allowedInBase = FabricatorDetails.AllowedInBase;
            constructible.allowedInSub = FabricatorDetails.AllowedInCyclops;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = FabricatorDetails.Model == ModelTypes.Workbench;
            constructible.allowedOnWall = FabricatorDetails.Model != ModelTypes.Workbench;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = false;
            constructible.techType = this.TechType; // This was necessary to correctly associate the recipe at building time

            // TODO
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
            crafter.SetPrivateField("powerRelay", powerRelay, BindingFlags.Instance);

            return prefab;
        }

        protected override TechData GetBlueprintRecipe() => FabricatorDetails.CreateRecipeTechData();
    }
}
