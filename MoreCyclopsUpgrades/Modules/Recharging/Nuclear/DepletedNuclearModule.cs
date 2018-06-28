using SMLHelper;
using SMLHelper.Patchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreCyclopsUpgrades
{
    public class DepletedNuclearModule
    {
        public static TechType DepletedCYNUKM { get; protected set; }

        public static void Patch()
        {
            DepletedCYNUKM = TechTypePatcher.AddTechType("DepletedCyclopsNuclearModule", "Depleted Cyclops Nuclear Module", "A depleted Cyclops Nuclear Module");
            TechType dummy = TechTypePatcher.AddTechType("CyclopsNuclearModuleRefil", "Recharge Cyclops Nuclear Module", "Recharge a Depleted Cyclops Nuclear Module with a Reactor Rod");
            Atlas.Sprite sprite = SpriteManager.Get(TechType.DepletedReactorRod);
            CustomSprite customSprite = new CustomSprite(dummy, sprite);
            CustomCraftNode customCraftNode = new CustomCraftNode(dummy, CraftTree.Type.Workbench, "CyclopsMenu/DepletedCyclopsNuclearModule");
            TechDataHelper techData = new TechDataHelper()
            {
                _craftAmount = 0,
                _ingredients = new List<IngredientHelper>()
                {
                    new IngredientHelper(DepletedCYNUKM, 1),
                    new IngredientHelper(TechType.ReactorRod, 1)
                },
                _linkedItems = new List<TechType>()
                {
                    NuclearCharger.CyNukBatteryType,
                    TechType.DepletedReactorRod
                },
                _techType = dummy
            };

            CustomSpriteHandler.customSprites.Add(customSprite);
            CraftDataPatcher.customTechData.Add(dummy, techData);
            CraftTreePatcher.customNodes.Add(customCraftNode);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab("DepletedCyclopsNuclearModule", "WorldEntities/Natural/DepletedCyclopsNuclearModule", dummy, DepletedGameObject));
        }

        public static GameObject DepletedGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Natural/DepletedReactorRod");
            GameObject obj = UnityEngine.Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = "DepletedCyclopsNuclearModule";

            return obj;
        }
    }
}
