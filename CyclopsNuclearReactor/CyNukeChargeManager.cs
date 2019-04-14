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

            CyNukeChargeManager mgr = Managers.Find(m => m.InstanceID == cyclops.GetInstanceID());

            return mgr ?? CreateNewManager(cyclops);
        }

        private static CyNukeChargeManager CreateNewManager(SubRoot cyclops)
        {
            var mgr = new CyNukeChargeManager(cyclops);
            Managers.Add(mgr);
            return mgr;
        }

        #endregion

        public const int MaxReactors = 2; // TODO make configurable

        public readonly SubRoot Cyclops;
        public readonly int InstanceID;

        private readonly Atlas.Sprite indicatorSprite = SpriteManager.Get(CyNukReactorSMLHelper.TechTypeID);

        public readonly List<CyNukeReactorMono> CyNukeReactors = new List<CyNukeReactorMono>();
        private readonly List<CyNukeReactorMono> _tempCache = new List<CyNukeReactorMono>();

        public CyNukeChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            InstanceID = cyclops.GetInstanceID();
        }

        internal void SyncReactorsExternally()
        {
            _tempCache.Clear();

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

            if (_tempCache.Count != CyNukeReactors.Count)
            {
                CyNukeReactors.Clear();
                CyNukeReactors.AddRange(_tempCache);
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
            int count = 0;
            foreach (CyNukeReactorMono reactor in CyNukeReactors)
            {
                count++;
                if (count > MaxReactors)
                {
                    reactor.OverLimit = true;
                }
                else
                {
                    producedPower += reactor.ProducePower(ref powerDeficit);
                }
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
                return NumberFormatter.FormatNumber(Mathf.CeilToInt(CyNukeReactors[0].GetTotalAvailablePower()));

            string value = string.Empty;
            for (int i = 0; i < CyNukeReactors.Count; i++)
            {
                value += NumberFormatter.FormatNumber(Mathf.CeilToInt(CyNukeReactors[i].GetTotalAvailablePower())) + '\n';
            }

            return value;
        }

        public Color GetIndicatorTextColor()
        {
            return Color.white;
        }

        #endregion
    }
}
