namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine;

    public class DepletedNuclearModule
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameId = "DepletedCyclopsNuclearModule";
        public const string FriendlyName = "Depleted Cyclops Nuclear Reactor Module";
        public const string Description = "Bring to a specialize fabricator for safe extraction of the depleted reactor rod inside.";

        public static void Patch()
        {
            TechTypeID = TechTypePatcher.AddTechType(NameId, FriendlyName, Description);


            TechType dummy = TechTypePatcher.AddTechType("CyclopsNuclearModuleRefil", "Reload Cyclops Nuclear Module", "Reload a Depleted Cyclops Nuclear Module with a Reactor Rod");


            Atlas.Sprite sprite = SpriteManager.Get(TechType.DepletedReactorRod);
            CustomSprite customSprite = new CustomSprite(dummy, sprite);
            CustomCraftNode customCraftNode = new CustomCraftNode(dummy, CraftTree.Type.Workbench, $"CyclopsMenu/{NameId}");
            TechDataHelper techData = new TechDataHelper()
            {
                _craftAmount = 0,
                _ingredients = new List<IngredientHelper>()
                {
                    new IngredientHelper(TechTypeID, 1),
                    new IngredientHelper(TechType.ReactorRod, 1)
                },
                _linkedItems = new List<TechType>()
                {
                    NuclearCharger.TechTypeID,
                    TechType.DepletedReactorRod
                },
                _techType = dummy
            };

            CustomSpriteHandler.customSprites.Add(customSprite);
            CraftDataPatcher.customTechData.Add(dummy, techData);
            CraftTreePatcher.customNodes.Add(customCraftNode);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameId, $"WorldEntities/Natural/{NameId}", dummy, DepletedGameObject));
        }

        public static GameObject DepletedGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Natural/DepletedReactorRod");
            GameObject gameObject = GameObject.Instantiate(prefab);

            gameObject.GetComponent<PrefabIdentifier>().ClassId = "DepletedCyclopsNuclearModule";
            gameObject.AddComponent<TechTag>().type = TechTypeID;

            return gameObject;
        }
    }
}
