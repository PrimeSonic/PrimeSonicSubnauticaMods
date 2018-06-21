namespace CustomCraft
{
    using System.Collections.Generic;
    using CustomCraft.PublicAPI;
    using UnityEngine.Assertions;

    internal abstract class CraftingNode
    {
        internal const char Splitter = '/';
        internal readonly CraftingNode Parent = null;

        protected CraftingNode(CraftingNode parent)
        {
            Parent = parent;
        }

        protected virtual CraftTree.Type Scheme => Parent.Scheme;

        internal abstract string Name { get; }

        protected string GetPath()
        {
            var steps = new Stack<string>(4);
            // Currently the deepest trees in the standard fabricators only go 3 deep

            CraftingNode node = this;
            while (node != null)
            {
                steps.Push(node.Name);
                node = node.Parent;
            }

            string path = string.Empty;
            while (steps.Count > 0)
            {
                path += steps.Pop() + Splitter;
            }

            path.TrimEnd(Splitter);

            return path;
        }

        internal CraftingPath GetCraftingPath => new CraftingPath(Scheme, GetPath());
    }

    internal class CraftingRoot : CraftingNode
    {
        public static readonly IList<CraftTree.Type> ValidCraftTrees = new List<CraftTree.Type>(6)
        {
            CraftTree.Type.Fabricator,
            CraftTree.Type.Constructor,
            CraftTree.Type.Workbench,
            CraftTree.Type.SeamothUpgrades,
            CraftTree.Type.MapRoom,
            CraftTree.Type.CyclopsFabricator
        };

        private readonly CraftTree.Type scheme;

        protected override CraftTree.Type Scheme => scheme;
        internal override string Name => scheme.ToString();

        internal CraftingRoot(CraftTree.Type scheme) : base(null)
        {
            Assert.IsTrue(ValidCraftTrees.Contains(scheme), "This class is only for use with standard, non-modded CraftTree Types.");

            this.scheme = scheme;
        }
    }

    internal class CraftingTab : CraftingNode
    {
        internal readonly string TabName;
        internal override string Name => TabName;

        internal CraftingTab(CraftingNode parent, string tabName) : base(parent)
        {
            TabName = tabName;
        }
    }

}
