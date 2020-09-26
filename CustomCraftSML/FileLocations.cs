using System.IO;
using System.Reflection;
using CustomCraft2SML.Serialization;

namespace CustomCraft2SML
{
    internal static class FileLocations
    {
        internal const string RootModName = "CustomCraft2SML";
        internal const string ModFriendlyName = "Custom Craft 2";
        internal static string FolderRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        internal static string SamplesFolder = FolderRoot + "SampleFiles/";
        internal static string OriginalsFolder = FolderRoot + "OriginalRecipes/";
        internal static string HowToFile = FolderRoot + "README_HowToUseThisMod.txt";
        internal static string WorkingFolder = FolderRoot + "WorkingFiles/";
        internal static string AssetsFolder = FolderRoot + "Assets/";
        internal static string ModJson = FolderRoot + "mod.json";
        internal static string ConfigFile = FolderRoot + CustomCraft2Config.FileName;
    }
}
