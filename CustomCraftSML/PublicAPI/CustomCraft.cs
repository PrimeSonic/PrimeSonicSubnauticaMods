namespace CustomCraft.PublicAPI
{
    using SMLHelper;
    using SMLHelper.Patchers;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {        
        public static void AddRecipe(TechType craftedItem, TechDataHelper recipe, CraftingPath craftingPath)
        {
            AddRecipe(craftedItem, recipe, craftingPath.Scheme, craftingPath.Path);
        }

        public static void AddRecipe(TechType craftedItem, TechDataHelper recipe, CraftTree.Type craftTree, string path)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(craftedItem, craftTree, path));
            CraftDataPatcher.customTechData[craftedItem] = recipe;
        }

        public static void ModifyRecipe(TechType craftedItem, TechDataHelper recipe)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            CraftDataPatcher.customTechData[craftedItem] = recipe;
        }

        public static void ModifyItemSize(TechType inventoryItem, int width, int height)
        {
            Assert.AreNotEqual(inventoryItem.ToString(), ((int)inventoryItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(width > 0 && height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(width < 6 && height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataPatcher.customItemSizes[inventoryItem] = new Vector2int(width, height);
        }
    }
}
