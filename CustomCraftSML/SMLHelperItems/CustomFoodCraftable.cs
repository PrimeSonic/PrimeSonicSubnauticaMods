namespace CustomCraft2SML.SMLHelperItems
{
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CustomFoodCraftable : Craftable
    {
        public override CraftTree.Type FabricatorType => Path.Scheme;
        public override string[] StepsToFabricatorTab => Path.Steps;
        public override TechGroup GroupForPDA => FoodEntry.PdaGroup;
        public override TechCategory CategoryForPDA => FoodEntry.PdaCategory;

        public override TechType RequiredForUnlock => FoodEntry.UnlockedBy_;

        public override string AssetsFolder => FileLocations.AssetsFolder;
        
        // TODO Unlocks will need to be handled

        public readonly TechType FoodItemOriginal;
        public readonly CustomFood FoodEntry;
        public readonly CraftingPath Path;

        public CustomFoodCraftable(CustomFood customFood, CraftingPath path, TechType baseItem)
            : base(customFood.ItemID, /*$"{customFood.ItemID}Prefab"*/customFood.DisplayName, customFood.Tooltip)
        {
            FoodEntry = customFood;
            Path = path;
            FoodItemOriginal = baseItem;
            
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(FoodItemOriginal);
            GameObject obj = UnityEngine.Object.Instantiate(prefab);

            Eatable eatable = obj.GetComponent<Eatable>();
            if (eatable != null)
            {
                eatable.foodValue = FoodEntry.FoodValue;
                eatable.waterValue = FoodEntry.WaterValue;
                eatable.decomposes = FoodEntry.Decomposes;
                eatable.kDecayRate = FoodEntry.DecayRate / 10000f;
                eatable.allowOverfill = FoodEntry.Overfill;
            }
            else
            {
                /*foreach (Component component in obj.GetComponents<Component>())
                {
                    if (component is Transform || component is MeshRenderer || component is MeshFilter)
                    {

                    }
                    else
                    {
                        GameObject.Destroy(component);
                    }
                }*/
                obj.AddComponent<Eatable>();
                eatable = obj.GetComponent<Eatable>();
                eatable.foodValue = FoodEntry.FoodValue;
                eatable.waterValue = FoodEntry.WaterValue;
                eatable.decomposes = FoodEntry.Decomposes;
                eatable.kDecayRate = FoodEntry.DecayRate / 10000f;
                eatable.allowOverfill = FoodEntry.Overfill;
            }

            // ADD MORE OPTIONS!

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return FoodEntry.CreateRecipeTechData();
        }
    }
}
