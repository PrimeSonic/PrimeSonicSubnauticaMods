namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;

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
}
