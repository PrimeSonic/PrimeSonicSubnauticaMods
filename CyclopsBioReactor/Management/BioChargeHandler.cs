namespace CyclopsBioReactor.Management
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class BioChargeHandler : CyclopsCharger
    {
        private const float BatteryDrainRate = 1.90f;

        private BioAuxCyclopsManager manager;
        private BioAuxCyclopsManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(Cyclops));

        internal const int MaxBioReactors = 6;

        private bool producingPower = false;
        private float totalBioCharge = 0f;
        private float totalBioCapacity = 0f;
        private float drainingEnergy = 0f;
        private float tempBioCharge = 0f;
        private float tempBioCapacity = 0f;
        private bool tempProducingPower = false;

        private readonly Atlas.Sprite sprite;

        public override float TotalReserveEnergy => this.Manager.TotalEnergyCharge;

        public BioChargeHandler(TechType cyBioBooster, SubRoot cyclops) : base(cyclops)
        {
            sprite = SpriteManager.Get(cyBioBooster);
        }

        public override Atlas.Sprite StatusSprite()
        {
            return sprite;
        }

        public override string StatusText()
        {
            return NumberFormatter.FormatValue(totalBioCharge) + (producingPower ? "+" : string.Empty);
        }

        public override Color StatusTextColor()
        {
            return NumberFormatter.GetNumberColor(totalBioCharge, totalBioCapacity, 0f);
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            if (this.Manager == null)
                return 0f;

            tempBioCharge = 0f;
            tempBioCapacity = 0f;
            tempProducingPower = false;

            this.Manager.ApplyToAll((CyBioReactorMono reactor) =>
            {
                tempBioCharge += reactor.Charge;
                tempBioCapacity = reactor.Capacity;
                tempProducingPower |= reactor.ProducingPower;
            });

            producingPower = tempProducingPower;
            totalBioCharge = tempBioCharge;
            totalBioCapacity = tempBioCapacity;

            // No energy is created but we can check for updates in this method since it always runs
            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (this.Manager == null)
                return 0f;

            drainingEnergy = 0f;

            this.Manager.ApplyToAll((CyBioReactorMono reactor) =>
            {
                drainingEnergy += reactor.GetBatteryPower(BatteryDrainRate, requestedPower);
            });

            return drainingEnergy;
        }
    }
}
