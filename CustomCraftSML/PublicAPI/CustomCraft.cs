namespace CustomCraft2SML.PublicAPI
{
    using CustomCraft2SML.Serialization;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {
        public const string RootModName = "CustomCraft2SML";

        public static void AddRecipe(IAddedRecipe addedRecipe)
        {
            var path = new CraftingPath(addedRecipe.Path);

            AddRecipe(addedRecipe.ItemID, addedRecipe.SmlHelperRecipe(), path.Scheme, path.Path);

            if (addedRecipe.ForceUnlockAtStart)
                KnownTechHandler.UnlockOnStart(addedRecipe.ItemID);

            if (addedRecipe.Unlocks.Count > 0)
                KnownTechHandler.SetAnalysisTechEntry(addedRecipe.ItemID, addedRecipe.Unlocks);
        }

        internal static void AddRecipe(TechType craftedItem, TechData recipe, CraftTree.Type craftTree, string path)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            //CraftTreeHandler.customNodes.Add(new CustomCraftNode(craftedItem, craftTree, path));            
            CraftDataHandler.SetTechData(craftedItem, recipe);

            string[] steps = path.Split(CraftingNode.Splitter);

            if (steps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(craftTree, craftedItem);
            else
                CraftTreeHandler.AddCraftingNode(craftTree, craftedItem, steps);
        }

        public static void ModifyRecipe(IModifiedRecipe modifiedRecipe)
        {
            ModifyRecipe(modifiedRecipe.ItemID, modifiedRecipe.SmlHelperRecipe());

            if (modifiedRecipe.ForceUnlockAtStart)
                KnownTechHandler.UnlockOnStart(modifiedRecipe.ItemID);

            if (modifiedRecipe.Unlocks.Count > 0)            
                KnownTechHandler.SetAnalysisTechEntry(modifiedRecipe.ItemID, modifiedRecipe.Unlocks);            
        }

        internal static void ModifyRecipe(TechType craftedItem, TechData recipe)
        {
            Assert.AreNotEqual(craftedItem.ToString(), ((int)craftedItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");
            // Only modded enums use the int string as their ToString value

            CraftDataHandler.SetTechData(craftedItem, recipe);
        }

        public static void CustomizeItemSize(ICustomSize customSize)
        {
            CustomizeItemSize(customSize.ItemID, customSize.Width, customSize.Height);
        }

        internal static void CustomizeItemSize(TechType inventoryItem, int width, int height)
        {
            Assert.AreNotEqual(inventoryItem.ToString(), ((int)inventoryItem).ToString(), "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(width > 0 && height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(width < 6 && height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataHandler.SetItemSize(inventoryItem, width, height);            
        }
    }
}
