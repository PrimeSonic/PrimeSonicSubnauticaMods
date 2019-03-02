namespace CustomCraft2SML.SMLHelperItems
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CustomFoodCraftable : Craftable
    {
        public override CraftTree.Type FabricatorType { get; }
        public override TechGroup GroupForPDA { get; }
        public override TechCategory CategoryForPDA { get; }
        public override string AssetsFolder { get; }

        public readonly TechType FoodItemOriginal;


        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(FoodItemOriginal);
            var obj = UnityEngine.Object.Instantiate(prefab);

            var identifier = obj.GetComponent<PrefabIdentifier>();
            var techTag = obj.GetComponent<TechTag>();
            var eatable = obj.GetComponent<Eatable>();

            //identifier.ClassId = ItemID;
            //techTag.type = Alias TechType;
            //eatable.foodValue = food;
            //eatable.waterValue = water;
            // ADD MORE OPTIONS!

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            throw new NotImplementedException();
        }
    }
}
