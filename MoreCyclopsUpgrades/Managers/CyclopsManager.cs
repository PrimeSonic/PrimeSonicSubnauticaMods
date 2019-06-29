namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.Config;

    internal class CyclopsManager
    {
        #region Static Members

        internal static bool Initialized => Managers.Count > 0;

        private static readonly ICollection<CreateAuxCyclopsManager> AuxManagerCreators = new List<CreateAuxCyclopsManager>();

        internal static void RegisterAuxManagerCreator(CreateAuxCyclopsManager createEvent, string assemblyName)
        {
            if (AuxManagerCreators.Contains(createEvent))
            {
                QuickLogger.Warning($"Duplicate AuxManagerCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received AuxManagerCreator from {assemblyName}");
            AuxManagerCreators.Add(createEvent);
        }

        // List was chosen because of the very small number of entries it will have.
        private static readonly List<CyclopsManager> Managers = new List<CyclopsManager>();

        internal static IEnumerable<T> GetAllManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            foreach (CyclopsManager mgr in Managers)
            {
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

        internal static IEnumerable<CyclopsManager> GetAllManagers()
        {
            return Managers;
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

        // Because this is going to be called on every Update cycle, it's getting elevated privilege within the mod.
        internal readonly ChargeManager Charge;
        internal readonly UpgradeManager Upgrade;
        internal readonly CyclopsHUDManager HUD;
        internal readonly PowerRatingManager Engine;

        private CyclopsManager(SubRoot cyclops)
        {
            QuickLogger.Debug($"Creating main CyclopsManager");
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
            Charge = new ChargeManager(cyclops);
            Upgrade = new UpgradeManager(cyclops);
            HUD = new CyclopsHUDManager(cyclops, ModConfig.Main);
            Engine = new PowerRatingManager(cyclops);

            foreach (CreateAuxCyclopsManager creator in AuxManagerCreators)
            {
                IAuxCyclopsManager auxMgr = creator.Invoke(cyclops);
                if (auxMgr != null)
                {
                    if (string.IsNullOrEmpty(auxMgr.Name))
                    {
                        QuickLogger.Error($"Failed IAuxCyclopsManager with no name value from '{creator.GetType().Assembly.GetName().Name}'");
                    }
                    else
                    {
                        QuickLogger.Debug($"Created new IAuxCyclopsManager {auxMgr.Name}");
                        AuxiliaryManagers.Add(auxMgr.Name, auxMgr);
                    }
                }
                else
                {
                    QuickLogger.Error($"Failed in creating IAuxCyclopsManager from '{creator.GetType().Assembly.GetName().Name}'");
                }
            }
        }

        internal bool InitializeAuxiliaryManagers()
        {
            Charge.InitializeChargers();
            Upgrade.InitializeUpgradeHandlers();

            foreach (IAuxCyclopsManager auxMgr in AuxiliaryManagers.Values)
            {
                QuickLogger.Debug($"Initializing IAuxCyclopsManager {auxMgr.Name}");

                bool success = auxMgr.Initialize(Cyclops);

                if (!success)
                {
                    QuickLogger.Error($"Failed to initialize IAuxCyclopsManager {auxMgr.Name}", true);
                    return false;
                }

                QuickLogger.Debug($"Successfully initialized IAuxCyclopsManager {auxMgr.Name}");
            }

            return true;
        }

        #endregion
    }
}
