namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Common;
    using Managers;
    using Modules;
    using Modules.Enhancement;
    using Monobehaviors;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioBoosterUpgradeHandler : UpgradeHandler
    {
        private float errorDelay = 0f;
        private const float delayInterval = 10f;
        internal readonly int MaxBioreactorsAllowed;

        public BioBoosterUpgradeHandler(int maxBioreactorsAllowed) : base(CyclopsModule.BioReactorBoosterID)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;
            MaxBioreactorsAllowed = maxBioreactorsAllowed;

            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                List<CyBioReactorMono> bioreactors = CyclopsManager.GetBioReactors(cyclops);

                if (bioreactors == null)
                    return;

                QuickLogger.Debug($"Handling BioBooster at {this.Count}");
                foreach (CyBioReactorMono reactor in bioreactors)
                {
                    reactor.UpdateBoosterCount(this.Count);
                }
            };

            OnFirstTimeMaxCountReached += () =>
            {
                ErrorMessage.AddMessage(BioReactorBooster.MaxBoostAchived);
            };

            IsAllowedToRemove += (SubRoot cyclops, Pickupable item, bool verbose) =>
            {
                List<CyBioReactorMono> bioreactors = CyclopsManager.GetBioReactors(cyclops);

                if (bioreactors == null)
                    return true;

                foreach (CyBioReactorMono reactor in bioreactors)
                {
                    if (!reactor.HasRoomToShrink())
                    {
                        if (Time.time > errorDelay)
                        {
                            errorDelay = Time.time + delayInterval;
                            ErrorMessage.AddMessage(BioReactorBooster.CannotRemove);
                        }

                        return false;
                    }
                }

                return true;
            };
        }
    }
}
