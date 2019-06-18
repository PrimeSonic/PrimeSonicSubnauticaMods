namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.Managers;

    public interface IMCURegistration
    {
        /// <summary>
        /// Registers a <see cref="AuxManagerCreateEvent"/> method that creates returns a new <see cref="IAuxCyclopsManager"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.<para/>
        /// Auxilary managers are always created first.
        /// </summary>
        /// <param name="createEvent">The create event.</param>
        void AuxCyclopsManager(AuxManagerCreateEvent createEvent);

        /// <summary>
        /// Registers a <see cref="CyclopsChargerCreateEvent"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="CyclopsChargerCreateEvent"/>.</param>
        void CyclopsCharger(CyclopsChargerCreateEvent createEvent);

        /// <summary>
        /// Registers a <see cref="UpgradeHandlerCreateEvent"/> method that creates returns a new <see cref="UpgradeHandler"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        void CyclopsUpgradeHandler(UpgradeHandlerCreateEvent createEvent);
    }

    public interface IMCUSearch
    {
        /// <summary>
        /// Gets the typed <see cref="IAuxCyclopsManager"/> at the specified Cyclops sub with the given <seealso cref="IAuxCyclopsManager.Name"/>.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="auxManagerName">The <seealso cref="IAuxCyclopsManager.Name"/> you defined for the auxilary cyclops manager.</param>
        /// <returns>A type casted <see cref="IAuxCyclopsManager"/> if found by name; Otherwise returns null if not found.</returns>
        /// <seealso cref="AuxManagerCreateEvent"/>
        T AuxCyclopsManager<T>(SubRoot cyclops, string auxManagerName) where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets all typed <see cref="IAuxCyclopsManager"/>s across all Cyclops subs with the given <seealso cref="IAuxCyclopsManager.Name"/>.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="auxManagerName">The <seealso cref="IAuxCyclopsManager.Name"/> you defined for the auxilary cyclops manager.</param>
        /// <returns>A type casted enumeration of all <see cref="IAuxCyclopsManager"/>s found across all Cyclops subs, identified by name.</returns>
        IEnumerable<T> FindAllAuxCyclopsManagers<T>(string auxManagerName) where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets the charge hangler at the specified Cyclops sub for the provided <seealso cref="ICyclopsCharger.Name"/> string.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="ICyclopsCharger"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CyclopsChargerCreateEvent"/> you passed into <seealso cref="RegisterChargerCreator(CyclopsChargerCreateEvent)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="chargeHandlerName">The <seealso cref="ICyclopsCharger.Name"/> of the charge handler.</param>
        /// <returns>A type casted <see cref="ICyclopsCharger"/> if found by name; Otherwise returns null.</returns>
        T CyclopsCharger<T>(SubRoot cyclops, string chargeHandlerName) where T : class, ICyclopsCharger;

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="UpgradeHandlerCreateEvent"/> you passed into <seealso cref="RegisterUpgradeCreator(UpgradeHandlerCreateEvent)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>A type casted <see cref="UpgradeHandler"/> if found by techtype; Otherwise returns null.</returns>
        T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler;

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="StackingGroupHandler"/> or <seealso cref="TieredGroupHandler{T}"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="UpgradeHandlerCreateEvent"/> you passed into <seealso cref="RegisterUpgradeCreator(UpgradeHandlerCreateEvent)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>A type casted <see cref="UpgradeHandler"/> if found by techtype; Otherwise returns null.</returns>
        T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler;
    }

    public interface IMCUCrossMod
    {
        /// <summary>
        /// Gets the steps to "CyclopsModules" crafting tab in the Cyclops Fabricator.<para/>
        /// This would be necessary for best cross-compatibility with the [VehicleUpgradesInCyclops] mod.<para/>
        /// Will return null if this mod isn't present, under the assumption that this mod isn't present and it is otherwise find to add crafting nodes to the Cyclops Fabricator root.
        /// </summary>
        /// <value>
        /// The steps to the Cyclops Fabricator's "CyclopsModules" crafting tab if it exists.
        /// </value>
        string[] StepsToCyclopsModulesTabInCyclopsFabricator { get; }        
    }

    /// <summary>
    /// The main entry point for all API services provided by MoreCyclopsUpgrades.
    /// </summary>
    /// <seealso cref="IMCUCrossMod" />
    public class MCUServices : IMCUCrossMod, IMCURegistration, IMCUSearch
    {
        private static readonly MCUServices singleton = new MCUServices();

        private static readonly string[] cyModulesTab = new[] { "CyclopsModules" };
        private bool CyclopsFabricatorHasCyclopsModulesTab { get; } = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

        public string[] StepsToCyclopsModulesTabInCyclopsFabricator => this.CyclopsFabricatorHasCyclopsModulesTab ? cyModulesTab : null;

        public static IMCUCrossMod CrossMod => singleton;
        public static IMCURegistration Register => singleton;
        public static IMCUSearch Find => singleton;

        /// <summary>
        /// "Practically zero" for all intents and purposes.<para/>
        /// Any energy value lower than this should be considered zero.
        /// </summary>
        public const float MinimalPowerValue = 0.001f;

        private MCUServices()
        {
            // Hide constructor
        }

        public void CyclopsCharger(CyclopsChargerCreateEvent createEvent)
        {
            ChargeManager.RegisterChargerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void CyclopsUpgradeHandler(UpgradeHandlerCreateEvent createEvent)
        {
            UpgradeManager.RegisterHandlerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void AuxCyclopsManager(AuxManagerCreateEvent createEvent)
        {
            CyclopsManager.RegisterAuxManagerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public T AuxCyclopsManager<T>(SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetManager<T>(cyclops, auxManagerName);
        }

        public IEnumerable<T> FindAllAuxCyclopsManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetAllManagers<T>(auxManagerName);
        }

        public T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler
        {
            UpgradeManager mgr = CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName);
            if (mgr != null)
            {
                return mgr.GetUpgradeHandler<T>(upgradeId);
            }

            return null;
        }

        public T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            UpgradeManager mgr = CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName);
            if (mgr != null)
            {
                return mgr.GetGroupHandler<T>(upgradeId);
            }

            return null;
        }

        public T CyclopsCharger<T>(SubRoot cyclops, string chargeHandlerName) where T : class, ICyclopsCharger
        {
            ChargeManager mgr = CyclopsManager.GetManager<ChargeManager>(cyclops, ChargeManager.ManagerName);
            if (mgr != null && mgr.KnownChargers.TryGetValue(chargeHandlerName, out ICyclopsCharger cyclopsCharger))
            {
                return (T)cyclopsCharger;
            }

            return null;
        }
    }
}
