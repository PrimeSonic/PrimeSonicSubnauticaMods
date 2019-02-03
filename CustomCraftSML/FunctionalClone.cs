namespace CustomCraft2SML
{
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class FunctionalClone : ModPrefab
    {
        internal readonly TechType BaseItem;
        public FunctionalClone(IAliasRecipe aliasRecipe, TechType baseItem)
            : base(aliasRecipe.ItemID, $"{aliasRecipe}Prefab", CustomCraft.GetTechType(aliasRecipe.ItemID))
        {
            BaseItem = baseItem;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(BaseItem);
            return GameObject.Instantiate(prefab);
        }
    }
}
