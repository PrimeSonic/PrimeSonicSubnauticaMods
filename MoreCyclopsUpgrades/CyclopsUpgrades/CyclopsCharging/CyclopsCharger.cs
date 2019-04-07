namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using UnityEngine;

    internal abstract class CyclopsCharger : ICyclopsCharger
    {
        public readonly SubRoot Cyclops;

        protected CyclopsCharger(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public abstract float ProducePower(float requestedPower);

        public abstract bool HasPowerIndicatorInfo();

        public abstract Atlas.Sprite GetIndicatorSprite();

        public abstract string GetIndicatorText();

        public abstract Color GetIndicatorTextColor();
    }
}
