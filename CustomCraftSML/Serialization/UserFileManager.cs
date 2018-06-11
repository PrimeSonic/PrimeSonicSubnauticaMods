namespace CustomCraftSML.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class UserFileManager
    {
        private const string FolderRoot = @"./QMods/CustomCraftSML/";
        private const string ModifiedSizesFile = FolderRoot + "ModifiedSizes.txt";

        internal static IList<ModifiedSize> LoadModifiedSizes()
        {
            try
            {
                if (File.Exists(ModifiedSizesFile))
                {
                    string[] lines = File.ReadAllLines(ModifiedSizesFile);
                    return ModifiedSize.Parse(lines);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Logger.Log("Error loading ModifiedSizes.txt", ex.ToString());
                return null;
            }
        }

        internal static void SaveModifiedSizes(IList<ModifiedSize> modifiedSizes)
        {
            try
            {
                string[] lines = new string[modifiedSizes.Count];
                for (int i = 0; i < modifiedSizes.Count; i++)
                {
                    lines[i] = modifiedSizes[i].ToString();
                }

                File.WriteAllLines(ModifiedSizesFile, lines);
            }
            catch (Exception ex)
            {
                Logger.Log("Error saving ModifiedSizes.txt", ex.ToString());
            }
        }

    }
}
