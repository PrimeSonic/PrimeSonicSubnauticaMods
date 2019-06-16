namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using Common;

    internal class CyclopsManager
    {
        #region Static Members

        private static readonly ICollection<AuxManagerCreator> AuxManagerCreators = new List<AuxManagerCreator>();

        internal static void RegisterAuxManagerCreator(AuxManagerCreator createEvent, string assemblyName)
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
            CyclopsManager mgr = GetManager(cyclops.GetInstanceID(), cyclops);

            if (mgr != null && mgr.AuxiliaryManagers.TryGetValue(auxManagerName, out IAuxCyclopsManager auxManager))
            {
                return (T)auxManager;
            }

            return null;
        }

        private static CyclopsManager GetManager(int id, SubRoot cyclops)
        {
            if (cyclops.isBase || !cyclops.isCyclops)
                return null;

            CyclopsManager mgr = Managers.Find(m => m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManager(cyclops);
        }

        private static CyclopsManager CreateNewManager(SubRoot cyclops)
        {
            var mgr = new CyclopsManager(cyclops);

            foreach (KeyValuePair<string, IAuxCyclopsManager> auxMgr in mgr.AuxiliaryManagers)
            {
                bool success = auxMgr.Value.Initialize(cyclops);

                if (!success)
                {
                    QuickLogger.Error($"Failed to initialized manager {auxMgr.Key}", true);
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

        private CyclopsManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();

            foreach (AuxManagerCreator creator in AuxManagerCreators)
            {
                IAuxCyclopsManager auxMgr = creator.Invoke(cyclops);
                AuxiliaryManagers.Add(auxMgr.Name, auxMgr);
            }
        }

        #endregion
    }
}
