namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Buildables;

    internal class CyNukeManager : BuildableManager<CyNukeReactorMono>
    {
        public const int MaxReactors = CyNukeChargeManager.MaxReactors;

        private static IEnumerable<CyNukeManager> AllNukManagers => MCUServices.Find.AllAuxCyclopsManagers<CyNukeManager>();

        internal static void SyncAllReactors()
        {
            foreach (CyNukeManager mgr in AllNukManagers)
                mgr.SyncBuildables();
        }

        public static void RemoveReactor(CyNukeReactorMono reactor)
        {
            foreach (CyNukeManager mgr in AllNukManagers)
                mgr.CyNukeReactors.Remove(reactor);
        }

        public readonly List<CyNukeReactorMono> CyNukeReactors = new List<CyNukeReactorMono>();

        public CyNukeManager(SubRoot cyclops) : base(cyclops)
        {
        }

        public override bool Initialize(SubRoot cyclops)
        {
            return base.Cyclops == cyclops;
        }

        protected override void ConnectWithManager(CyNukeReactorMono buildable)
        {
            buildable.ConnectToCyclops(base.Cyclops, this);
        }
    }
}
