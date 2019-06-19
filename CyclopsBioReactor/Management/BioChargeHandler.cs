namespace CyclopsBioReactor.Management
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class BioChargeHandler : ICyclopsCharger
    {
        internal const string ChargerName = "BioChrHldr";

        internal const float BatteryDrainRate = 0.01f;
        private const float BioReactorRateLimiter = 0.90f;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops, BioAuxCyclopsManager.ManagerName));

        public bool IsRenewable { get; } = false;

        public string Name { get; } = ChargerName;

        internal const int MaxBioReactors = 6;
        internal bool ProducingPower = false;

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
            return NumberFormatter.FormatNumber(Mathf.RoundToInt(totalBioCharge));
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
            if (this.Manager.CyBioReactors.Count == 0)
            {
                ProducingPower = false;
                return 0f;
            }

            float tempBioCharge = 0f;
            float tempBioCapacity = 0f;
            float charge = 0f;

            int poweredReactors = 0;
            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
            {
                if (!reactor.HasPower)
                    continue;

                if (poweredReactors < MaxBioReactors)
                {
                    poweredReactors++;

                    charge += reactor.GetBatteryPower(BatteryDrainRate * BioReactorRateLimiter, requestedPower);

                    tempBioCharge += reactor.Battery._charge;
                    tempBioCapacity = reactor.Battery._capacity;
                }
            }

            ProducingPower = poweredReactors > 0;

            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            return charge;
        }

        public float TotalReservePower()
        {
            float totalPower = 0f;
            foreach (CyBioReactorMono reactor in this.Manager.CyBioReactors)
                totalPower += reactor.Battery.charge;

            return totalPower;
        }
    }
}
