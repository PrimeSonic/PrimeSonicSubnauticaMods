namespace MoreCyclopsUpgrades.API
{
    using MoreCyclopsUpgrades.Managers;

    public class MoreCyclopsUpgradesService : IMoreCyclopsUpgradesService
    {
        public static IMoreCyclopsUpgradesService ModClient { get; } = new MoreCyclopsUpgradesService();

        private MoreCyclopsUpgradesService()
        {
            // Hide constructor
        }

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        public void RegisterChargerCreator(ChargerCreator createEvent)
        {
            PowerManager.RegisterChargerCreator(createEvent);
        }

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        public void RegisterHandlerCreator(HandlerCreator createEvent)
        {
            UpgradeManager.RegisterHandlerCreator(createEvent);
        }
    }
}
