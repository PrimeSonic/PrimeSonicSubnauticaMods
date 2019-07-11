namespace CyclopsBioReactor.Management
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;

    internal class BioAuxCyclopsManager : IAuxCyclopsManager
    {
        private static IEnumerable<BioAuxCyclopsManager> GetAllBioManagers()
        {
            return MCUServices.Find.AllAuxCyclopsManagers<BioAuxCyclopsManager>();
        }

        internal const int MaxBioReactors = BioChargeHandler.MaxBioReactors;

        internal readonly TechType cyBioBooster;
        internal readonly TechType cyBioReactor;

        internal readonly List<CyBioReactorMono> CyBioReactors = new List<CyBioReactorMono>();
        private readonly List<CyBioReactorMono> TempCache = new List<CyBioReactorMono>();

        internal readonly SubRoot Cyclops;

        public BioAuxCyclopsManager(SubRoot cyclops, TechType bioBooster, TechType bioReactor)
        {
            Cyclops = cyclops;
            cyBioBooster = bioBooster;
            cyBioReactor = bioReactor;
        }

        public bool Initialize(SubRoot cyclops)
        {
            return
                Cyclops == cyclops &&
                cyBioBooster != TechType.None &&
                cyBioReactor != TechType.None;
        }

        internal static void SyncAllBioReactors()
        {
            foreach (BioAuxCyclopsManager mgr in GetAllBioManagers())
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

        internal void RemoveSingleReactor(CyBioReactorMono cyBioReactorMono)
        {
            CyBioReactors.Remove(cyBioReactorMono);
        }

        internal static void RemoveReactor(CyBioReactorMono cyBioReactorMono)
        {
            foreach (BioAuxCyclopsManager mgr in GetAllBioManagers())
                mgr.RemoveSingleReactor(cyBioReactorMono);
        }
    }
}
