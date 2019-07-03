namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class CyNukeChargeManager : ICyclopsCharger
    {
        public const int MaxReactors = 2;

        public readonly SubRoot Cyclops;

        private CyNukeEnhancerHandler upgradeHandler;
        private CyNukeEnhancerHandler CyNukeEnhancer => upgradeHandler ?? 
            (upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<CyNukeEnhancerHandler>(Cyclops, CyNukeEnhancerMk1.TechTypeID, CyNukeEnhancerMk2.TechTypeID));

        internal int UpgradeLevel => this.CyNukeEnhancer == null ? 0 : this.CyNukeEnhancer.HighestValue;

        private readonly Atlas.Sprite indicatorSprite = SpriteManager.Get(SpriteManager.Group.Category, CyNukReactorBuildable.PowerIndicatorIconID);

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
            foreach (CyNukeChargeManager mgr in MCUServices.Find.AllCyclopsChargers<CyNukeChargeManager>())
                mgr.SyncReactorsExternally();
        }

        public static void RemoveReactor(CyNukeReactorMono reactor)
        {
            foreach (CyNukeChargeManager mgr in MCUServices.Find.AllCyclopsChargers<CyNukeChargeManager>())
            {
                mgr.CyNukeReactors.Remove(reactor);
            }
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

            if (CyNukeReactors.Count == 2)
                return $"{CyNukeReactors[0].PowerIndicatorString()}\n{CyNukeReactors[1].PowerIndicatorString()}";

            string value = string.Empty;
            for (int i = 0; i < CyNukeReactors.Count; i++)
                value += $"{CyNukeReactors[i].PowerIndicatorString()}\n";

            return value;
        }

        public Color GetIndicatorTextColor()
        {
            if (CyNukeReactors.Count == 0)
                return Color.white;

            int totalActiveRods = 0;
            int maxRods = 0;

            foreach (CyNukeReactorMono reactor in CyNukeReactors)
            {
                totalActiveRods += reactor.ActiveRodCount;
                maxRods += reactor.MaxActiveSlots;
            }

            // All slots active
            if (totalActiveRods == maxRods)
                return Color.green;

            // No slots active
            if (totalActiveRods == 0)
                return Color.white;

            // Some slots depleted
            return Color.yellow;
        }

        public float TotalReservePower()
        {
            float total = 0f;
            foreach (CyNukeReactorMono reactor in CyNukeReactors)
            {
                total += reactor.GetTotalAvailablePower();
            }
            return total;
        }

        #endregion
    }
}
