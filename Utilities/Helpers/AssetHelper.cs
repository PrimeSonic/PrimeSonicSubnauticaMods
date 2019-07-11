namespace Common.Helpers
{
    using System;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// A  helper class that deals with the AssetBudle
    /// </summary>
    internal static class AssetHelper
    {
        /// <summary>
        /// The AssetBundle for the mod
        /// </summary>
        public static AssetBundle Asset(string modName, string modBundleName)
        {
            if (modName.Equals(string.Empty) && modBundleName.Equals(string.Empty))
            {
                throw new ArgumentException($"Both {nameof(modName)} and {nameof(modBundleName)} are empty");
            }

            if (modName.Equals(string.Empty) || modBundleName.Equals(string.Empty))
            {
                string result = modName.Equals(string.Empty) ? nameof(modName) : nameof(modBundleName);
                throw new ArgumentException($"{result} is empty");
            }

            return AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Environment.CurrentDirectory, "QMods"), Path.Combine(modName, Path.Combine("Assets", modBundleName))));
        }

    }
}
