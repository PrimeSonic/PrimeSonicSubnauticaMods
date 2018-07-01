using System;

namespace CustomCraft.PublicAPI
{
    public class CraftingPath
    {
        public readonly CraftTree.Type Scheme;
        public readonly string Path;

        internal CraftingPath(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }

        internal CraftingPath(string path)
        {
            Path = path;

            string schemeString = path.Substring(0, path.IndexOf('/'));

            Scheme = (CraftTree.Type)Enum.Parse(typeof(CraftTree.Type), schemeString);
        }

        public override string ToString()
        {
            return $"CraftingPath > {Scheme}/{Path}";
        }
    }
}
