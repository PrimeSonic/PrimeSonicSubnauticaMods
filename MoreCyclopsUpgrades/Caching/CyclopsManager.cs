namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.Monobehaviors;

    internal class CyclopsManager
    {
        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public List<CyBioReactorMono> BioReactors => this.PowerManager.CyBioReactors;

        public SubRoot Cyclops { get; private set; }

        public readonly int InstanceID;

        private CyclopsManager(SubRoot cyclops)
        {
            this.Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        public void Syncronize(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            this.UpgradeManager.SyncUpgradeConsoles();
            this.PowerManager.SyncBioReactors();
            this.PowerManager.UpdateConsoleHUD(hudManager);
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

        public static List<CyBioReactorMono> GetBioReactors(ItemsContainer container, SubRoot cyclops)
        {
            return GetManager(cyclops.GetInstanceID(), cyclops)?.BioReactors;
        }

        public static void SyncronizeAll(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            GetManager(hudManager.subRoot.GetInstanceID(), hudManager.subRoot)?.Syncronize(hudManager);
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

            var mgr = new CyclopsManager(cyclops);

            if (powerMgr.Initialize(mgr) && upgradeMgr.Initialize(mgr))
            {
                mgr.UpgradeManager = upgradeMgr;
                mgr.PowerManager = powerMgr;
                Managers.Add(mgr);

                return mgr;
            }

            return null;
        }
    }
}
