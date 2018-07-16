namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class NuclearFabricator
    {
        public static CraftTree.Type NukeFabTreeType { get; private set; }
        public static TechType NukeFabTechType { get; private set; }

        // This name will be used as both the new TechType of the buildable fabricator and the CraftTree Type for the custom crafting tree.
        public const string CustomFabAndTreeID = "NuclearFabricator";

        // The text you'll see in-game when you mouseover over it.
        public const string FriendlyName = "Nuclear Fabricator";

        public const string HandOverText = "UseNukFabricator";

        public static void Patch()
        {
            // Create new Craft Tree Type
            CreateCustomTree(out CraftTree.Type craftType);
            NukeFabTreeType = craftType;

            // Create a new TechType for new fabricator
            NukeFabTechType = TechTypeHandler.AddTechType(CustomFabAndTreeID, FriendlyName, 
                "A specialized fabricator for safe handling of radioactive energy sources.", false);

            // Create a Recipie for the new TechType
            var customFabRecipe = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.Titanium, 2),
                                 new Ingredient(TechType.ComputerChip, 1),
                                 new Ingredient(TechType.Magnetite, 1),
                                 new Ingredient(TechType.Lead, 2),
                             })
            };

            // Add the new TechType to the buildables
            CraftDataHandler.AddBuildable(NukeFabTechType);

            // Add the new TechType to the group of Interior Module buildables
            CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, NukeFabTechType);

            // Set the buildable prefab
            PrefabHandler.RegisterPrefab(new VModFabricatorModulePrefab(CustomFabAndTreeID, NukeFabTechType));

            LanguageHandler.SetLanguageLine(HandOverText, "Use Nuclear Fabricator");

            // Set the custom sprite for the Habitat Builder Tool menu
            SpriteHandler.RegisterSprite(NukeFabTechType, @"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorI.png");

            // Associate the recipie to the new TechType
            CraftDataHandler.SetTechData(NukeFabTechType, customFabRecipe);

            KnownTechHandler.SetAnalysisTechEntry(CyclopsModule.DepletedNuclearModuleID, new TechType[1] { NukeFabTechType }, $"{FriendlyName} blueprint discovered!");
        }

        private static void CreateCustomTree(out CraftTree.Type craftType)
        {
            ModCraftTreeRoot rootNode = CraftTreeHandler.CreateCustomCraftTreeAndType(CustomFabAndTreeID, out craftType);

            rootNode.AddCraftingNode(CyclopsModule.NuclearChargerID);
            rootNode.AddCraftingNode(CyclopsModule.RefillNuclearModuleID);
            rootNode.AddCraftingNode(TechType.ReactorRod);
        }

        internal class VModFabricatorModulePrefab : ModPrefab
        {
            internal VModFabricatorModulePrefab(string classId, TechType techType) : base(classId, $"{classId}PreFab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                // Instantiate CyclopsFabricator object
                GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/Fabricator"));                

                // Update prefab name
                prefab.name = CustomFabAndTreeID;

                // Add prefab ID
                var prefabId = prefab.AddComponent<PrefabIdentifier>();
                prefabId.ClassId = CustomFabAndTreeID;
                prefabId.name = FriendlyName;

                // Add tech tag
                var techTag = prefab.AddComponent<TechTag>();
                techTag.type = NukeFabTechType;

                // Update sky applier
                var skyApplier = prefab.GetComponent<SkyApplier>();
                skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
                skyApplier.anchorSky = Skies.Auto;

                // Associate custom craft tree to the fabricator
                var fabricator = prefab.GetComponent<Fabricator>();
                fabricator.craftTree = NukeFabTreeType;
                fabricator.handOverText = HandOverText;

                // Associate power relay
                var ghost = fabricator.GetComponent<GhostCrafter>();
                var powerRelay = new PowerRelay();

                fabricator.SetPrivateField("powerRelay", powerRelay, BindingFlags.FlattenHierarchy);

                // Add constructable
                var constructible = prefab.AddComponent<Constructable>();
                constructible.allowedInBase = true;
                constructible.allowedInSub = true;
                constructible.allowedOutside = false;
                constructible.allowedOnCeiling = false;
                constructible.allowedOnGround = false;
                constructible.allowedOnWall = true;
                constructible.allowedOnConstructables = false;
                constructible.controlModelState = true;
                constructible.rotationEnabled = false;
                constructible.techType = NukeFabTechType; // This was necessary to correctly associate the recipe at building time

                // Set the custom texture
                var customTexture = ImageUtils.LoadTextureFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/NuclearFabricatorT.png");
                var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material.mainTexture = customTexture;

                return prefab;
            }
        }
    }
}
