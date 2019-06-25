namespace MoreCyclopsUpgrades.API
{
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
        /// Updates the Cyclops Power Rating while including the penalty set by the user for the current Challenge Mode.
        /// </summary>
        /// <param name="cyclops">The cyclops to update.</param>
        /// <param name="powRating">The power rating.</param>
        /// <returns>The adjusted power rating value.</returns>
        float ChangePowerRatingWithPenalty(SubRoot cyclops, float powRating);
    }
}
