namespace CustomCraft2SML.SMLHelperItems
{
    using System.Collections;
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

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            TaskResult<GameObject> result = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(FoodEntry.FoodPrefab, result);
            GameObject obj = result.Get();

            Eatable eatable = obj.GetComponent<Eatable>();

            if (eatable is null)
                eatable = obj.AddComponent<Eatable>();

            eatable.foodValue = FoodEntry.FoodValue;
            eatable.waterValue = FoodEntry.WaterValue;
            eatable.decomposes = FoodEntry.Decomposes;
            eatable.kDecayRate = FoodEntry.DecayRateMod * StandardDecayRate;

            gameObject.Set(obj);
        }
    }
}
