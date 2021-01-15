namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.General;

    internal class CyclopsManager
    {
        #region Static Data

        private static readonly Dictionary<string, CreateAuxCyclopsManager> AuxManagerCreators = new Dictionary<string, CreateAuxCyclopsManager>();
        internal static List<CyclopsManager> Managers { get; } = new List<CyclopsManager>();

        #endregion

        #region Static Methods

        internal static bool TooLateToRegister => Managers.Count > 0;

        internal static void RegisterAuxManagerCreator(CreateAuxCyclopsManager createEvent, string typeName)
        {
            if (AuxManagerCreators.ContainsKey(typeName))
            {
                QuickLogger.Warning($"Block duplicate AuxManagerCreator '{typeName}'");
                return;
            }

            QuickLogger.Info($"Received AuxManagerCreator '{typeName}'");
            AuxManagerCreators.Add(typeName, createEvent);
        }

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

        internal static T GetManager<T>(ref SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            CyclopsManager mgr = GetManager(ref cyclops);

            if (mgr != null && mgr.AuxiliaryManagers.TryGetValue(auxManagerName, out IAuxCyclopsManager auxManager))
            {
                return (T)auxManager;
            }

            return CreateNewAuxManager<T>(mgr, auxManagerName);
        }

        internal static CyclopsManager GetManager(ref SubRoot cyclops)
        {
            if (cyclops == null || cyclops.isBase || !cyclops.isCyclops)
                return null;

            for (int m = 0; m < Managers.Count; m++)
            {
                CyclopsManager mgr = Managers[m];
                if (mgr.Cyclops == cyclops && mgr.InstanceID == cyclops.GetInstanceID())
                    return mgr;
            }

            return CreateNewCyclopsManager(ref cyclops);
        }

        private static CyclopsManager CreateNewCyclopsManager(ref SubRoot cyclops)
        {
            var mgr = new CyclopsManager(cyclops);
            Managers.Add(mgr);
            return mgr;
        }

        private static T CreateNewAuxManager<T>(CyclopsManager mgr, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            QuickLogger.Debug($"Started creating new IAuxCyclopsManager '{auxManagerName}'");
            if (AuxManagerCreators.TryGetValue(auxManagerName, out CreateAuxCyclopsManager creator))
            {
                IAuxCyclopsManager auxMgr = creator.Invoke(mgr.Cyclops);
                if (auxMgr != null || !auxMgr.Initialize(mgr.Cyclops))
                {
                    QuickLogger.Debug($"Created new IAuxCyclopsManager '{auxManagerName}'");
                    mgr.AuxiliaryManagers.Add(auxManagerName, auxMgr);
                    return (T)auxMgr;
                }
                else
                {
                    QuickLogger.Error($"Failed in creating IAuxCyclopsManager '{auxManagerName}'");
                }
            }

            QuickLogger.Warning($"Did not find creator method for IAuxCyclopsManager '{auxManagerName}'");
            return null;
        }

        #endregion

        #region Instance Members

        public readonly SubRoot Cyclops;
        public readonly int InstanceID;

        private readonly Dictionary<string, IAuxCyclopsManager> AuxiliaryManagers = new Dictionary<string, IAuxCyclopsManager>();

        internal readonly UpgradeManager Upgrade;
        internal readonly ChargeManager Charge;
        internal readonly CyclopsHUDManager HUD;
        internal readonly PowerRatingManager Engine;

        private CyclopsManager(SubRoot cyclops)
        {
            QuickLogger.Debug($"Creating new main CyclopsManager");
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();

            Upgrade = new UpgradeManager(cyclops);
            Charge = new ChargeManager(cyclops);
            HUD = new CyclopsHUDManager(cyclops);
            Engine = new PowerRatingManager(cyclops);
        }

        #endregion
    }
}
