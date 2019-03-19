namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using System.Collections.Generic;

    internal class BioBoosterUpgrade : CyclopsUpgrade
    {
        public BioBoosterUpgrade() : base(CyclopsModule.BioReactorBoosterID)
        {
            this.MaxCount = CyBioReactorMono.MaxBoosters;

            OnFinishedUpgrades = (SubRoot cyclops) =>
            {
                CyBioReactorMono lastRef = null;
                bool changedHappened = false;

                List<CyBioReactorMono> bioreactors = CyclopsManager.GetBioReactors(cyclops);

                foreach (CyBioReactorMono reactor in bioreactors)
                {
                    changedHappened |= (lastRef = reactor).UpdateBoosterCount(this.Count);
                }

                if (changedHappened && this.Count == CyBioReactorMono.MaxBoosters)
                {
                    ErrorMessage.AddMessage("Maximum boost to bioreactors achieved");
                }
            };
        }
    }
}
