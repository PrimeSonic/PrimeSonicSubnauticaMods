namespace CyclopsBioReactor.Management
{
    using Common;
    using CyclopsBioReactor.Items;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal partial class BioManager : UpgradeHandler
    {
        private float errorDelay = 0f;
        private const float delayInterval = 10f;

        internal int TotalBoosters => this.Count;

        public BioManager(SubRoot cyclops)
            : base(CyBioBoosterID, cyclops)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedWithUpgrades += () =>
            {
                QuickLogger.Debug($"Handling BioBooster at {this.Count}");
                foreach (CyBioReactorMono reactor in CyBioReactors)
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
                foreach (CyBioReactorMono reactor in CyBioReactors)
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
