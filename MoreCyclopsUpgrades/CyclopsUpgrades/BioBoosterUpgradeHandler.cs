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

        public BioBoosterUpgradeHandler(SubRoot cyclops) : base(CyclopsModule.BioReactorBoosterID, cyclops)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedUpgrades += () =>
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

            IsAllowedToRemove += (Pickupable item, bool verbose) =>
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
