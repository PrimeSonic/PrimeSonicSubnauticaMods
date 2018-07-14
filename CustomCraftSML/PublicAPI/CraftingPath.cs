namespace CustomCraft2SML.PublicAPI
{
    using System;

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
            int firstBreak = path.IndexOf('/');
            string schemeString = path.Substring(0, firstBreak);

            Scheme = (CraftTree.Type)Enum.Parse(typeof(CraftTree.Type), schemeString);
            Path = path.Substring(firstBreak + 1);
        }

        public override string ToString()
        {
            return Path.TrimEnd('/');
        }
    }
}
