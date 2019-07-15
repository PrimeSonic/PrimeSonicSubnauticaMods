namespace CyclopsNuclearReactor
{
    using System;
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;

    internal class CyNukeManager : IAuxCyclopsManager
    {
        public const int MaxReactors = CyNukeChargeManager.MaxReactors;

        internal static void SyncReactors()
        {
            foreach (CyNukeManager mgr in MCUServices.Find.AllAuxCyclopsManagers<CyNukeManager>())
                mgr.SyncReactorsExternally();
        }

        public static void RemoveReactor(CyNukeReactorMono reactor)
        {
            foreach (CyNukeManager mgr in MCUServices.Find.AllAuxCyclopsManagers<CyNukeManager>())
                mgr.CyNukeReactors.Remove(reactor);
        }

        private readonly SubRoot Cyclops;

        private CyNukeEnhancerHandler upgradeHandler;
        private CyNukeEnhancerHandler CyNukeEnhancer => upgradeHandler ??
            (upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<CyNukeEnhancerHandler>(Cyclops, CyNukeEnhancerMk1.TechTypeID, CyNukeEnhancerMk2.TechTypeID));

        internal int UpgradeLevel => this.CyNukeEnhancer == null ? 0 : this.CyNukeEnhancer.HighestValue;

        public readonly List<CyNukeReactorMono> CyNukeReactors = new List<CyNukeReactorMono>();

        public CyNukeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public void SyncReactorsExternally()
        {
            var _tempCache = new List<CyNukeReactorMono>();

            CyNukeReactorMono[] foundReactors = Cyclops.GetAllComponentsInChildren<CyNukeReactorMono>();

            for (int r = 0; r < foundReactors.Length; r++)
            {
                CyNukeReactorMono reactor = foundReactors[r];
                if (_tempCache.Contains(reactor))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                _tempCache.Add(reactor);

                if (reactor.ParentCyclops == null)
                {
                    QuickLogger.Debug("Cyclops Nuclear Reactor synced externally");
                    // This is a workaround to get a reference to the Cyclops into the CyNukeReactorMono
                    reactor.ConnectToCyclops(Cyclops, this);
                }
            }
        }

        public void AddReactor(CyNukeReactorMono reactor)
        {
            if (!CyNukeReactors.Contains(reactor))
            {
                CyNukeReactors.Add(reactor);
            }
        }

        public bool Initialize(SubRoot cyclops)
        {
            return this.Cyclops == cyclops;
        }
    }
}
