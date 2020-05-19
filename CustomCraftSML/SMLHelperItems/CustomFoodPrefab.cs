namespace CustomCraft2SML.SMLHelperItems
{
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class CustomFoodPrefab : ModPrefab
    {
        public const float StandardDecayRate = 0.015f;

        public readonly CustomFood FoodEntry;

        public CustomFoodPrefab(CustomFood customFood)
            : base(customFood.ItemID, $"{customFood.ItemID}Prefab", customFood.TechType)
        {
            FoodEntry = customFood;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(FoodEntry.FoodPrefab);
            GameObject obj = UnityEngine.Object.Instantiate(prefab);

            Eatable eatable = obj.GetComponent<Eatable>();

            if (eatable is null)
                eatable = obj.AddComponent<Eatable>();

            eatable.foodValue = FoodEntry.FoodValue;
            eatable.waterValue = FoodEntry.WaterValue;
            eatable.decomposes = FoodEntry.Decomposes;
            eatable.kDecayRate = FoodEntry.DecayRateMod * StandardDecayRate;

            return obj;
        }


    }
}
