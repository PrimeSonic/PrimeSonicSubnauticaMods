namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CyNukeChargeManager : ICyclopsCharger
    {
        #region Static Methods

        // This system was modeled after the CyclopsManager of MoreCyclopsUpgrades given its proven reliability

        internal static readonly List<CyNukeChargeManager> Managers = new List<CyNukeChargeManager>();

        internal static CyNukeChargeManager GetManager(SubRoot cyclops)
        {
            if (cyclops.isBase || !cyclops.isCyclops)
                return null;

            CyNukeChargeManager mgr = Managers.Find(m => m.Cyclops == cyclops && m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManager(cyclops);
        }

        private static CyNukeChargeManager CreateNewManager(SubRoot cyclops)
        {
            var mgr = new CyNukeChargeManager(cyclops);
            Managers.Add(mgr);
            mgr.SyncReactorsExternally();
            return mgr;
        }

        internal static List<CyNukeReactorMono> GetReactors(SubRoot cyclops)
        {
            CyNukeChargeManager mgr = GetManager(cyclops);

            return mgr.CyNukeReactors;
        }

        #endregion

        public const int MaxReactors = 2; // TODO make configurable

        public readonly SubRoot Cyclops;
        public readonly int InstanceID;

        private readonly Atlas.Sprite indicatorSprite = SpriteManager.Get(CyNukReactorBuildable.TechTypeID);

        public readonly List<CyNukeReactorMono> CyNukeReactors = new List<CyNukeReactorMono>();

        public void AddReactor(CyNukeReactorMono reactor)
        {
            if (!CyNukeReactors.Contains(reactor))
            {
                CyNukeReactors.Add(reactor);
            }
        }

        public CyNukeChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();

            QuickLogger.Debug($"Created new CyNukeChargeManager for Cyclops {InstanceID}");
        }

        internal void SyncReactorsExternally()
        {
            var _tempCache = new List<CyNukeReactorMono>();

            CyNukeReactorMono[] foundReactors = Cyclops.GetAllComponentsInChildren<CyNukeReactorMono>();

            foreach (CyNukeReactorMono reactor in foundReactors)
            {
                if (_tempCache.Contains(reactor))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                _tempCache.Add(reactor);

                if (reactor.ParentCyclops == null)
                {
                    QuickLogger.Debug("Cyclops Nuclear Reactor synced externally");
                    // This is a workaround to get a reference to the Cyclops into the CyNukeReactorMono
                    reactor.ConnectToCyclops(Cyclops, this);
                }
            }
        }

        internal static void SyncReactors()
        {
            foreach (CyNukeChargeManager mgr in Managers)
                mgr.SyncReactorsExternally();
        }

        #region ICyclopsCharger Methods

        public float ProducePower(float requestedPower)
        {
            if (CyNukeReactors.Count == 0)
                return 0f;

            float powerDeficit = requestedPower;
            float producedPower = 0f;

            foreach (CyNukeReactorMono reactor in CyNukeReactors)
            {
                if (!reactor.HasPower())
                    continue;

                producedPower += reactor.ProducePower(ref powerDeficit);
            }

            return producedPower;
        }

        public bool HasPowerIndicatorInfo()
        {
            if (CyNukeReactors.Count == 0)
                return false;

            foreach (CyNukeReactorMono reactor in CyNukeReactors)
            {
                if (reactor.HasPower())
                    return true;
            }

            return false;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return indicatorSprite;
        }

        public string GetIndicatorText()
        {
            if (CyNukeReactors.Count == 0)
                return string.Empty;

            if (CyNukeReactors.Count == 1)
                return CyNukeReactors[0].PowerIndicatorString();

            string value = string.Empty;

            for (int i = 0; i < CyNukeReactors.Count; i++)
                value += $"{CyNukeReactors[i].PowerIndicatorString()}\n";

            return value;
        }

        public Color GetIndicatorTextColor()
        {
            return Color.white;
        }

        #endregion
    }
}
