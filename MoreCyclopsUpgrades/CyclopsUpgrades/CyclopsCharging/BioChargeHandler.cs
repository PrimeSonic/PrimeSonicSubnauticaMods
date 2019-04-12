namespace MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging
{
    using MoreCyclopsUpgrades.Caching;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using MoreCyclopsUpgrades.SaveData;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioChargeHandler : ICyclopsCharger
    {
        private const float BioReactorRateLimiter = 0.85f;

        private readonly ChargeManager ChargeManager;
        private BioBoosterUpgradeHandler BioBoosters => ChargeManager.BioBoosters;

        private List<CyBioReactorMono> BioReactors => ChargeManager.CyBioReactors;

        private readonly int MaxBioReactors = ModConfig.Settings.MaxBioReactors();
        internal bool ProducingPower = false;

        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        public readonly SubRoot Cyclops;

        public BioChargeHandler(ChargeManager chargeManager)
        {
            ChargeManager = chargeManager;
            Cyclops = chargeManager.Cyclops;
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
            return ProducingPower;
        }

        public float ProducePower(float requestedPower)
        {
            if (this.BioReactors.Count == 0)
            {
                ProducingPower = false;
                return 0f;
            }

            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            float charge = 0f;

            int poweredReactors = 0;
            foreach (CyBioReactorMono reactor in this.BioReactors)
            {
                if (!reactor.HasPower)
                    continue;

                if (poweredReactors < MaxBioReactors)
                {
                    poweredReactors++;
                    reactor.OverLimit = false;

                    charge += reactor.GetBatteryPower(PowerManager.BatteryDrainRate * BioReactorRateLimiter, requestedPower);

                    tempBioCharge += reactor.Battery._charge;
                    tempBioCapacity = reactor.Battery._capacity;
                }
                else
                {
                    reactor.OverLimit = true;
                }
            }

            ProducingPower = poweredReactors > 0;

            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            return charge;
        }

    }


}
