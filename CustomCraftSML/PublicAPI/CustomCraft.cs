namespace CustomCraft2SML.PublicAPI
{
    using CustomCraft2SML.Serialization;
    using SMLHelper.V2.Handlers;
    using UnityEngine.Assertions;

    public static class CustomCraft
    {
        public const string RootModName = "CustomCraft2SML";

        public static void AddRecipe(IAddedRecipe addedRecipe)
        {
            Assert.IsTrue(addedRecipe.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            HandleNewRecipe(addedRecipe);

            HandleCraftTreeAddition(addedRecipe);

            HandleUnlocks(addedRecipe);
        }

        private static void HandleCraftTreeAddition(IAddedRecipe addedRecipe)
        {
            var craftPath = new CraftingPath(addedRecipe.Path);

            string[] steps = craftPath.Path.Split(CraftingNode.Splitter);

            if (steps.Length <= 1)
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, addedRecipe.ItemID);
            else
                CraftTreeHandler.AddCraftingNode(craftPath.Scheme, addedRecipe.ItemID, steps);
        }

        public static void ModifyRecipe(IModifiedRecipe modifiedRecipe)
        {
            Assert.IsTrue(modifiedRecipe.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            HandleNewRecipe(modifiedRecipe);

            HandleUnlocks(modifiedRecipe);
        }

        private static void HandleNewRecipe(IModifiedRecipe modifiedRecipe)
        {
            if (modifiedRecipe.IngredientCount > 0 || modifiedRecipe.LinkedItemCount > 0)
                CraftDataHandler.SetTechData(modifiedRecipe.ItemID, modifiedRecipe.SmlHelperRecipe());
        }

        private static void HandleUnlocks(IModifiedRecipe modifiedRecipe)
        {
            if (modifiedRecipe.ForceUnlockAtStart)
                KnownTechHandler.UnlockOnStart(modifiedRecipe.ItemID);

            if (modifiedRecipe.Unlocks.Count > 0)
                KnownTechHandler.SetAnalysisTechEntry(modifiedRecipe.ItemID, modifiedRecipe.Unlocks);
        }

        public static void CustomizeItemSize(ICustomSize customSize)
        {
            Assert.IsTrue(customSize.ItemID <= TechType.Databox, "This API in intended only for use with standard, non-modded TechTypes.");

            Assert.IsTrue(customSize.Width > 0 && customSize.Height > 0, "Values must be positive and non-zero");
            Assert.IsTrue(customSize.Width < 6 && customSize.Height < 6, "Values must be smaller than six to fit");
            // Value chosen for what should be the standard inventory size

            CraftDataHandler.SetItemSize(customSize.ItemID, customSize.Width, customSize.Height);
        }
    }
}
