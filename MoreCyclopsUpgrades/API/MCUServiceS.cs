namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.Managers;

    public interface IMCUServices
    {
        /// <summary>
        /// Registers a <see cref="AuxManagerCreator"/> method that creates returns a new <see cref="IAuxCyclopsManager"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.<para/>
        /// Auxilary managers are always created first.
        /// </summary>
        /// <param name="createEvent">The create event.</param>
        void RegisterAuxManagerCreators(AuxManagerCreator createEvent);

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        void RegisterChargerCreator(ChargerCreator createEvent);

        /// <summary>
        /// Registers a <see cref="HandlerCreator"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
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

        /// <summary>
        /// Gets the typed <see cref="IAuxCyclopsManager"/> that was created for the specified Cyclops sub.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="cyclops">The managed cyclops.</param>
        /// <param name="auxManagerName">Name you defined for the auxilary cyclops manager.</param>
        /// <returns>A type casted <see cref="IAuxCyclopsManager"/> if found by name; Otherwise returns null if not found.</returns>
        /// <seealso cref="AuxManagerCreator"/>
        T GetManager<T>(SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets all typed <see cref="IAuxCyclopsManager"/>s across all Cyclops subs.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="auxManagerName">Name you defined for the auxilary cyclops manager.</param>
        /// <returns>A type casted enumeration of all <see cref="IAuxCyclopsManager"/>s found across all Cyclops subs, identified by name.</returns>
        IEnumerable<T> GetAllManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager;
    }

    /// <summary>
    /// The main entry point for all API services provided by MoreCyclopsUpgrades.
    /// </summary>
    /// <seealso cref="IMCUServices" />
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
            ChargeManager.RegisterChargerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void RegisterHandlerCreator(HandlerCreator createEvent)
        {
            UpgradeManager.RegisterHandlerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void RegisterAuxManagerCreators(AuxManagerCreator createEvent)
        {
            CyclopsManager.RegisterAuxManagerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public T GetManager<T>(SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetManager<T>(cyclops, auxManagerName);
        }

        public IEnumerable<T> GetAllManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetAllManagers<T>(auxManagerName);
        }

        public bool CyclopsFabricatorHasCyclopsModulesTab { get; private set; } = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

        public string[] StepsToCyclopsModulesTab { get; } = new[] { "CyclopsModules" };
    }
}
