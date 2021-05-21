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
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(FoodEntry.FoodPrefab);
            yield return task;
            GameObject prefab = task.GetResult();
            GameObject obj = Object.Instantiate(prefab);

            Eatable eatable = obj.EnsureComponent<Eatable>();

            eatable.foodValue = FoodEntry.FoodValue;
            eatable.waterValue = FoodEntry.WaterValue;
            eatable.decomposes = FoodEntry.Decomposes;
            eatable.kDecayRate = FoodEntry.DecayRateMod * StandardDecayRate;
#if BELOWZERO
            eatable.coldMeterValue = FoodEntry.HeatValue;
#endif
            gameObject.Set(obj);
        }
    }
}
