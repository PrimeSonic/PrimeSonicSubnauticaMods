namespace CustomCraft2SML.Serialization
{
    using System;
    using System.Collections.Generic;

    internal class CraftTreePath
    {
        public const char Separator = '/';

        public CraftTree.Type Scheme { get; }
        public string RawPath { get; }
        public bool IsAtRoot { get; set; }
        public List<string> RawSteps { get; }
        public string FinalNodeID { get; }
        public string[] StepsToParentTab { get; }
        public string[] StepsToNode { get; }
        public bool HasError { get; }
        public string Error { get; }

        public CraftTreePath(string rawPath, string finalNode)
        {
            this.RawPath = rawPath;
            this.RawSteps = new List<string>(rawPath.Trim(Separator).Split(Separator));

            if (string.IsNullOrEmpty(this.RawPath) || this.RawSteps.Count == 0)
            {
                this.HasError = true;
                this.Error = "Empty craft tree path";
                return;
            }
            
            this.Scheme = GetCraftTreeType(this.RawSteps[0]);

            if (this.Scheme == CraftTree.Type.None)
            {
                this.HasError = true;
                this.Error = "Unable to identify fabricator from path";
                return;
            }

            this.IsAtRoot = this.RawSteps.Count == 1;
            this.FinalNodeID = finalNode;

            if (string.IsNullOrEmpty(this.FinalNodeID))
            {
                this.HasError = true;
                this.Error = "Missing TabID or ItemID";
                return;
            }

            this.StepsToParentTab = StepsToParentAdding();
            this.StepsToNode = StepsToNodeRemoving();

            this.HasError = false;
        }

        private string[] StepsToParentAdding()
        {
            if (this.IsAtRoot)
                return null;

            var copy = new List<string>(this.RawSteps);
            copy.RemoveAt(0);
            return copy.ToArray();
        }

        private string[] StepsToNodeRemoving()
        {
            if (this.IsAtRoot)
                return new[] { this.FinalNodeID };

            var copy = new List<string>(this.RawSteps)
            {
                this.FinalNodeID
            };
            copy.RemoveAt(0);
            return copy.ToArray();
        }

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
