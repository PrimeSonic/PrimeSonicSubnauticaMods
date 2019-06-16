namespace CyclopsBioReactor.Management
{

    using Common;
    using MoreCyclopsUpgrades.API;

    internal partial class BioManager : IAuxCyclopsManager
    {
        internal const string ManagerName = "CyBioMgr";

        internal static TechType CyBioBoosterID { get; set; }
        internal static TechType CyBioReactorID { get; set; }

        public string Name { get; } = ManagerName;

        public bool Initialize(SubRoot cyclops)
        {
            return
                Cyclops == cyclops &&
                CyBioBoosterID != TechType.None &&
                CyBioReactorID != TechType.None;
        }

        internal static void SyncAllBioReactors()
        {
            foreach (BioManager mgr in MCUServices.Client.GetAllManagers<BioManager>(ManagerName))
                mgr.SyncBioReactors();
        }

        internal void SyncBioReactors()
        {
            TempCache.Clear();

            SubRoot cyclops = Cyclops;

            CyBioReactorMono[] cyBioReactors = cyclops.GetAllComponentsInChildren<CyBioReactorMono>();

            foreach (CyBioReactorMono cyBioReactor in cyBioReactors)
            {
                if (TempCache.Contains(cyBioReactor))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(cyBioReactor);

                if (cyBioReactor.ParentCyclops == null)
                {
                    QuickLogger.Debug("CyBioReactorMono synced externally");
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    cyBioReactor.ConnectToCyclops(cyclops, this);
                }
            }

            if (TempCache.Count != CyBioReactors.Count)
            {
                CyBioReactors.Clear();
                CyBioReactors.AddRange(TempCache);
            }
        }

        internal static void RemoveReactor(CyBioReactorMono cyBioReactorMono)
        {
            throw new System.NotImplementedException();
        }
    }
}
