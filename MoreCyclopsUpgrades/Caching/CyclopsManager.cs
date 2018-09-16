namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;
    using Common;

    internal class CyclopsManager
    {
        public UpgradeManager UpgradeManager { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public SubRoot Cyclops { get; private set; }

        public readonly int InstanceID;

        public int Calls { get; private set; } = 0;

        protected CyclopsManager(UpgradeManager upgradeManager, PowerManager powerManager, SubRoot cyclops)
        {
            this.UpgradeManager = upgradeManager;
            this.PowerManager = powerManager;
            this.Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        private const int TimeToCheck = 1200;
        private static int CallCounter = 0;

        // List was chosen because of the very small number of entries it will mamaged.
        private static List<CyclopsManager> Managers = new List<CyclopsManager>(3);

        public static CyclopsManager GetAllManagers(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops);

        private static CyclopsManager GetManager(int id, SubRoot cyclops)
        {
            HandlePurging();

            CyclopsManager mgr = Managers.Find(m => m.InstanceID == cyclops.GetInstanceID());

            if (mgr != null)
            {
                if (Managers.Count > 1)
                    mgr.Calls++;

                return mgr;
            }

            return null;
        }

        private static void HandlePurging()
        {
            if (Managers.Count < 2)
                return;

            CallCounter++;

            if (CallCounter < TimeToCheck)
                return;

            CallCounter = 0;

            CyclopsManager unused = Managers.Find(m => m.Calls == 0);

            if (unused != null)
            {
                Managers.Remove(unused);
                QuickLogger.Debug("Unsued SubRoot reference removed", true);
            }
        }

        public static void CreateNewManagers(SubRoot cyclops)
        {
            var upgradeMgr = new UpgradeManager();
            var powerMgr = new PowerManager();

            var mgr = new CyclopsManager(upgradeMgr, powerMgr, cyclops);

            upgradeMgr.Initialize(mgr);
            powerMgr.Initialize(mgr);

            Managers.Add(mgr);
        }

        public static UpgradeManager GetUpgradeManager(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops)?.UpgradeManager;

        public static PowerManager GetPowerManager(SubRoot cyclops) => GetManager(cyclops.GetInstanceID(), cyclops)?.PowerManager;
    }
}
