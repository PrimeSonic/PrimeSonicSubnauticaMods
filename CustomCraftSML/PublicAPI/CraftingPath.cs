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
    }
}
