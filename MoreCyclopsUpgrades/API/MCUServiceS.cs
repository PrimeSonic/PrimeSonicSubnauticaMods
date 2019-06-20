namespace MoreCyclopsUpgrades.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.Managers;

    public interface IMCURegistration
    {
        /// <summary>
        /// Registers a <see cref="CreateAuxCyclopsManager"/> method that creates returns a new <see cref="IAuxCyclopsManager"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.<para/>
        /// Use this when you simply need to have a class that is attaches one instance per Cyclops.
        /// </summary>
        /// <param name="createEvent">The create event.</param>
        void AuxCyclopsManager(CreateAuxCyclopsManager createEvent);

        /// <summary>
        /// Registers a <see cref="IAuxCyclopsManagerCreator"/> class that can create a new <see cref="IAuxCyclopsManager"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.<para/>
        /// Use this when you simply need to have a class that attaches one instance per Cyclops.
        /// </summary>
        /// <param name="createEvent">The create event.</param>
        void AuxCyclopsManager(IAuxCyclopsManagerCreator managerCreator);

        /// <summary>
        /// Registers a <see cref="CreateCyclopsCharger"/> method that creates a new <see cref="ICyclopsCharger"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="CreateCyclopsCharger"/>.</param>
        void CyclopsCharger(CreateCyclopsCharger createEvent);

        /// <summary>
        /// Registers a <see cref="ICyclopsChargerCreator"/> class can create a new <see cref="ICyclopsCharger"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="chargerCreator">A class that implements the <see cref="ICyclopsChargerCreator.CreateCyclopsCharger(SubRoot)"/> method.</param>
        void CyclopsCharger(ICyclopsChargerCreator chargerCreator);

        /// <summary>
        /// Registers a <see cref="CreateUpgradeHandler"/> method that creates a new <see cref="UpgradeHandler"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="UpgradeHandler"/>.</param>
        void CyclopsUpgradeHandler(CreateUpgradeHandler createEvent);

        /// <summary>
        /// Registers a <see cref="CreateUpgradeHandler"/> class can create a new <see cref="UpgradeHandler"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A class that implements this <see cref="IUpgradeHandlerCreator.CreateUpgradeHandler(SubRoot)"/> method.</param>
        void CyclopsUpgradeHandler(IUpgradeHandlerCreator handlerCreator);
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
        /// <seealso cref="CreateAuxCyclopsManager"/>
        T AuxCyclopsManager<T>(SubRoot cyclops, string auxManagerName) where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets all typed <see cref="IAuxCyclopsManager"/>s across all Cyclops subs with the given <seealso cref="IAuxCyclopsManager.Name"/>.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="auxManagerName">The <seealso cref="IAuxCyclopsManager.Name"/> you defined for the auxilary cyclops manager.</param>
        /// <returns>A type casted enumeration of all <see cref="IAuxCyclopsManager"/>s found across all Cyclops subs, identified by name.</returns>
        IEnumerable<T> AllAuxCyclopsManagers<T>(string auxManagerName) where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets the charge hangler at the specified Cyclops sub for the provided <seealso cref="ICyclopsCharger.Name"/> string.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="ICyclopsCharger"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateCyclopsCharger"/> you passed into <seealso cref="RegisterChargerCreator(CreateCyclopsCharger)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="chargeHandlerName">The <seealso cref="ICyclopsCharger.Name"/> of the charge handler.</param>
        /// <returns>A type casted <see cref="ICyclopsCharger"/> if found by name; Otherwise returns null.</returns>
        T CyclopsCharger<T>(SubRoot cyclops, string chargeHandlerName) where T : class, ICyclopsCharger;

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateUpgradeHandler"/> you passed into <seealso cref="RegisterUpgradeCreator(CreateUpgradeHandler)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>A type casted <see cref="UpgradeHandler"/> if found by techtype; Otherwise returns null.</returns>
        T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler;

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="StackingGroupHandler"/> or <seealso cref="TieredGroupHandler{T}"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateUpgradeHandler"/> you passed into <seealso cref="RegisterUpgradeCreator(CreateUpgradeHandler)"/>.</typeparam>
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

        /// <summary>
        /// Contains methods for asisting with cross-mod compatibility with other Cyclops mod.
        /// </summary>
        public static IMCUCrossMod CrossMod => singleton;

        /// <summary>
        /// Register your upgrades, charger, and managers with MoreCyclopsUpgrades.<para/>
        /// WARNING! These methods MUST be invoked during patch time.
        /// </summary>
        public static IMCURegistration Register => singleton;

        /// <summary>
        /// Provides methods to find the upgrades, chargers, and managers you registered once the Cyclops sub is running.
        /// </summary>
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

        public void CyclopsCharger(CreateCyclopsCharger createEvent)
        {
            if (ChargeManager.Initialized)
                QuickLogger.Error("CyclopsChargerCreator have already been invoked. This method should only be called during patch time.");
            else
                ChargeManager.RegisterChargerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void CyclopsCharger(ICyclopsChargerCreator chargerCreator)
        {
            if (ChargeManager.Initialized)
                QuickLogger.Error("CyclopsChargerCreator have already been invoked. This method should only be called during patch time.");
            else
                ChargeManager.RegisterChargerCreator(chargerCreator.CreateCyclopsCharger, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void CyclopsUpgradeHandler(CreateUpgradeHandler createEvent)
        {
            if (UpgradeManager.Initialized)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void CyclopsUpgradeHandler(IUpgradeHandlerCreator handlerCreator)
        {
            if (UpgradeManager.Initialized)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(handlerCreator.CreateUpgradeHandler, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void AuxCyclopsManager(CreateAuxCyclopsManager createEvent)
        {
            if (CyclopsManager.Initialized)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void AuxCyclopsManager(IAuxCyclopsManagerCreator managerCreator)
        {
            if (CyclopsManager.Initialized)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(managerCreator.CreateAuxCyclopsManager, Assembly.GetCallingAssembly().GetName().Name);
        }

        public T AuxCyclopsManager<T>(SubRoot cyclops, string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetManager<T>(cyclops, auxManagerName);
        }

        public IEnumerable<T> AllAuxCyclopsManagers<T>(string auxManagerName)
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetAllManagers<T>(auxManagerName);
        }

        public T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler
        {
            return CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName)?.GetUpgradeHandler<T>(upgradeId);
        }

        public T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            return CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName)?.GetGroupHandler<T>(upgradeId);
        }

        public T CyclopsCharger<T>(SubRoot cyclops, string chargeHandlerName) where T : class, ICyclopsCharger
        {
            return CyclopsManager.GetManager<ChargeManager>(cyclops, ChargeManager.ManagerName)?.GetCharger<T>(chargeHandlerName);
        }
    }
}
