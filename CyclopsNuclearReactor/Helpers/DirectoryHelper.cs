namespace CyclopsNuclearReactor.Helpers
{
    using System;
    using System.IO;

    public static class DirectoryHelper
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
