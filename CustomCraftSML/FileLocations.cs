using CustomCraft2SML.Serialization;

namespace CustomCraft2SML
{
    internal static class FileLocations
    {
        internal const string RootModName = "CustomCraft2SML";
        internal const string ModFriendlyName = "Custom Craft 2";
        internal const string FolderRoot = "./QMods/" + RootModName + "/";
        internal const string SamplesFolder = FolderRoot + "SampleFiles/";
        internal const string OriginalsFolder = FolderRoot + "OriginalRecipes/";
        internal const string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";
        internal const string WorkingFolder = FolderRoot + "WorkingFiles/";
        internal const string AssetsFolder = FolderRoot + "Assets/";
        internal const string ModJson = FolderRoot + "mod.json";
        internal const string ConfigFile = FolderRoot + CustomCraft2Config.FileName;
    }
}
