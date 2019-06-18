namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.Managers;

    internal class CyclopsManager
    {
        #region Static Members

        private static readonly ICollection<AuxManagerCreateEvent> AuxManagerCreators = new List<AuxManagerCreateEvent>();

        internal static void RegisterAuxManagerCreator(AuxManagerCreateEvent createEvent, string assemblyName)
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

            return null;
        }

        internal static CyclopsManager GetManager(SubRoot cyclops)
        {
            if (cyclops.isBase || !cyclops.isCyclops)
                return null;

            CyclopsManager mgr = Managers.Find(m => m.Cyclops == cyclops && m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManager(cyclops);
        }

        private static CyclopsManager CreateNewManager(SubRoot cyclops)
        {
            var mgr = new CyclopsManager(cyclops);

            foreach (IAuxCyclopsManager auxMgr in mgr.AuxiliaryManagers.Values)
            {
                bool success = auxMgr.Initialize(cyclops);

                if (!success)
                {
                    QuickLogger.Error($"Failed to initialize IAuxCyclopsManager {auxMgr.Name}", true);
                    return null;
                }

                QuickLogger.Debug($"Initialized IAuxCyclopsManager {auxMgr.Name}");
            }

            return mgr;
        }

        #endregion

        #region Instance Members

        public readonly SubRoot Cyclops;
        public readonly int InstanceID;

        internal readonly IDictionary<string, IAuxCyclopsManager> AuxiliaryManagers = new Dictionary<string, IAuxCyclopsManager>();

        // Because this is going to be called on every Update cycle, it's getting elevated privilege within the mod.
        internal ChargeManager QuickChargeManager;

        private CyclopsManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();

            foreach (AuxManagerCreateEvent creator in AuxManagerCreators)
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

                        if (QuickChargeManager == null && auxMgr is ChargeManager chargeManager)
                            QuickChargeManager = chargeManager;
                    }
                }
                else
                {
                    QuickLogger.Error($"Failed in creating IAuxCyclopsManager from '{creator.GetType().Assembly.GetName().Name}'");
                }
            }
        }

        #endregion
    }
}
