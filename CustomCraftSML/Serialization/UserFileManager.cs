namespace CustomCraftSML.Serialization
{
    using CustomCraft.PublicAPI;
    using CustomCraftSML.Serialization.EasyMarkup;

    internal static class UserFileManager
    {
        private const string FolderRoot = @"./QMods/CustomCraftSML/";
        private const string CustomSizesFile = FolderRoot + "CustomSizes.txt";
        private const string ModifiedRecipesFile = FolderRoot + "ModifiedRecipes.txt";
        private const string AddedRecipiesFile = FolderRoot + "AddedRecipes.txt";

        private static CustomSizeList customSizeList;
        private static ModifiedRecipeList modifiedRecipeList;
        private static AddedRecipeList addedRecipeList;

        internal static void PatchFromFiles()
        {
            customSizeList = new CustomSizeList();
            modifiedRecipeList = new ModifiedRecipeList();
            addedRecipeList = new AddedRecipeList();

            if (customSizeList.Deserialize(CustomSizesFile))
            {
                foreach (ICustomSize customSize in customSizeList)
                {
                    CustomCraft.ModifyItemSize(customSize.ItemID, customSize.Width, customSize.Height);
                }
            }

            if (modifiedRecipeList.Deserialize(ModifiedRecipesFile))
            {
                foreach (IModifiedRecipe item in modifiedRecipeList)
                {
                    CustomCraft.ModifyRecipe(item.ItemID, item.SmlHelperRecipe());
                }
            }

            if (addedRecipeList.Deserialize(AddedRecipiesFile))
            {
                foreach (IAddedRecipe item in addedRecipeList)
                {
                    CustomCraft.AddRecipe(item.ItemID, item.SmlHelperRecipe(), new CraftingPath(item.Path));
                }
            }

        }

    }
}
