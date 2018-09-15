namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;

    internal class CyclopsManager
    {
        private static IDictionary<string, CyclopsManager> Managers = new Dictionary<string, CyclopsManager>();

        protected CyclopsManager(UpgradeManager upgradeManager, PowerManager powerManager, SubRoot cyclops)
        {
            this.UpgradeManager = upgradeManager;
            this.PowerManager = powerManager;
            this.Cyclops = cyclops;
        }

        public static CyclopsManager GetManager(SubRoot cyclops)
        {
            string id = cyclops.GetComponent<PrefabIdentifier>().ClassId;

            if (Managers.TryGetValue(id, out var mgr))
            {
                if (!ReferenceEquals(cyclops, mgr.Cyclops))
                {
                    mgr.Cyclops = cyclops;
                    mgr.UpgradeManager.Cyclops = cyclops;
                    mgr.PowerManager.Cyclops = cyclops;
                }

                return mgr;
            }

            return null;
        }

        public static void CreateNewManagers(SubRoot cyclops)
        {
            var existingMgr = GetManager(cyclops);

            if (existingMgr != null)
                return; // Already exists and now updated

            string id = cyclops.GetComponent<PrefabIdentifier>().ClassId;

            var upgradeMgr = new UpgradeManager();
            upgradeMgr.Initialize(cyclops);

            var powerMgr = new PowerManager();
            powerMgr.Initialize(cyclops, upgradeMgr);

            Managers.Add(id, new CyclopsManager(upgradeMgr, powerMgr, cyclops));
        }

        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public SubRoot Cyclops { get; private set; }


    }
}
