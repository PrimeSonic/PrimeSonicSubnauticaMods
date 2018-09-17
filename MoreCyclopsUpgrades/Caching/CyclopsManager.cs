namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;

    internal class CyclopsManager
    {
        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public SubRoot Cyclops { get; private set; }

        public readonly int InstanceID;

        protected CyclopsManager(UpgradeManager upgradeManager, PowerManager powerManager, SubRoot cyclops)
        {
            this.UpgradeManager = upgradeManager;
            this.PowerManager = powerManager;
            this.Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        // List was chosen because of the very small number of entries it will mamaged.
        private static List<CyclopsManager> Managers = new List<CyclopsManager>();

        public static CyclopsManager GetAllManagers(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops);

        public static UpgradeManager GetUpgradeManager(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops)?.UpgradeManager;

        public static PowerManager GetPowerManager(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops)?.PowerManager;

        private static CyclopsManager GetManager(int id, SubRoot cyclops)
        {
            CyclopsManager mgr = Managers.Find(m => m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManagers(cyclops);
        }

        private static CyclopsManager CreateNewManagers(SubRoot cyclops)
        {
            var upgradeMgr = new UpgradeManager();
            var powerMgr = new PowerManager();

            var mgr = new CyclopsManager(upgradeMgr, powerMgr, cyclops);

            upgradeMgr.Initialize(mgr);
            powerMgr.Initialize(mgr);

            Managers.Add(mgr);

            return mgr;
        }
    }
}
