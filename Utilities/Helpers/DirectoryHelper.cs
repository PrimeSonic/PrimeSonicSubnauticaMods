namespace Common.Helpers
{
    using System;
    using System.IO;

    internal static class DirectoryHelper
    {
        public static string GrabFromAssetsDirectory(string modName, string file)
        {
            string path = Path.Combine(Path.Combine(Environment.CurrentDirectory, "QMods"),
                Path.Combine(modName, Path.Combine("Assets", file)));

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return path;
        }
    }
}
