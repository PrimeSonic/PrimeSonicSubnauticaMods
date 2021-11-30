namespace CustomCraft2SML.Fabricators
{
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using System;
    using UnityEngine;
    using CustomFabricator = Serialization.Entries.CustomFabricator;

    internal class CustomFabricatorBuildable : ModPrefab
    {
        protected readonly CustomFabricator FabricatorDetails;

        public CustomFabricatorBuildable(CustomFabricator customFabricator)
            : base(customFabricator.ItemID, $"{customFabricator.ItemID}PreFan", customFabricator.TechType)
        {
            FabricatorDetails = customFabricator;
        }

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

                    // Add prefab ID because CyclopsFabricator normaly doesn't have one
                    PrefabIdentifier prefabId = prefab.AddComponent<PrefabIdentifier>();
                    prefabId.ClassId = FabricatorDetails.ItemID;
                    prefabId.name = FabricatorDetails.DisplayName;

                    // Add tech tag because CyclopsFabricator normaly doesn't have one
                    TechTag techTag = prefab.AddComponent<TechTag>();
                    techTag.type = this.TechType;

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
            crafter.handOverText = FabricatorDetails.DisplayName;

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

            SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            if (FabricatorDetails.HasColorValue)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();             
                skinnedMeshRenderer.material.color = FabricatorDetails.ColorTint; // Tint option available
            }

            // Associate power relay
            var powerRelay = new PowerRelay();

            // This is actually a dirty hack
            // The problem is that the parent SubRoot isn't correctly associated at this time.
            // The power relay should be getting set in the GhostCrafter Start() method.
            // But the parent components are coming up null.
            crafter.powerRelay = powerRelay;

            return prefab;
        }
    }
}
