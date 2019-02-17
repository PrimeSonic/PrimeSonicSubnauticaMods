using System;
using System.Collections.Generic;

namespace CustomCraft2SML.PublicAPI
{
    public class CraftingPath
    {
        public const char Separator = '/';

        public CraftTree.Type Scheme { get; private set; } = CraftTree.Type.None;
        public string Path { get; private set; }
        public string[] Steps { get; internal set; }
        public string[] CraftNodeSteps { get; internal set; }
        public bool IsAtRoot => this.Steps == null || string.IsNullOrEmpty(this.Path) || this.Steps.Length == 0;

        internal CraftingPath(CraftTree.Type scheme, string path) : this(scheme.ToString(), path)
        {
            this.Scheme = scheme;
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

            if (this.Scheme == CraftTree.Type.None)
                this.Scheme = GetCraftTreeType(schemeString);

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

        internal static readonly Dictionary<string, CraftTree.Type> CraftTreeLookup = new Dictionary<string, CraftTree.Type>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Fabricator", CraftTree.Type.Fabricator },
            { "Constructor", CraftTree.Type.Constructor },
            { "MobileVehicleBay", CraftTree.Type.Constructor },
            { "Workbench", CraftTree.Type.Workbench },
            { "ModificationStation", CraftTree.Type.Workbench },
            { "SeamothUpgrades", CraftTree.Type.SeamothUpgrades },
            { "VehicleUpgradeConsole", CraftTree.Type.SeamothUpgrades },
            { "MapRoom", CraftTree.Type.MapRoom },
            { "ScannerRoom", CraftTree.Type.MapRoom },
            { "CyclopsFabricator", CraftTree.Type.CyclopsFabricator },
        };

        internal static CraftTree.Type GetCraftTreeType(string schemeString)
        {
            if (CraftTreeLookup.TryGetValue(schemeString, out CraftTree.Type type))
            {
                return type;
            }

            return CraftTree.Type.None;
        }
    }
}
