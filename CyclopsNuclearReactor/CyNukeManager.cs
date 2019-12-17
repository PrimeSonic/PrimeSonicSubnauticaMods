namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;

    internal class CyNukeManager : BuildableManager<CyNukeReactorMono>
    {
        private static IEnumerable<CyNukeManager> GetAllNukManagers()
        {
            return MCUServices.Find.AllAuxCyclopsManagers<CyNukeManager>();
        }

        internal static void SyncAllReactors()
        {
            IEnumerable<CyNukeManager> allMgrs = GetAllNukManagers();
            if (allMgrs == null)
                return;

            foreach (CyNukeManager mgr in allMgrs)
                mgr.SyncBuildables();
        }

        public static void RemoveReactor(CyNukeReactorMono reactor)
        {
            IEnumerable<CyNukeManager> allMgrs = GetAllNukManagers();
            if (allMgrs == null)
                return;

            foreach (CyNukeManager mgr in allMgrs)
                mgr.TrackedBuildables.Remove(reactor);
        }

        public float TotalEnergyCharge
        {
            get
            {
                float totalPower = 0f;
                for (int b = 0; b < TrackedBuildables.Count; b++)
                    totalPower += TrackedBuildables[b].GetTotalAvailablePower();

                return totalPower;
            }
        }

        public CyNukeManager(SubRoot cyclops) : base(cyclops)
        {
        }

        public override bool Initialize(SubRoot cyclops)
        {
            return base.Cyclops == cyclops;
        }

        public CyNukeReactorMono First => TrackedBuildables[0];
        public CyNukeReactorMono Second => TrackedBuildables[1];

        protected override void ConnectWithManager(CyNukeReactorMono buildable)
        {
            MCUServices.Logger.Debug("Connecting CyNukeReactorMono with Cyclops");
            buildable.ConnectToCyclops(base.Cyclops, this);
        }
    }
}
