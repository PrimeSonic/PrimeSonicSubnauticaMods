namespace CyclopsBioReactor.Management
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using System.Collections.Generic;
    using UnityEngine;

    internal partial class BioManager : ICyclopsCharger
    {
        internal readonly List<CyBioReactorMono> CyBioReactors = new List<CyBioReactorMono>();
        private readonly List<CyBioReactorMono> TempCache = new List<CyBioReactorMono>();

        internal const float BatteryDrainRate = 0.01f;
        private const float BioReactorRateLimiter = 0.90f;

        private readonly List<CyBioReactorMono> BioReactors = new List<CyBioReactorMono>();

        public bool IsRenewable { get; } = false;

        internal const int MaxBioReactors = 6;
        internal bool ProducingPower = false;

        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;

        private readonly Atlas.Sprite sprite = SpriteManager.Get(CyBioBoosterID);

        public readonly SubRoot Cyclops;


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
            throw new System.NotImplementedException();
        }



    }


}
