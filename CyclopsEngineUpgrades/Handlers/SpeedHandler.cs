namespace CyclopsEngineUpgrades.Handlers
{
    using CyclopsEngineUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;

    internal class SpeedHandler : UpgradeHandler
    {
        private readonly CyclopsSpeedModule speedModule;
        private readonly EngineManager powerManager;

        public SpeedHandler(CyclopsSpeedModule cyclopsSpeedModule, SubRoot cyclops) : base(cyclopsSpeedModule.TechType, cyclops)
        {
            powerManager = MCUServices.Client.FindManager<EngineManager>(cyclops, EngineManager.ManagerName);
            powerManager.SpeedBoosters = this;
            speedModule = cyclopsSpeedModule;            
            this.MaxCount = EngineManager.MaxSpeedBoosters;

            OnFirstTimeMaxCountReached = () =>
            {
                ErrorMessage.AddMessage(CyclopsSpeedModule.MaxRatingAchived);
            };
            OnFinishedWithUpgrades = () =>
            {
                powerManager.UpdatePowerSpeedRating();
            };
        }
    }
}
