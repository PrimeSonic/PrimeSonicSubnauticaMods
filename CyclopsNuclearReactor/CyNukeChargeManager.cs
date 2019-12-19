namespace CyclopsNuclearReactor
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class CyNukeChargeManager : CyclopsCharger
    {
        public const int MaxReactors = 2;

        private CyNukeManager cyNukeManager;
        private CyNukeManager CyNukeManager => cyNukeManager ?? (cyNukeManager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(base.Cyclops));

        private readonly Atlas.Sprite indicatorSprite = SpriteManager.Get(SpriteManager.Group.Category, CyNukReactorBuildable.PowerIndicatorIconID);

        public override float TotalReserveEnergy => this.CyNukeManager.TotalEnergyCharge;

        public CyNukeChargeManager(SubRoot cyclops) : base(cyclops)
        {
        }

        #region ICyclopsCharger Methods

        public override Atlas.Sprite StatusSprite()
        {
            return indicatorSprite;
        }

        public override string StatusText()
        {
            if (this.CyNukeManager.TrackedBuildablesCount == 0)
                return string.Empty;

            if (this.CyNukeManager.TrackedBuildablesCount == 1)
                return this.CyNukeManager.First.PowerIndicatorString();

            if (this.CyNukeManager.TrackedBuildablesCount == 2)
                return $"{this.CyNukeManager.Second.PowerIndicatorString()}\n{this.CyNukeManager.First.PowerIndicatorString()}";

            return string.Empty;
        }

        public override Color StatusTextColor()
        {
            if (this.CyNukeManager.TrackedBuildablesCount == 0)
                return Color.white;

            int totalActiveRods = 0;
            int maxRods = 0;

            this.CyNukeManager.ApplyToAll((reactor) =>
            {
                totalActiveRods += reactor.ActiveRodCount;
                maxRods += reactor.MaxActiveSlots;
            });

            // All slots active
            if (totalActiveRods == maxRods)
                return Color.green;

            // No slots active
            if (totalActiveRods == 0)
                return Color.white;

            // Some slots depleted
            return Color.yellow;
        }

        protected override float GenerateNewEnergy(float requestedPower)
        {
            return 0f;
        }

        protected override float DrainReserveEnergy(float requestedPower)
        {
            if (this.CyNukeManager.TrackedBuildablesCount == 0)
                return 0f;

            float powerDeficit = requestedPower;
            float producedPower = 0f;

            this.CyNukeManager.ApplyToAll((reactor) =>
            {
                if (reactor.HasPower())
                {
                    producedPower += reactor.GetPower(ref powerDeficit);
                }
            });

            return producedPower;
        }

        #endregion
    }
}
