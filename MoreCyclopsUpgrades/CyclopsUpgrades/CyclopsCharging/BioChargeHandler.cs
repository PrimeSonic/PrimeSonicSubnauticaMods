namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioChargeHandler : CyclopsCharger
    {
        private readonly PowerManager powerManager;
        private readonly BioBoosterUpgradeHandler bioBoosters;

        private List<CyBioReactorMono> BioReactors => powerManager.CyBioReactors;

        private int countWithPower = 0;
        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        public BioChargeHandler(SubRoot cyclops, PowerManager powerManagerReference) : base(cyclops)
        {
            powerManager = powerManagerReference;
            bioBoosters = powerManager.BioBoosters;
        }

        public override Atlas.Sprite GetIndicatorSprite()
        {
            return SpriteManager.Get(CyclopsModule.BioReactorBoosterID);
        }

        public override string GetIndicatorText()
        {
            return NumberFormatter.FormatNumber(Mathf.CeilToInt(totalBioCharge), NumberFormat.Amount);
        }

        public override Color GetIndicatorTextColor()
        {
            return NumberFormatter.GetNumberColor(totalBioCharge, totalBioCapacity, 0f);
        }

        public override bool IsShowingIndicator()
        {
            return countWithPower > 0;
        }

        public override float ProducePower(float requestedPower)
        {
            if (this.BioReactors.Count == 0)
                return 0f;

            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            float charge = 0f;

            countWithPower = 0;
            foreach (CyBioReactorMono reactor in this.BioReactors)
            {
                if (!reactor.HasPower)
                    continue;

                if (countWithPower < bioBoosters.MaxBioreactorsAllowed)
                {
                    countWithPower++;
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

            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            return charge;
        }
    }
}
