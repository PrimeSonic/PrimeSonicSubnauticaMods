namespace MoreCyclopsUpgrades.API
{
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.PDA;
    using MoreCyclopsUpgrades.API.Upgrades;

    /// <summary>
    /// Defines a set of patch-time APIs to be used to register your own class factories that will integrate into MoreCyclopsUpgrades.
    /// </summary>
    public interface IMCURegistration
    {
        /// <summary>
        /// Registers a <see cref="CreateAuxCyclopsManager" /> method that creates returns a new <see cref="IAuxCyclopsManager" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this when you simply need to have a class that is attaches one instance per Cyclops.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <param name="createEvent">The create event.</param>
        void AuxCyclopsManager<T>(CreateAuxCyclopsManager createEvent) where T : IAuxCyclopsManager;

        /// <summary>
        /// Registers a <see cref="IAuxCyclopsManagerCreator" /> class that can create a new <see cref="IAuxCyclopsManager" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this when you simply need to have a class that attaches one instance per Cyclops.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="IAuxCyclopsManager" />.</typeparam>
        /// <param name="managerCreator">The manager creator class instance.</param>
        void AuxCyclopsManager<T>(IAuxCyclopsManagerCreator managerCreator) where T : IAuxCyclopsManager;

        /// <summary>
        /// Registers a <see cref="CreateCyclopsCharger" /> method that creates a new <see cref="CyclopsCharger" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this for rechargable batteries and energy drawn from the environment.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="CyclopsCharger" />.</typeparam>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="CreateCyclopsCharger" />.</param>
        void CyclopsCharger<T>(CreateCyclopsCharger createEvent) where T : CyclopsCharger;

        /// <summary>
        /// Registers a <see cref="ICyclopsChargerCreator" /> class that can create a new <see cref="CyclopsCharger" /> on demand.<para />
        /// This method will be invoked only once for each Cyclops sub in the game world.<para />
        /// Use this for rechargable batteries and energy drawn from the environment.
        /// </summary>
        /// <typeparam name="T">Your class that implements <see cref="CyclopsCharger" />.</typeparam>
        /// <param name="chargerCreator">A class that implements the <see cref="ICyclopsChargerCreator.CreateCyclopsCharger(SubRoot)" /> method.</param>
        void CyclopsCharger<T>(ICyclopsChargerCreator chargerCreator) where T : CyclopsCharger;

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
        /// <param name="handlerCreator">A class that implements this <see cref="IUpgradeHandlerCreator.CreateUpgradeHandler(SubRoot)"/> method.</param>
        void CyclopsUpgradeHandler(IUpgradeHandlerCreator handlerCreator);

        /// <summary>
        /// Registers a <see cref="IIconOverlayCreator" /> class that can create a new <see cref="IconOverlay" /> on demand.<para />
        /// This method will be invoked every time the PDA screen opens up on a Cyclops Upgrade Console that contains a module of the specified <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The upgrade module's techtype.</param>
        /// <param name="overlayCreator">A class that implements a method the <see cref="IIconOverlayCreator.CreateIconOverlay(uGUI_ItemIcon, InventoryItem)"/> method.</param>
        void PdaIconOverlay(TechType techType, IIconOverlayCreator overlayCreator);

        /// <summary>
        /// Registers a <see cref="CreateIconOverlay" /> method that creates a new <see cref="IconOverlay" /> on demand.<para />
        /// This method will be invoked every time the PDA screen opens up on a Cyclops Upgrade Console that contains a module of the specified <see cref="TechType"/>.
        /// </summary>
        /// <param name="techType">The upgrade module's techtype.</param>
        /// <param name="createEvent">A method that takes in a <see cref="uGUI_ItemIcon"/> and <see cref="InventoryItem"/> and returns a new <see cref="IconOverlay"/>.</param>
        void PdaIconOverlay(TechType techType, CreateIconOverlay createEvent);
    }
}
