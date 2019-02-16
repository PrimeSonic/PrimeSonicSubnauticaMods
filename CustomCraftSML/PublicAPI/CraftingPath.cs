namespace CustomCraft2SML.PublicAPI
{
    using System;

    public class CraftingPath
    {
        public const char Separator = '/';

        public CraftTree.Type Scheme { get; private set; } = CraftTree.Type.None;
        public string Path { get; private set; }
        public string[] Steps { get; internal set; }
        public string[] CraftNodeSteps { get; internal set; }
        public bool IsAtRoot => this.Steps == null || string.IsNullOrEmpty(this.Path) || this.Steps.Length == 0;

        internal CraftingPath(CraftTree.Type scheme, string path)
        {
            this.Scheme = scheme;
            this.Path = path;
        }

        internal CraftingPath(string path, string craftNode = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.Scheme = CraftTree.Type.None;
                return;
            }

            path = path.TrimEnd(Separator);

            int firstBreak = path.IndexOf(Separator);

            string schemeString;

            if (firstBreak > -1)
            {
                schemeString = path.Substring(0, firstBreak);
                this.Steps = path.Substring(firstBreak + 1).Split(Separator);
            }
            else
            {
                schemeString = path;
            }

            this.Scheme = (CraftTree.Type)Enum.Parse(typeof(CraftTree.Type), schemeString);

            if (!string.IsNullOrEmpty(craftNode))
            {
                this.Path = $"{path.TrimEnd(Separator)}/{craftNode}";
                this.CraftNodeSteps = this.Path.Substring(firstBreak + 1).Split(Separator);
            }
            else
            {
                this.Path = $"{path.TrimEnd(Separator)}";
            }
        }

        public override string ToString() => this.Path.TrimEnd(Separator);
    }
}
