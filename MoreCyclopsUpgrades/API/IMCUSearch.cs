namespace MoreCyclopsUpgrades.API
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;

    /// <summary>
    /// Defines a set of game-time APIs to search for the various instances created by the creator methods and classes registered on <see cref="IMCURegistration"/>.
    /// </summary>
    public interface IMCUSearch
    {
        /// <summary>
        /// Gets the typed <see cref="IAuxCyclopsManager"/> for the specified Cyclops sub.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <returns>A type casted <see cref="IAuxCyclopsManager"/> if found; Otherwise returns null if not found.</returns>
        /// <seealso cref="CreateAuxCyclopsManager"/>
        T AuxCyclopsManager<T>(SubRoot cyclops) where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets all typed <see cref="IAuxCyclopsManager"/>s across all Cyclops subs.
        /// </summary>
        /// <typeparam name="T">The class you created that implements <see cref="IAuxCyclopsManager"/>.</typeparam>
        /// <returns>A type casted enumeration of all <see cref="IAuxCyclopsManager"/>s found across all Cyclops subs, identified by name.</returns>
        IEnumerable<T> AllAuxCyclopsManagers<T>() where T : class, IAuxCyclopsManager;

        /// <summary>
        /// Gets the typed <see cref="CyclopsCharger"/> at the specified Cyclops sub.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="CyclopsCharger"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateCyclopsCharger"/> you passed into <seealso cref="IMCURegistration.CyclopsCharger(CreateCyclopsCharger)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <returns>A type casted <see cref="CyclopsCharger"/> if found; Otherwise returns null.</returns>
        T CyclopsCharger<T>(SubRoot cyclops) where T : CyclopsCharger;

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler"/> for something else in your mod.
        /// </summary>        
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>An <see cref="UpgradeHandler"/> if found by techtype; Otherwise returns null.</returns>
        UpgradeHandler CyclopsUpgradeHandler(SubRoot cyclops, TechType upgradeId);

        /// <summary>
        /// Gets the upgrade handler at the specified Cyclops sub for the specified upgrade module <see cref="TechType"/>.<para/>
        /// Use this if you need to obtain a reference to your <seealso cref="UpgradeHandler"/> for something else in your mod.
        /// </summary>
        /// <typeparam name="T">The class created by the <seealso cref="CreateUpgradeHandler"/> you passed into <seealso cref="IMCURegistration.CyclopsUpgradeHandler(CreateUpgradeHandler)"/>.</typeparam>
        /// <param name="cyclops">The cyclops to search in.</param>
        /// <param name="upgradeId">The upgrade module techtype ID.</param>
        /// <returns>A type casted <see cref="UpgradeHandler"/> if found by techtype; Otherwise returns null.</returns>
        T CyclopsUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId) where T : UpgradeHandler;

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
        T CyclopsGroupUpgradeHandler<T>(SubRoot cyclops, TechType upgradeId, params TechType[] additionalIds) where T : UpgradeHandler, IGroupHandler;
    }
}
