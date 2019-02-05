namespace CustomCraft2SML.PublicAPI
{
    using System;

    public class CraftingPath
    {
        public readonly CraftTree.Type Scheme;
        public readonly string Path;
        public string[] Steps { get; internal set; }
        public bool IsAtRoot => Steps == null || string.IsNullOrEmpty(Path) || Steps.Length == 0;

        internal CraftingPath(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }

        internal CraftingPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Scheme = CraftTree.Type.None;
                return;
            }

            int firstBreak = path.IndexOf('/');

            string schemeString;

            if (firstBreak > -1)
            {
                schemeString = path.Substring(0, firstBreak);
                Path = path.Substring(firstBreak + 1);
                this.Steps = Path.Split('/');
            }
            else
            {
                schemeString = path;
            }

            Scheme = (CraftTree.Type)Enum.Parse(typeof(CraftTree.Type), schemeString);
        }

        public override string ToString() => Path.TrimEnd('/');
    }
}
