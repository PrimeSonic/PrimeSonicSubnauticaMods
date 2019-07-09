namespace CyclopsBioReactor.Management
{
    using CommonCyclopsUpgrades;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class BioChargeHandler : ICyclopsCharger
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float BatteryDrainRate = 1.90f;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops));

        internal const int MaxBioReactors = 6;

        private bool providingPower = false;
        private bool producingPower = false;
        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        private readonly Atlas.Sprite sprite;

        public readonly SubRoot Cyclops;

        public BioChargeHandler(TechType cyBioBooster, SubRoot cyclops)
        {
            sprite = SpriteManager.Get(cyBioBooster);
            Cyclops = cyclops;
        }

        public Atlas.Sprite GetIndicatorSprite()
        {
            return sprite;
        }

        public string GetIndicatorText()
        {
            return NumberFormatter.FormatValue(totalBioCharge) + (producingPower ? "+" : string.Empty);
        }

        public Color GetIndicatorTextColor()
        {
            return NumberFormatter.GetNumberColor(totalBioCharge, totalBioCapacity, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return providingPower;
        }

        public float ProducePower(float requestedPower)
        {
            if (requestedPower < MinimalPowerValue || this.Manager.CyBioReactors.Count == 0)
            {
                providingPower = false;
                return 0f;
            }

            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            float charge = 0f;
            bool currentlyProducingPower = false;

            int poweredReactors = 0;
            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
            {
                if (!reactor.HasPower)
                    continue;

                if (poweredReactors < MaxBioReactors)
                {
                    poweredReactors++;

                    charge += reactor.GetBatteryPower(BatteryDrainRate, requestedPower);

                    tempBioCharge += reactor.Charge;
                    tempBioCapacity = reactor.Capacity;
                    currentlyProducingPower |= reactor.ProducingPower;
                }
            }

            providingPower = poweredReactors > 0;
            producingPower = currentlyProducingPower;
            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            return charge;
        }

        public float TotalReservePower()
        {
            float totalPower = 0f;
            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
                totalPower += reactor.Charge;

            return totalPower;
        }
    }
}
