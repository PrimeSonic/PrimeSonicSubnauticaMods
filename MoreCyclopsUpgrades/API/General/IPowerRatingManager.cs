namespace MoreCyclopsUpgrades.API.General
{
    /// <summary>
    /// Defines a class that can handle modifiers for the Cyclops Power Rating across multiple multipliers.
    /// </summary>
    public interface IPowerRatingManager
    {
        /// <summary>
        /// Applies the power rating modifier to the Cyclops this <see cref="IPowerRatingManager"/> is managing.
        /// </summary>
        /// <param name="techType">The source of the power rating modifier. Not allowed to be <see cref="TechType.None"/>.</param>
        /// <param name="modifier">
        /// The modifier. Must be a positive value.<para/>
        /// Values less than <c>1f</c> reduce engine efficienty rating.<para/>
        /// Values greater than <c>1f</c> improve engine efficienty rating.
        /// </param>
        void ApplyPowerRatingModifier(TechType techType, float modifier);
    }
}