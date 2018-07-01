namespace CustomCraftSML.Serialization
{
    using System.IO;
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
            if (File.Exists(CustomSizesFile))
            {
                if (customSizeList.Deserialize(File.ReadAllText(CustomSizesFile)))
                {
                    foreach (ICustomSize customSize in customSizeList)
                    {
                        CustomCraft.ModifyItemSize(customSize.ItemID, customSize.Width, customSize.Height);
                    }
                }
            }

            modifiedRecipeList = new ModifiedRecipeList();
            if (File.Exists(ModifiedRecipesFile))
            {
                if (modifiedRecipeList.Deserialize(File.ReadAllText(ModifiedRecipesFile)))
                {
                    foreach (IModifiedRecipe item in modifiedRecipeList)
                    {
                        CustomCraft.ModifyRecipe(item.ItemID, item.SmlHelperRecipe());
                    }
                }
            }

            addedRecipeList = new AddedRecipeList();
            if (File.Exists(AddedRecipiesFile))
            {
                if (addedRecipeList.Deserialize(File.ReadAllText(AddedRecipiesFile)))
                {
                    foreach (IAddedRecipe item in addedRecipeList)
                    {
                        CustomCraft.AddRecipe(item.ItemID, item.SmlHelperRecipe(), new CraftingPath(item.Path));
                    }
                }
            }
        }

    }
}
