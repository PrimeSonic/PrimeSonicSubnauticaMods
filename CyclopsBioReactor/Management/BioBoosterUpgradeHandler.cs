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

                this.Manager.ApplyToAll((CyBioReactorMono reactor) => reactor.UpdateBoosterCount(this.Count));
            };

            OnFirstTimeMaxCountReached = () =>
            {
                ErrorMessage.AddMessage(BioReactorBooster.MaxBoostAchived);
            };

            IsAllowedToRemove = (Pickupable item, bool verbose) =>
            {
                return this.Manager.FindFirst(false, (CyBioReactorMono reactor) => reactor.HasRoomToShrink(), () =>
                {
                    if (Time.time > errorDelay)
                    {
                        errorDelay = Time.time + delayInterval;
                        ErrorMessage.AddMessage(BioReactorBooster.CannotRemove);
                    }
                });
            };
        }
    }
}
