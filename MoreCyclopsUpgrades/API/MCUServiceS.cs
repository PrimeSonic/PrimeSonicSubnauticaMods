namespace MoreCyclopsUpgrades.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API.Buildables;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.PDA;
    using MoreCyclopsUpgrades.API.StatusIcons;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.Managers;

    /// <summary>
    /// The main entry point for all API services provided by MoreCyclopsUpgrades.
    /// </summary>
    /// <seealso cref="IMCUCrossMod" />
    public class MCUServices : IMCUCrossMod, IMCURegistration, IMCUSearch, IMCULogger
    {
        /// <summary>
        /// "Practically zero" for all intents and purposes.<para/>
        /// Any energy value lower than this should be considered zero.
        /// </summary>
        public const float MinimalPowerValue = 0.001f;

        private static readonly MCUServices singleton = new MCUServices();

        private MCUServices()
        {
            // Hide constructor
        }

        #region IMCUCrossMod

        /// <summary>
        /// Contains methods for asisting with cross-mod compatibility with other Cyclops mod.
        /// </summary>
        public static IMCUCrossMod CrossMod => singleton;

        /// <summary>
        /// Gets the steps to "CyclopsModules" crafting tab in the Cyclops Fabricator.<para />
        /// This would be necessary for best cross-compatibility with the [VehicleUpgradesInCyclops] mod.<para />
        /// Will return null if this mod isn't present, under the assumption that this mod isn't present and it is otherwise find to add crafting nodes to the Cyclops Fabricator root.
        /// </summary>
        /// <value>
        /// The steps to the Cyclops Fabricator's "CyclopsModules" crafting tab if it exists.
        /// </value>
        public string[] StepsToCyclopsModulesTabInCyclopsFabricator { get; } = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops") ? new[] { "CyclopsModules" } : null;

        /// <summary>
        /// Gets the <see cref="IPowerRatingManager" /> manging the specified Cyclops sub;
        /// </summary>
        /// <param name="cyclops"></param>
        /// <returns></returns>
        public IPowerRatingManager GetPowerRatingManager(SubRoot cyclops)
        {
            return CyclopsManager.GetManager(ref cyclops)?.Engine;
        }

        /// <summary>
        /// Applies the power rating modifier to the specified Cyclops.
        /// </summary>
        /// <param name="cyclops">The Cyclops sub to apply the modifier to.</param>
        /// <param name="techType">The source of the power rating modifier. Not allowed to be <see cref="TechType.None" />.</param>
        /// <param name="modifier">The modifier. Must be a positive value.<para />
        /// Values less than <c>1f</c> reduce engine efficienty rating.<para />
        /// Values greater than <c>1f</c> improve engine efficienty rating.</param>
        public void ApplyPowerRatingModifier(SubRoot cyclops, TechType techType, float modifier)
        {
            CyclopsManager.GetManager(ref cyclops)?.Engine.ApplyPowerRatingModifier(techType, modifier);
        }

        /// <summary>
        /// Checks whether the Cyclops has the specified upgrade module installed anywhere across all upgrade consoles.
        /// </summary>
        /// <param name="cyclops">The cyclops to search.</param>
        /// <param name="techType">The upgrade module's techtype ID.</param>
        /// <returns>
        ///   <c>true</c> if the upgrade is found installed on the Cyclops; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUpgradeInstalled(SubRoot cyclops, TechType techType)
        {
            var mgr = CyclopsManager.GetManager(ref cyclops);

            if (mgr == null)
                return false;

            if (mgr.Upgrade.KnownsUpgradeModules.TryGetValue(techType, out UpgradeHandler handler))
            {
                return handler.HasUpgrade;
            }

            return false;
        }

        /// <summary>
        /// Gets the total number of the specified upgrade module currently installed in the Cyclops.
        /// </summary>
        /// <param name="cyclops">The cyclops to search.</param>
        /// <param name="techType">The upgrade module's techtype ID.</param>
        /// <returns>
        /// The number of upgrade modules of this techtype ID currently in the Cyclops.
        /// </returns>
        public int GetUpgradeCount(SubRoot cyclops, TechType techType)
        {
            var mgr = CyclopsManager.GetManager(ref cyclops);

            if (mgr == null)
                return 0;

            if (mgr.Upgrade.KnownsUpgradeModules.TryGetValue(techType, out UpgradeHandler handler))
            {
                return handler.Count;
            }

            return 0;
        }

        /// <summary>
        /// Gets an enumeration of all <see cref="UpgradeSlot"/>s in this Cyclops across all upgrade consoles.
        /// </summary>
        /// <param name="cyclops">The cyclops to search.</param>
        /// <returns>An iterator of <see cref="IEnumerable{UpgradeSlot}"/> the covers all upgrade slots in the Cyclops.</returns>
        public IEnumerable<UpgradeSlot> GetAllUpgradeSlots(SubRoot cyclops)
        {
            var mgr = CyclopsManager.GetManager(ref cyclops);

            if (mgr?.Upgrade?.Initialized == true)
            {
                foreach (var slot in mgr.Upgrade.UpgradeSlots)
                    yield return slot;
            }
        }

        /// <summary>
        /// Returns a collection of all upgrade handlers for the Cyclops sub.
        /// </summary>
        /// <param name="cyclops">The cyclops sub being accessed.</param>
        /// <returns>A read-only collection of all upgrade handlers managing this sub.</returns>
        public IMCUUpgradeCollection GetAllUpgradeHandlers(SubRoot cyclops)
        {
            var mgr = CyclopsManager.GetManager(ref cyclops);

            if (mgr?.Upgrade?.Initialized == true)
            {
                return mgr.Upgrade.KnownsUpgradeModules;
            }

            return null;
        }

        #endregion

        #region IMCURegistration

        /// <summary>
        /// Register your upgrades, charger, and managers with MoreCyclopsUpgrades.<para/>
        /// WARNING! These methods MUST be invoked during patch time.
        /// </summary>
        public static IMCURegistration Register => singleton;

        /// <summary>
        /// Registers a <see cref="CreateCyclopsCharger" /> method that creates a new <see cref="Charging.CyclopsCharger" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this for rechargable batteries and energy drawn from the environment.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="Charging.CyclopsCharger" />.</typeparam>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="CreateCyclopsCharger" />.</param>
        public void CyclopsCharger<T>(CreateCyclopsCharger createEvent)
            where T : CyclopsCharger
        {
            if (ChargeManager.TooLateToRegister)
                QuickLogger.Error("CyclopsChargerCreator have already been invoked. This method should only be called during patch time.");
            else
                ChargeManager.RegisterChargerCreator(createEvent, typeof(T).Name);
        }

        /// <summary>
        /// Registers a <see cref="ICyclopsChargerCreator" /> class that can create a new <see cref="Charging.CyclopsCharger" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this for rechargable batteries and energy drawn from the environment.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="Charging.CyclopsCharger" />.</typeparam>
        /// <param name="chargerCreator">A class that implements the <see cref="ICyclopsChargerCreator.CreateCyclopsCharger(SubRoot)" /> method.</param>
        public void CyclopsCharger<T>(ICyclopsChargerCreator chargerCreator)
            where T : CyclopsCharger
        {
            CyclopsCharger<T>(chargerCreator.CreateCyclopsCharger);
        }

        /// <summary>
        /// Registers a <see cref="CreateUpgradeHandler" /> method that creates a new <see cref="UpgradeHandler" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="createEvent">A method that takes a <see cref="SubRoot"/> parameter a returns a new instance of an <see cref="UpgradeHandler" />.</param>
        public void CyclopsUpgradeHandler(CreateUpgradeHandler createEvent)
        {
            if (UpgradeManager.TooLateToRegister)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        /// <summary>
        /// Registers a <see cref="IUpgradeHandlerCreator" /> class can create a new <see cref="UpgradeHandler" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <param name="handlerCreator">A class that implements this <see cref="IUpgradeHandlerCreator.CreateUpgradeHandler(SubRoot)" /> method.</param>
        public void CyclopsUpgradeHandler(IUpgradeHandlerCreator handlerCreator)
        {
            if (UpgradeManager.TooLateToRegister)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(handlerCreator.CreateUpgradeHandler, Assembly.GetCallingAssembly().GetName().Name);
        }

        /// <summary>
        /// Registers a <see cref="CyclopsStatusIconCreator"/> method that creates a new <see cref="StatusIcons.CyclopsStatusIcon"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="StatusIcons.CyclopsStatusIcon"/>.</typeparam>
        /// <param name="createEvent">A method that takes a <see cref="SubRoot"/> parameter a returns a new instance of <see langword="abstract"/><see cref="StatusIcons.CyclopsStatusIcon"/>.</param>
        public void CyclopsStatusIcon<T>(CyclopsStatusIconCreator createEvent)
            where T : CyclopsStatusIcon
        {
            if (CyclopsHUDManager.TooLateToRegister)
                QuickLogger.Error("CyclopsStatusIconCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsHUDManager.RegisterStatusIconCreator(createEvent, typeof(T).Name);
        }

        /// <summary>
        /// Registers a <see cref="ICyclopsStatusIconCreator"/> class that creates a new <see cref="StatusIcons.CyclopsStatusIcon"/> on demand.<para/>
        /// This method will be invoked only once for each Cyclops sub in the game world.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="StatusIcons.CyclopsStatusIcon"/>.</typeparam>
        public void CyclopsStatusIcon<T>(ICyclopsStatusIconCreator statusIconCreator)
            where T : CyclopsStatusIcon
        {
            CyclopsStatusIcon<T>(statusIconCreator.CreateCyclopsStatusIcon);
        }

        /// <summary>
        /// Registers a <see cref="CreateAuxCyclopsManager" /> method that creates returns a new <see cref="IAuxCyclopsManager" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this when you simply need to have a class that is attaches one instance per Cyclops.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <param name="createEvent">The create event.</param>
        public void AuxCyclopsManager<T>(CreateAuxCyclopsManager createEvent)
            where T : IAuxCyclopsManager
        {
            if (CyclopsManager.TooLateToRegister)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(createEvent, typeof(T).Name);
        }

        /// <summary>
        /// Registers a <see cref="IAuxCyclopsManagerCreator" /> class that can create a new <see cref="IAuxCyclopsManager" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this when you simply need to have a class that attaches one instance per Cyclops.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <param name="managerCreator">The manager creator class instance.</param>
        public void AuxCyclopsManager<T>(IAuxCyclopsManagerCreator managerCreator)
            where T : IAuxCyclopsManager
        {
            if (CyclopsManager.TooLateToRegister)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(managerCreator.CreateAuxCyclopsManager, typeof(T).Name);
        }

        /// <summary>
        /// Registers a <see cref="IIconOverlayCreator" /> class that can create a new <see cref="IconOverlay" /> on demand.<para />
        /// This method will be invoked every time the PDA screen opens up on a Cyclops Upgrade Console that contains a module of the specified <see cref="TechType" />.
        /// </summary>
        /// <param name="techType">The upgrade module's techtype.</param>
        /// <param name="overlayCreator">A class that implements a method the <see cref="IIconOverlayCreator.CreateIconOverlay(uGUI_ItemIcon, InventoryItem)" /> method.</param>
        public void PdaIconOverlay(TechType techType, IIconOverlayCreator overlayCreator)
        {
            PdaOverlayManager.RegisterHandlerCreator(techType, overlayCreator.CreateIconOverlay, Assembly.GetCallingAssembly().GetName().Name);
        }

        /// <summary>
        /// Registers a <see cref="CreateIconOverlay" /> method that creates a new <see cref="IconOverlay" /> on demand.<para />
        /// This method will be invoked every time the PDA screen opens up on a Cyclops Upgrade Console that contains a module of the specified <see cref="TechType" />.
        /// </summary>
        /// <param name="techType">The upgrade module's techtype.</param>
        /// <param name="createEvent">A method that takes in a <see cref="uGUI_ItemIcon" /> and <see cref="InventoryItem" /> and returns a new <see cref="IconOverlay" />.</param>
        public void PdaIconOverlay(TechType techType, CreateIconOverlay createEvent)
        {
            PdaOverlayManager.RegisterHandlerCreator(techType, createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        #endregion

        #region IMCUSearch

        /// <summary>
        /// Provides methods to find the upgrades, chargers, and managers you registered once the Cyclops sub is running.
        /// </summary>
        public static IMCUSearch Find => singleton;

        /// <summary>
        /// Gets the typed <see cref="IAuxCyclopsManager" /> for the specified Cyclops sub.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <returns>
        /// A type casted <see cref="IAuxCyclopsManager" /> if found; Otherwise returns null if not found.
        /// </returns>
        /// <seealso cref="CreateAuxCyclopsManager" />
        public T AuxCyclopsManager<T>(SubRoot cyclops)
            where T : class, IAuxCyclopsManager
        {
            if (cyclops == null)
                return null;

            return CyclopsManager.GetManager<T>(ref cyclops, typeof(T).Name);
        }

        /// <summary>
        /// Gets all typed <see cref="IAuxCyclopsManager" />s across all Cyclops subs.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <returns>
        /// A type casted enumeration of all <see cref="IAuxCyclopsManager" />s found across all Cyclops subs, identified by name.
        /// </returns>
        public IEnumerable<T> AllAuxCyclopsManagers<T>()
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetAllManagers<T>(typeof(T).Name);
        }

        /// <summary>
        /// Gets the typed <see cref="Charging.CyclopsCharger" /> at the specified Cyclops sub.<para />
        /// Use this if you need to obtain a reference to your <seealso cref="Charging.CyclopsCharger" /> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateCyclopsCharger" /> you passed into <seealso cref="IMCURegistration.CyclopsCharger(CreateCyclopsCharger)" />.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <returns>
        /// A type casted <see cref="Charging.CyclopsCharger" /> if found; Otherwise returns null.
        /// </returns>
        public T CyclopsCharger<T>(SubRoot cyclops) where T : CyclopsCharger
        {
            return CyclopsManager.GetManager(ref cyclops)?.Charge.GetCharger<T>(typeof(T).Name);
        }

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType" />.<para />
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler" /> for something else in your mod.
        /// </summary>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>
        /// An <see cref="UpgradeHandler" /> if found by techtype; Otherwise returns null.
        /// </returns>
        public UpgradeHandler CyclopsUpgradeHandler(SubRoot cyclops, TechType upgradeId)
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetUpgradeHandler<UpgradeHandler>(upgradeId);
        }

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType" />.<para />
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler" /> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateUpgradeHandler" /> you passed into <seealso cref="IMCURegistration.CyclopsUpgradeHandler(CreateUpgradeHandler)" />.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>
        /// A type casted <see cref="UpgradeHandler" /> if found by techtype; Otherwise returns null.
        /// </returns>
        public T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetUpgradeHandler<T>(upgradeId);
        }

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType" />.<para />
        /// Use this if you need to obtain a reference to your <seealso cref="StackingGroupHandler" /> or <seealso cref="TieredGroupHandler{T}" /> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateUpgradeHandler" /> you passed into <seealso cref="IMCURegistration.CyclopsUpgradeHandler(CreateUpgradeHandler)" />.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <param name="additionalIds">Additional techtype IDs for a more precise search.</param>
        /// <returns>
        /// A type casted <see cref="UpgradeHandler" /> if found by techtype; Otherwise returns null.
        /// </returns>
        public T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetGroupHandler<T>(upgradeId);
        }

        #endregion

        #region IMCULogger

        /// <summary>
        /// Provides a set of logging APIs that other mods can use.<para/>
        /// Debug level logs will only be printed of MCU's debug logging is enabled.
        /// </summary>
        public static IMCULogger Logger => singleton;

        /// <summary>
        /// Gets a value indicating whether calls into <see cref="Debug" /> are handled or ignored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug level logs enabled; otherwise, <c>false</c>.
        /// </value>
        public bool DebugLogsEnabled => QuickLogger.DebugLogsEnabled;

        /// <summary>
        /// Writes an INFO level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        public void Info(string logmessage, bool showOnScreen = false)
        {
            QuickLogger.Info(logmessage, showOnScreen, Assembly.GetCallingAssembly().GetName());
        }

        /// <summary>
        /// Writes a WARN level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        public void Warning(string logmessage, bool showOnScreen = false)
        {
            QuickLogger.Warning(logmessage, showOnScreen, Assembly.GetCallingAssembly().GetName());
        }

        /// <summary>
        /// Writes an ERROR level log to the log file. Can be optionally printed to screen.
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        public void Error(string logmessage, bool showOnScreen = false)
        {
            QuickLogger.Error(logmessage, showOnScreen, Assembly.GetCallingAssembly().GetName());
        }

        /// <summary>
        /// Writes <see cref="Exception" /> to an ERROR level log to file.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="logmessage">The optional additional message.</param>
        public void Error(Exception ex, string logmessage = null)
        {
            if (logmessage == null)
                QuickLogger.Error(ex, Assembly.GetCallingAssembly().GetName());
            else
                QuickLogger.Error(logmessage, ex, Assembly.GetCallingAssembly().GetName());
        }

        /// <summary>
        /// Writes an DEBUG level log to the log file if <see cref="DebugLogsEnabled" /> is enabled. Can be optionally printed to screen.<para />
        /// No action taken when <see cref="DebugLogsEnabled" /> is set to <c>false</c>;
        /// </summary>
        /// <param name="logmessage">The log message to write.</param>
        /// <param name="showOnScreen">if set to <c>true</c> the log message will show on screen.</param>
        public void Debug(string logmessage, bool showOnScreen = false)
        {
            QuickLogger.Debug(logmessage, showOnScreen, Assembly.GetCallingAssembly().GetName());
        }



        #endregion
    }
}
