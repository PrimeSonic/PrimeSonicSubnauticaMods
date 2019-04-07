namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.Monobehaviors;
    using System.Collections.Generic;

    internal class CyclopsManager
    {
        public readonly UpgradeManager UpgradeManager;

        public readonly PowerManager PowerManager;

        public readonly CyclopsHUDManager HUDManager;

        public List<CyBioReactorMono> BioReactors => PowerManager.CyBioReactors;

        public readonly SubRoot Cyclops;

        public readonly int InstanceID;

        public CyclopsManager(SubRoot cyclops, UpgradeManager upgradeManager, PowerManager powerManager, CyclopsHUDManager hUDManager)
        {
            UpgradeManager = upgradeManager;
            PowerManager = powerManager;
            HUDManager = hUDManager;
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        // List was chosen because of the very small number of entries it will have.
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

        public static CyclopsHUDManager GetHUDManager(SubRoot cyclops)
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
            var upgradeMgr = new UpgradeManager(cyclops);
            var powerMgr = new PowerManager(cyclops);
            var hudManager = new CyclopsHUDManager(cyclops);

            var mgr = new CyclopsManager(cyclops, upgradeMgr, powerMgr, hudManager);

            Managers.Add(mgr);

            // Managers must be initialized in this order
            if (!upgradeMgr.Initialize(mgr) ||
                !powerMgr.Initialize(mgr) ||
                !hudManager.Initialize(mgr))
            {
                QuickLogger.Error("Failed to initialized manager", true);
                Managers.Remove(mgr);
                return null;
            }

            return mgr;
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
