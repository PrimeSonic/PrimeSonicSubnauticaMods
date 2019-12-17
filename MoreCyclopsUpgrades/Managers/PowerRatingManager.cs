namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.Config;
    using UnityEngine;

    internal class PowerRatingManager : IPowerRatingManager
    {
        private readonly SubRoot cyclops;
        private readonly IDictionary<TechType, float> modifiers = new Dictionary<TechType, float>();
        private readonly IModConfig settings = ModConfig.Main;

        public PowerRatingManager(SubRoot cyclops)
        {

            this.cyclops = cyclops;
        }

        public void ApplyPowerRatingModifier(TechType techType, float modifier)
        {
            if (techType == TechType.None)
                return;

            modifiers[techType] = Mathf.Abs(modifier);
        }

        internal void UpdatePowerRating()
        {
            float rating = settings.RechargePenalty;

            foreach (KeyValuePair<TechType, float> modifier in modifiers)
                rating *= modifier.Value;

            if (rating != cyclops.currPowerRating)
            {
                cyclops.currPowerRating = rating;
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", rating));
            }
        }
    }
}
