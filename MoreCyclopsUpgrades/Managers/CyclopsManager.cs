namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.General;

    internal class CyclopsManager
    {
        #region Static Members

        internal static bool Initialized => Managers.Count > 0;

        private static readonly IDictionary<CreateAuxCyclopsManager, string> AuxManagerCreators = new Dictionary<CreateAuxCyclopsManager, string>();

        internal static void RegisterAuxManagerCreator(CreateAuxCyclopsManager createEvent, string typeName)
        {
            if (AuxManagerCreators.ContainsKey(createEvent))
            {
                QuickLogger.Warning($"Block duplicate AuxManagerCreator '{typeName}'");
                return;
            }

            QuickLogger.Info($"Received AuxManagerCreator '{typeName}'");
            AuxManagerCreators.Add(createEvent, typeName);
        }

        // List was chosen because of the very small number of entries it will have.
        internal static List<CyclopsManager> Managers { get; } = new List<CyclopsManager>();

        internal static IEnumerable<T> GetAllManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            for (int i = 0; i < Managers.Count; i++)
            {
                CyclopsManager mgr = Managers[i];
                if (mgr != null && mgr.AuxiliaryManagers.TryGetValue(auxManagerName, out IAuxCyclopsManager auxManager))
                {
                    yield return (T)auxManager;
                }
            }
        }

        internal static T GetManager<T>(SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            CyclopsManager mgr = GetManager(cyclops);

            if (mgr != null && mgr.AuxiliaryManagers.TryGetValue(auxManagerName, out IAuxCyclopsManager auxManager))
            {
                return (T)auxManager;
            }

            QuickLogger.Warning($"Did not find IAuxCyclopsManager '{auxManagerName}'");
            return null;
        }

        internal static CyclopsManager GetManager(SubRoot cyclops)
        {
            if (cyclops.isBase || !cyclops.isCyclops)
                return null;

            CyclopsManager mgr = Managers.Find(m => m.Cyclops == cyclops && m.InstanceID == cyclops.GetInstanceID());

            if (mgr == null)
            {
                mgr = new CyclopsManager(cyclops);
                Managers.Add(mgr);

                if (mgr.InitializeAuxiliaryManagers())
                    return mgr;
                else
                {
                    Managers.Remove(mgr);
                    return null;
                }
            }

            return mgr;
        }

        #endregion

        #region Instance Members

        public readonly SubRoot Cyclops;
        public readonly int InstanceID;

        internal readonly IDictionary<string, IAuxCyclopsManager> AuxiliaryManagers = new Dictionary<string, IAuxCyclopsManager>();

        internal readonly UpgradeManager Upgrade;
        internal readonly ChargeManager Charge;
        internal readonly CyclopsHUDManager HUD;
        internal readonly PowerRatingManager Engine;

        private CyclopsManager(SubRoot cyclops)
        {
            QuickLogger.Debug($"Creating main CyclopsManager");
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();

            Upgrade = new UpgradeManager(cyclops);
            Charge = new ChargeManager(cyclops);
            HUD = new CyclopsHUDManager(cyclops);
            Engine = new PowerRatingManager(cyclops);

            foreach (KeyValuePair<CreateAuxCyclopsManager, string> creatorPair in AuxManagerCreators)
            {
                CreateAuxCyclopsManager creator = creatorPair.Key;
                string name = creatorPair.Value;
                IAuxCyclopsManager auxMgr = creator.Invoke(cyclops);
                if (auxMgr != null)
                {

                    QuickLogger.Debug($"Created new IAuxCyclopsManager '{name}'");
                    AuxiliaryManagers.Add(name, auxMgr);
                }
                else
                {
                    QuickLogger.Error($"Failed in creating IAuxCyclopsManager from '{creator.GetType().Assembly.GetName().Name}'");
                }
            }
        }

        internal bool InitializeAuxiliaryManagers()
        {
            Upgrade.InitializeUpgradeHandlers();
            Charge.InitializeChargers();

            foreach (KeyValuePair<string, IAuxCyclopsManager> auxMgrPair in AuxiliaryManagers)
            {
                string name = auxMgrPair.Key;
                IAuxCyclopsManager auxMgr = auxMgrPair.Value;
                QuickLogger.Debug($"Initializing IAuxCyclopsManager {name}");

                bool success = auxMgr.Initialize(Cyclops);

                if (!success)
                {
                    QuickLogger.Error($"Failed to initialize IAuxCyclopsManager {name}", true);
                    return false;
                }

                QuickLogger.Debug($"Successfully initialized IAuxCyclopsManager {name}");
            }

            return true;
        }

        #endregion
    }
}
