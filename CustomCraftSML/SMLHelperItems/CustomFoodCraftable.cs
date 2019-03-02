namespace CustomCraft2SML.SMLHelperItems
{
    using System;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Entries;
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
        public readonly CustomFood FoodEntry;

        public CustomFoodCraftable(CustomFood customFood, TechType baseItem)
            : base(customFood.ItemID, $"{customFood.ItemID}Prefab", customFood.Tooltip)
        {
            FoodEntry = customFood;
            FoodItemOriginal = baseItem;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(FoodItemOriginal);
            GameObject obj = UnityEngine.Object.Instantiate(prefab);

            Eatable eatable = obj.GetComponent<Eatable>();
                        
            eatable.foodValue = FoodEntry.FoodValue;
            eatable.waterValue = FoodEntry.WaterValue;
            eatable.decomposes = FoodEntry.Decomposes ?? true;
            eatable.kDecayRate = FoodEntry.DecayRate ?? 1;

            // ADD MORE OPTIONS!

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return FoodEntry.CreateRecipeTechData();
        }
    }
}
