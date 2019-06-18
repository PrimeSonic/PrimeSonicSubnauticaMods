namespace CyclopsBioReactor.Management
{
    using Common;
    using CyclopsBioReactor.Items;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal class BioBoosterUpgradeHandler : UpgradeHandler
    {
        private float errorDelay = 0f;
        private const float delayInterval = 10f;
        internal int TotalBoosters => this.Count;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Client.FindManager<BioAuxCyclopsManager>(cyclops, BioAuxCyclopsManager.ManagerName));

        public BioBoosterUpgradeHandler(TechType cyBioBooster, SubRoot cyclops)
            : base(cyBioBooster, cyclops)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedWithUpgrades += () =>
            {
                QuickLogger.Debug($"Handling BioBooster at {this.Count}");
                foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
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
                foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
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
