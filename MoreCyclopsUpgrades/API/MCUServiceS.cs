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

        public IPowerRatingManager GetPowerRatingManager(SubRoot cyclops)
        {
            return CyclopsManager.GetManager(ref cyclops)?.Engine;
        }

        public void ApplyPowerRatingModifier(SubRoot cyclops, TechType techType, float modifier)
        {
            CyclopsManager.GetManager(ref cyclops)?.Engine.ApplyPowerRatingModifier(techType, modifier);
        }

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

        #endregion

        #region IMCURegistration

        /// <summary>
        /// Register your upgrades, charger, and managers with MoreCyclopsUpgrades.<para/>
        /// WARNING! These methods MUST be invoked during patch time.
        /// </summary>
        public static IMCURegistration Register => singleton;

        public void CyclopsCharger<T>(CreateCyclopsCharger createEvent)
            where T : CyclopsCharger
        {
            if (ChargeManager.TooLateToRegister)
                QuickLogger.Error("CyclopsChargerCreator have already been invoked. This method should only be called during patch time.");
            else
                ChargeManager.RegisterChargerCreator(createEvent, typeof(T).Name);
        }

        public void CyclopsCharger<T>(ICyclopsChargerCreator chargerCreator)
            where T : CyclopsCharger
        {
            CyclopsCharger<T>(chargerCreator.CreateCyclopsCharger);
        }

        public void CyclopsUpgradeHandler(CreateUpgradeHandler createEvent)
        {
            if (UpgradeManager.TooLateToRegister)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(createEvent, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void CyclopsUpgradeHandler(IUpgradeHandlerCreator handlerCreator)
        {
            if (UpgradeManager.TooLateToRegister)
                QuickLogger.Error("UpgradeHandlerCreators have already been invoked. This method should only be called during patch time.");
            else
                UpgradeManager.RegisterHandlerCreator(handlerCreator.CreateUpgradeHandler, Assembly.GetCallingAssembly().GetName().Name);
        }

        public void AuxCyclopsManager<T>(CreateAuxCyclopsManager createEvent)
            where T : IAuxCyclopsManager
        {
            if (CyclopsManager.TooLateToRegister)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(createEvent, typeof(T).Name);
        }

        public void AuxCyclopsManager<T>(IAuxCyclopsManagerCreator managerCreator)
            where T : IAuxCyclopsManager
        {
            if (CyclopsManager.TooLateToRegister)
                QuickLogger.Error("AuxCyclopsManagerCreator have already been invoked. This method should only be called during patch time.");
            else
                CyclopsManager.RegisterAuxManagerCreator(managerCreator.CreateAuxCyclopsManager, typeof(T).Name);
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

        public T AuxCyclopsManager<T>(SubRoot cyclops)
            where T : class, IAuxCyclopsManager
        {
            if (cyclops == null)
                return null;

            return CyclopsManager.GetManager<T>(ref cyclops, typeof(T).Name);
        }

        public IEnumerable<T> AllAuxCyclopsManagers<T>()
            where T : class, IAuxCyclopsManager
        {
            return CyclopsManager.GetAllManagers<T>(typeof(T).Name);
        }

        public T CyclopsCharger<T>(SubRoot cyclops) where T : CyclopsCharger
        {
            return CyclopsManager.GetManager(ref cyclops)?.Charge.GetCharger<T>(typeof(T).Name);
        }

        public IEnumerable<T> AllCyclopsChargers<T>() where T : CyclopsCharger
        {
            for (int m = 0; m < CyclopsManager.Managers.Count; m++)
            {
                T chg = CyclopsManager.Managers[m].Charge?.GetCharger<T>(typeof(T).Name);

                if (chg != null)
                    yield return chg;
            }
        }

        public UpgradeHandler CyclopsUpgradeHandler(SubRoot cyclops, TechType upgradeId)
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetUpgradeHandler<UpgradeHandler>(upgradeId);
        }

        public T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetUpgradeHandler<T>(upgradeId);
        }

        public T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler
        {
            return CyclopsManager.GetManager(ref cyclops)?.Upgrade?.GetGroupHandler<T>(upgradeId);
        }

        #endregion
    }
}
