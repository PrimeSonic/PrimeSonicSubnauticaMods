namespace CustomCraft2SML.PublicAPI
{
    using System;

    public class CraftingPath
    {
        public const char Separator = '/';

        public readonly CraftTree.Type Scheme;
        public readonly string Path;
        public string[] Steps { get; internal set; }
        public string[] CraftNodeSteps { get; internal set; }
        public bool IsAtRoot => this.Steps == null || string.IsNullOrEmpty(Path) || this.Steps.Length == 0;

        internal CraftingPath(CraftTree.Type scheme, string path)
        {
            Scheme = scheme;
            Path = path;
        }

        internal CraftingPath(string path, string craftNode = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Scheme = CraftTree.Type.None;
                return;
            }

            path = path.TrimEnd(Separator);

            int firstBreak = path.IndexOf(Separator);

            string schemeString;

            if (firstBreak > -1)
            {
                schemeString = path.Substring(0, firstBreak);
                Path = path.Substring(firstBreak + 1);
                this.Steps = Path.Split(Separator);
            }
            else
            {
                schemeString = path;
            }

            Scheme = (CraftTree.Type)Enum.Parse(typeof(CraftTree.Type), schemeString);

            if (!string.IsNullOrEmpty(craftNode))
            {
                CraftNodeSteps = $"{path.TrimEnd(Separator)}/{craftNode}".Split(Separator);
            }
        }

        public override string ToString() => Path.TrimEnd(Separator);
    }
}
