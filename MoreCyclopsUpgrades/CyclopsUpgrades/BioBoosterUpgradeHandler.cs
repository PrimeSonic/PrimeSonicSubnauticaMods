namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using System.Collections.Generic;

    internal class BioBoosterUpgradeHandler : UpgradeHandler
    {
        public BioBoosterUpgradeHandler() : base(CyclopsModule.BioReactorBoosterID)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                List<CyBioReactorMono> bioreactors = CyclopsManager.GetBioReactors(cyclops);

                foreach (CyBioReactorMono reactor in bioreactors)
                {
                    reactor.UpdateBoosterCount(this.Count);
                }
            };

            OnFirstTimeMaxCountReached += () =>
            {
                ErrorMessage.AddMessage("Maximum boost to bioreactors achieved");
            };
        }
    }
}
