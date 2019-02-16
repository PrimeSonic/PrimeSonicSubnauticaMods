namespace CustomCraft2SML
{
    using CustomCraft2SML.Interfaces;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class FunctionalClone : Spawnable
    {
        internal readonly TechType BaseItem;
        public FunctionalClone(IAliasRecipe aliasRecipe, TechType baseItem)
            : base(aliasRecipe.ItemID, $"{aliasRecipe}Prefab", aliasRecipe.Tooltip)
        {
            BaseItem = baseItem;
            this.TechType = aliasRecipe.TechType;
        }

        public override string AssetsFolder { get; } = FileReaderWriter.RootModName + "/Assets";

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(BaseItem);
            return GameObject.Instantiate(prefab);
        }
    }
}
