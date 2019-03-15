namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.Monobehaviors;

    internal class CyclopsManager
    {
        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public CyclopsHUDManager HUDManager { get; private set; }

        public List<CyBioReactorMono> BioReactors => this.PowerManager.CyBioReactors;

        public SubRoot Cyclops { get; }

        public readonly int InstanceID;

        private CyclopsManager(SubRoot cyclops)
        {
            this.Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        // List was chosen because of the very small number of entries it will mamaged.
        private static List<CyclopsManager> Managers = new List<CyclopsManager>();

        public static CyclopsManager GetAllManagers(SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops);
        }

        public static UpgradeManager GetUpgradeManager(SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops)?.UpgradeManager;
        }

        public static PowerManager GetPowerManager(SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops)?.PowerManager;
        }

        public static List<CyBioReactorMono> GetBioReactors(SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops)?.BioReactors;
        }

        public static CyclopsHUDManager GeHUDManager(SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops)?.HUDManager;
        }

        private static CyclopsManager GetManager(int id, SubRoot cyclops)
        {
            if (cyclops.isBase || !cyclops.isCyclops)
                return null;

            CyclopsManager mgr = Managers.Find(m => m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManagers(cyclops);
        }

        private static CyclopsManager CreateNewManagers(SubRoot cyclops)
        {
            var upgradeMgr = new UpgradeManager();
            var powerMgr = new PowerManager();
            var hudManager = new CyclopsHUDManager();

            var mgr = new CyclopsManager(cyclops)
            {
                UpgradeManager = upgradeMgr,
                PowerManager = powerMgr,
                HUDManager = hudManager
            };

            if (upgradeMgr.Initialize(mgr) && powerMgr.Initialize(mgr) && hudManager.Initialize(mgr))
            {                
                Managers.Add(mgr);

                return mgr;
            }

            return null;
        }

        public static void SyncUpgradeConsoles()
        {
            foreach (CyclopsManager mgr in Managers)
                mgr.UpgradeManager.SyncUpgradeConsoles();
        }

        public static void SyncBioReactors()
        {
            foreach (CyclopsManager mgr in Managers)
                mgr.PowerManager.SyncBioReactors();
        }
    }
}
