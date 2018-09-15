namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;

    internal class CyclopsManager
    {
        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public SubRoot Cyclops { get; private set; }

        protected CyclopsManager(UpgradeManager upgradeManager, PowerManager powerManager, SubRoot cyclops)
        {
            this.UpgradeManager = upgradeManager;
            this.PowerManager = powerManager;
            this.Cyclops = cyclops;
        }

        private static IDictionary<string, CyclopsManager> Managers = new Dictionary<string, CyclopsManager>();

        public static CyclopsManager GetAllManagers(SubRoot cyclops) => GetManager(cyclops.GetComponent<PrefabIdentifier>().ClassId, cyclops);

        private static CyclopsManager GetManager(string id, SubRoot cyclops)
        {
            if (Managers.TryGetValue(id, out CyclopsManager mgr))
            {
                if (!ReferenceEquals(cyclops, mgr.Cyclops))
                {
                    mgr.Cyclops = cyclops;
                }

                return mgr;
            }

            return null;
        }

        public static void CreateNewManagers(SubRoot cyclops)
        {
            string id = cyclops.GetComponent<PrefabIdentifier>().ClassId;

            CyclopsManager existingMgr = GetManager(id, cyclops);

            if (existingMgr != null)
                return; // Already exists and now updated

            var upgradeMgr = new UpgradeManager();
            var powerMgr = new PowerManager();

            var mgr = new CyclopsManager(upgradeMgr, powerMgr, cyclops);

            upgradeMgr.Initialize(mgr);
            powerMgr.Initialize(mgr);

            Managers.Add(id, mgr);
        }

        public static UpgradeManager GetUpgradeManager(SubRoot cyclops) => GetManager(cyclops.GetComponent<PrefabIdentifier>().ClassId, cyclops)?.UpgradeManager;

        public static PowerManager GetPowerManager(SubRoot cyclops) => GetManager(cyclops.GetComponent<PrefabIdentifier>().ClassId, cyclops)?.PowerManager;
    }
}
