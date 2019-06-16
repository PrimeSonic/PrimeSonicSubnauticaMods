namespace MoreCyclopsUpgrades.API
{
    using System.IO;
    using MoreCyclopsUpgrades.Managers;

    public interface IMCUServices
    {
        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand.<para/>
        /// /// This method is only invoked once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        void RegisterChargerCreator(ChargerCreator createEvent);

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand.<para/>
        /// This method is only invoked once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        void RegisterHandlerCreator(HandlerCreator createEvent);

        /// <summary>
        /// Gets a value indicating whether the Cyclops Fabricator has a "CyclopsModules" crafting tab.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the "CyclopsModules" tab has been added to the Cyclops Fabricator; otherwise, <c>false</c>.
        /// </value>
        bool CyclopsFabricatorHasCyclopsModulesTab { get; }

        /// <summary>
        /// Gets the steps to "CyclopsModules" crafting tab in the Cyclops Fabricator.<para/>
        /// Use only if <see cref="CyclopsFabricatorHasCyclopsModulesTab"/> is <c>true</c>.
        /// </summary>
        /// <value>
        /// The steps to  the Cyclops Fabricator's "CyclopsModules" crafting tab.
        /// </value>
        string[] StepsToCyclopsModulesTab { get; }
    }

    public class MCUServices : IMCUServices
    {
        public static IMCUServices Client { get; } = new MCUServices();

        /// <summary>
        /// "Practically zero" for all intents and purposes.<para/>
        ///  Any energy value lower than this should be considered zero.
        /// </summary>
        public const float MinimalPowerValue = 0.001f;

        private MCUServices()
        {
            // Hide constructor
        }

        public void RegisterChargerCreator(ChargerCreator createEvent)
        {
            ChargeManager.RegisterChargerCreator(createEvent);
        }

        public void RegisterHandlerCreator(HandlerCreator createEvent)
        {
            UpgradeManager.RegisterHandlerCreator(createEvent);
        }

        public bool CyclopsFabricatorHasCyclopsModulesTab { get; private set; } = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

        public string[] StepsToCyclopsModulesTab { get; } = new[] { "CyclopsModules" };
    }
}
