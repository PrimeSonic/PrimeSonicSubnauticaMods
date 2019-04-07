namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using UnityEngine;

    public delegate CyclopsCharger ChargerCreator(SubRoot cyclops);

    /// <summary>
    /// Defines the bahavior of a class that the Cyclops uses to recharge itself.
    /// </summary>
    public abstract class CyclopsCharger
    {
        public readonly SubRoot Cyclops;

        protected CyclopsCharger(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public abstract float ProducePower(float requestedPower);

        public abstract bool IsShowingIndicator();

        public abstract Atlas.Sprite GetIndicatorSprite();

        public abstract string GetIndicatorText();

        public abstract Color GetIndicatorTextColor();
    }
}
