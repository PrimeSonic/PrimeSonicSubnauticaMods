namespace CyclopsBioReactor.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;

    internal class BioAuxCyclopsManager : BuildableManager<CyBioReactorMono>
    {
        #region Static Members

        private static IEnumerable<BioAuxCyclopsManager> GetAllBioManagers()
        {
            return MCUServices.Find.AllAuxCyclopsManagers<BioAuxCyclopsManager>();
        }

        internal static void SyncAllBioReactors()
        {
            IEnumerable<BioAuxCyclopsManager> allMgrs = GetAllBioManagers();
            if (allMgrs == null)
                return;

            foreach (BioAuxCyclopsManager mgr in allMgrs)
                mgr.SyncBuildables();
        }

        internal static void RemoveReactor(CyBioReactorMono cyBioReactorMono)
        {
            IEnumerable<BioAuxCyclopsManager> allMgrs = GetAllBioManagers();
            if (allMgrs == null)
                return;

            foreach (BioAuxCyclopsManager mgr in allMgrs)
                mgr.RemoveBuildable(cyBioReactorMono);
        }

        #endregion

        public BioAuxCyclopsManager(SubRoot cyclops) : base(cyclops)
        {
        }

        public override bool Initialize(SubRoot cyclops)
        {
            return base.Cyclops == cyclops;
        }

        protected override void ConnectWithManager(CyBioReactorMono buildable)
        {
            MCUServices.Logger.Debug("Connecting CyBioReactorMono with Cyclops");
            buildable.ConnectToCyclops(base.Cyclops, this);
        }

        public float TotalEnergyCharge
        {
            get
            {
                float totalPower = 0f;
                for (int b = 0; b < TrackedBuildables.Count; b++)
                    totalPower += TrackedBuildables[b].Charge;

                return totalPower;
            }
        }
    }
}
