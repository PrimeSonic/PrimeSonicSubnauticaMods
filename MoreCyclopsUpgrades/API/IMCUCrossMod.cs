namespace MoreCyclopsUpgrades.API
{
    using MoreCyclopsUpgrades.API.General;

    /// <summary>
    /// Defines a set of utility APIs used for better cross-mod compatibility. 
    /// </summary>
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

        /// <summary>
        /// Gets the <see cref="IPowerRatingManager"/> manging the specified Cyclops sub;
        /// </summary>
        /// <param name="cyclops"></param>
        /// <returns></returns>
        IPowerRatingManager GetPowerRatingManager(SubRoot cyclops);

        /// <summary>
        /// Applies the power rating modifier to the specified Cyclops.
        /// </summary>
        /// <param name="cyclops">The Cyclops sub to apply the modifier to.</param>
        /// <param name="techType">The source of the power rating modifier. Not allowed to be <see cref="TechType.None"/>.</param>
        /// <param name="modifier">
        /// The modifier. Must be a positive value.<para/>
        /// Values less than <c>1f</c> reduce engine efficienty rating.<para/>
        /// Values greater than <c>1f</c> improve engine efficienty rating.
        /// </param>
        void ApplyPowerRatingModifier(SubRoot cyclops, TechType techType, float modifier);

        /// <summary>
        /// Checks whether the Cyclops has the specified upgrade module installed anywhere across all upgrade consoles.
        /// </summary>
        /// <param name="cyclops">The cyclops to search.</param>
        /// <param name="techType">The upgrade module's techtype ID.</param>
        /// <returns>
        ///   <c>true</c> if the upgrade is found installed on the Cyclops; otherwise, <c>false</c>.
        /// </returns>
        bool HasUpgradeInstalled(SubRoot cyclops, TechType techType);

        /// <summary>
        /// Gets the total number of the specified upgrade module currently installed in the Cyclops.
        /// </summary>
        /// <param name="cyclops">The cyclops to search.</param>
        /// <param name="techType">The upgrade module's techtype ID.</param>
        /// <returns>The number of upgrade modules of this techtype ID currently in the Cyclops.</returns>
        int GetUpgradeCount(SubRoot cyclops, TechType techType);
    }
}
