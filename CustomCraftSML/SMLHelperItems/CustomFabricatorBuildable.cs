namespace CustomCraft2SML.Fabricators
{
    using Common;
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using System;
    using System.Collections;
    using UnityEngine;
    using UWE;
    using CustomFabricator = Serialization.Entries.CustomFabricator;

    internal class CustomFabricatorBuildable : ModPrefab
    {
        protected readonly CustomFabricator FabricatorDetails;

        public CustomFabricatorBuildable(CustomFabricator customFabricator)
            : base(customFabricator.ItemID, $"{customFabricator.ItemID}PreFan", customFabricator.TechType)
        {
            FabricatorDetails = customFabricator;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject obj;
            Constructable constructible = null;
            GhostCrafter crafter;
            switch (FabricatorDetails.Model)
            {
                case ModelTypes.Fabricator:
                {
                    var task = CraftData.GetPrefabForTechTypeAsync(TechType.Fabricator);
                    yield return task;
                    var prefab = task.GetResult();
                    obj = GameObject.Instantiate(prefab);
                    crafter = obj.GetComponent<Fabricator>();
                }
                break;
                case ModelTypes.Workbench:
                {
                    var task = CraftData.GetPrefabForTechTypeAsync(TechType.Workbench);
                    yield return task;
                    var prefab = task.GetResult();
                    obj = GameObject.Instantiate(prefab);
                    crafter = obj.GetComponent<Workbench>();
                }
                break;
#if SUBNAUTICA
                case ModelTypes.MoonPool:
                {
                    IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("Submarine/Build/CyclopsFabricator");
                    yield return request;
                    request.TryGetPrefab(out GameObject prefab);

                    obj = GameObject.Instantiate(prefab);

                    crafter = obj.GetComponent<Fabricator>();

                    // Add prefab ID because CyclopsFabricator normaly doesn't have one
                    PrefabIdentifier prefabId = obj.AddComponent<PrefabIdentifier>();
                    prefabId.ClassId = FabricatorDetails.ItemID;
                    prefabId.name = FabricatorDetails.DisplayName;

                    // Add tech tag because CyclopsFabricator normaly doesn't have one
                    TechTag techTag = obj.AddComponent<TechTag>();
                    techTag.type = this.TechType;

                    // Retrieve sub game objects
                    GameObject cyclopsFabLight = obj.FindChild("fabricatorLight");
                    GameObject cyclopsFabModel = obj.FindChild("submarine_fabricator_03");
                    // Translate CyclopsFabricator model and light
                    obj.transform.localPosition = new Vector3(cyclopsFabModel.transform.localPosition.x, // Same X position
                                                                 cyclopsFabModel.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabModel.transform.localPosition.z); // Same Z position
                    obj.transform.localPosition = new Vector3(cyclopsFabLight.transform.localPosition.x, // Same X position
                                                                 cyclopsFabLight.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabLight.transform.localPosition.z); // Same Z position
                    // Add constructable - This prefab normally isn't constructed.
                    constructible = obj.AddComponent<Constructable>();
                    constructible.model = cyclopsFabModel;
                }
                break;
#endif
                default:
                    throw new InvalidOperationException("ModelType in CustomFabricator does not correspond to a valid fabricator type");
            }

            crafter.craftTree = FabricatorDetails.TreeTypeID;
            crafter.handOverText = $"Use {FabricatorDetails.DisplayName}";

            constructible = constructible ?? obj.GetComponent<Constructable>();

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

            SkyApplier skyApplier = obj.GetComponentInChildren<SkyApplier>(true);
            if (skyApplier != null)
            {
                skyApplier.renderers = obj.GetComponentsInChildren<Renderer>();
                skyApplier.anchorSky = Skies.Auto;
            }
            else
            {
                QuickLogger.Warning("Unable to locate SkyApplier for custom fabricator", true);
            }
            

            if (FabricatorDetails.HasColorValue)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material.color = FabricatorDetails.ColorTint; // Tint option available
            }

            // Associate power relay
            var powerRelay = new PowerRelay();

            // This is actually a dirty hack
            // The problem is that the parent SubRoot isn't correctly associated at this time.
            // The power relay should be getting set in the GhostCrafter Start() method.
            // But the parent components are coming up null.
            crafter.powerRelay = powerRelay;

            gameObject.Set(obj);
        }
    }
}
