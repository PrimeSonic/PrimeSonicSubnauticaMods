namespace CustomCraft2SML.PublicAPI
{
    using System;
    using System.Collections.Generic;

    public class CraftingPath
    {
        public const char Separator = '/';

        public CraftTree.Type Scheme { get; private set; } = CraftTree.Type.None;
        public string Path { get; private set; }
        public string[] Steps { get; internal set; }
        public bool IsAtRoot { get; private set; }

        internal CraftingPath(CraftTree.Type scheme, string path, string craftNode = null) : this(path, craftNode)
        {
            this.Scheme = scheme;
        }

        internal CraftingPath(string path, string craftNode)
        {
            this.Path = path;

            if (string.IsNullOrEmpty(path) && this.Scheme == CraftTree.Type.None)
            {
                return;
            }

            string[] pathSteps = path.Trim(Separator).Split(Separator);

            if (this.Scheme == CraftTree.Type.None)
                this.Scheme = GetCraftTreeType(pathSteps[0]);

            if (pathSteps.Length == 1)
            {
                this.Steps = new[] { craftNode };
                this.IsAtRoot = true;
            }
            else
            {
                this.Steps = new string[pathSteps.Length - 1];
                this.IsAtRoot = false;

                for (int p = 1; p < pathSteps.Length; p++)                
                    this.Steps[p - 1] = pathSteps[p];                
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
