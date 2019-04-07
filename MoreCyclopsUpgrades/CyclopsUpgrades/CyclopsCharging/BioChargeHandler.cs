namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioChargeHandler : ICyclopsCharger
    {
        private readonly PowerManager powerManager;
        private readonly BioBoosterUpgradeHandler bioBoosters;

        private List<CyBioReactorMono> BioReactors => powerManager.CyBioReactors;

        private int reactorsWithPower = 0;
        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        public readonly SubRoot Cyclops;

        public BioChargeHandler(SubRoot cyclops, PowerManager powerManagerReference)
        {
            Cyclops = cyclops;
            powerManager = powerManagerReference;
            bioBoosters = powerManager.BioBoosters;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return SpriteManager.Get(CyclopsModule.BioReactorBoosterID);
        }

        public string GetIndicatorText()
        {
            return NumberFormatter.FormatNumber(Mathf.CeilToInt(totalBioCharge), NumberFormat.Amount);
        }

        public Color GetIndicatorTextColor()
        {
            return NumberFormatter.GetNumberColor(totalBioCharge, totalBioCapacity, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return reactorsWithPower > 0;
        }

        public float ProducePower(float requestedPower)
        {
            if (this.BioReactors.Count == 0)
                return 0f;

            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            float charge = 0f;

            int poweredReactors = 0;
            foreach (CyBioReactorMono reactor in this.BioReactors)
            {
                if (!reactor.HasPower)
                    continue;

                if (poweredReactors < bioBoosters.MaxBioreactorsAllowed)
                {
                    poweredReactors++;
                    reactor.OverLimit = false;

                    charge += reactor.GetBatteryPower(PowerManager.BatteryDrainRate, requestedPower);

                    tempBioCharge += reactor.Battery._charge;
                    tempBioCapacity = reactor.Battery._capacity;
                }
                else
                {
                    reactor.OverLimit = true;
                }
            }

            reactorsWithPower = poweredReactors;
            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            return charge;
        }
    }
}
