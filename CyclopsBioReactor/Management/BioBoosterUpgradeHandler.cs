namespace CyclopsBioReactor.Management
{
    using Common;
    using CyclopsBioReactor.Items;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class BioBoosterUpgradeHandler : UpgradeHandler
    {
        private float errorDelay = 0f;
        private const float delayInterval = 10f;
        internal int TotalBoosters => this.Count;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(base.Cyclops));

        public BioBoosterUpgradeHandler(TechType cyBioBooster, SubRoot cyclops)
            : base(cyBioBooster, cyclops)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedUpgrades = () =>
            {
                QuickLogger.Debug($"Handling BioBooster at {this.Count}");

                for (int b = 0; b < this.Manager.CyBioReactors.Count; b++)
                    this.Manager.CyBioReactors[b].UpdateBoosterCount(this.Count);
            };

            OnFirstTimeMaxCountReached = () =>
            {
                ErrorMessage.AddMessage(BioReactorBooster.MaxBoostAchived);
            };

            IsAllowedToRemove = (Pickupable item, bool verbose) =>
            {
                for (int b = 0; b < this.Manager.CyBioReactors.Count; b++)
                {
                    if (!this.Manager.CyBioReactors[b].HasRoomToShrink())
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
