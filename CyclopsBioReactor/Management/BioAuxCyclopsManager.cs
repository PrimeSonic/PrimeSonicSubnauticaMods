namespace CyclopsBioReactor.Management
{
    using System.Collections.Generic;
    using Common;
    using CommonCyclopsBuildables;
    using MoreCyclopsUpgrades.API;

    internal class BioAuxCyclopsManager : BuildableManager<CyBioReactorMono>
    {
        #region Static Members

        private static IEnumerable<BioAuxCyclopsManager> AllBioManagers => MCUServices.Find.AllAuxCyclopsManagers<BioAuxCyclopsManager>();

        internal static void SyncAllBioReactors()
        {
            foreach (BioAuxCyclopsManager mgr in AllBioManagers)
                mgr.SyncBuildables();
        }

        internal static void RemoveReactor(CyBioReactorMono cyBioReactorMono)
        {
            foreach (BioAuxCyclopsManager mgr in AllBioManagers)
                mgr.RemoveBuildable(cyBioReactorMono);
        }

        internal const int MaxBioReactors = BioChargeHandler.MaxBioReactors;

        #endregion

        internal readonly TechType CyBioBoosterID;
        internal readonly TechType CyBioReactorID;

        public BioAuxCyclopsManager(SubRoot cyclops, TechType bioBooster, TechType bioReactor) : base(cyclops)
        {
            CyBioBoosterID = bioBooster;
            CyBioReactorID = bioReactor;
        }

        public override bool Initialize(SubRoot cyclops)
        {
            return
                Cyclops == cyclops &&
                CyBioBoosterID != TechType.None &&
                CyBioReactorID != TechType.None;
        }

        protected override void ConnectWithManager(CyBioReactorMono buildable)
        {
            QuickLogger.Debug("Connecting CyBioReactorMono with Cyclops");
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
