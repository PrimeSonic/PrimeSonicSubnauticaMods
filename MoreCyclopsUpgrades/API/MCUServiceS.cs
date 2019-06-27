namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.PDA;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Managers;

    /// <summary>
    /// The main entry point for all API services provided by MoreCyclopsUpgrades.
    /// </summary>
    /// <seealso cref="IMCUCrossMod" />
    public class MCUServices : IMCUCrossMod, IMCURegistration, IMCUSearch
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

        public string[] StepsToCyclopsModulesTabInCyclopsFabricator { get; } = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops") ? new[] { "CyclopsModules" } : null;

        public float ChangePowerRatingWithPenalty(SubRoot cyclops, float powRating)
        {
            return cyclops.currPowerRating = ModConfig.Main.RechargePenalty * powRating;
        }

        #endregion

        #region IMCURegistration

        /// <summary>
        /// Register your upgrades, charger, and managers with MoreCyclopsUpgrades.<para/>
        /// WARNING! These methods MUST be invoked during patch time.
        /// </summary>
        public static IMCURegistration Register => singleton;

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

        public void PdaIconOverlay(TechType techType, IIconOverlayCreator overlayCreator)
        {
            PdaOverlayManager.RegisterHandlerCreator(techType, overlayCreator.CreateIconOverlay, Assembly.GetCallingAssembly().GetName().Name);
        }

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

        public T CyclopsCharger<T>(SubRoot cyclops, string chargeHandlerName) where T : class, ICyclopsCharger
        {
            return CyclopsManager.GetManager<ChargeManager>(cyclops, ChargeManager.ManagerName)?.GetCharger<T>(chargeHandlerName);
        }

        public IEnumerable<T> AllCyclopsChargers<T>(string chargeHandlerName) where T : class, ICyclopsCharger
        {
            foreach (ChargeManager item in CyclopsManager.GetAllManagers<ChargeManager>(ChargeManager.ManagerName))
            {
                T chg = item.GetCharger<T>(chargeHandlerName);

                if (chg != null)
                    yield return chg;
            }
        }

        public T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler
        {
            return CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName)?.GetUpgradeHandler<T>(upgradeId);
        }

        public T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            return CyclopsManager.GetManager<UpgradeManager>(cyclops, UpgradeManager.ManagerName)?.GetGroupHandler<T>(upgradeId);
        }

        #endregion
    }
}
