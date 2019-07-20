namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class CyNukeChargeManager : CyclopsCharger
    {
        public const int MaxReactors = 2;

        private CyNukeManager cyNukeManager;
        private CyNukeManager CyNukeManager => cyNukeManager ?? (cyNukeManager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(base.Cyclops));

        private List<CyNukeReactorMono> CyNukeReactors => this.CyNukeManager?.CyNukeReactors;

        private readonly Atlas.Sprite indicatorSprite = SpriteManager.Get(SpriteManager.Group.Category, CyNukReactorBuildable.PowerIndicatorIconID);

        public override float TotalReserveEnergy
        {
            get
            {
                float total = 0f;
                for (int r = 0; r < this.CyNukeReactors.Count; r++)
                    total += this.CyNukeReactors[r].GetTotalAvailablePower();

                return total;
            }
        }

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
            if (this.CyNukeReactors == null)
                return string.Empty;

            if (this.CyNukeReactors.Count == 0)
                return string.Empty;

            if (this.CyNukeReactors.Count == 1)
                return this.CyNukeReactors[0].PowerIndicatorString();

            if (this.CyNukeReactors.Count == 2)
                return $"{this.CyNukeReactors[0].PowerIndicatorString()}\n{this.CyNukeReactors[1].PowerIndicatorString()}";

            string value = string.Empty;
            for (int i = 0; i < this.CyNukeReactors.Count; i++)
                value += $"{this.CyNukeReactors[i].PowerIndicatorString()}\n";

            return value;
        }

        public override Color StatusTextColor()
        {
            if (this.CyNukeReactors == null || this.CyNukeReactors.Count == 0)
                return Color.white;

            int totalActiveRods = 0;
            int maxRods = 0;

            for (int r = 0; r < this.CyNukeReactors.Count; r++)
            {
                CyNukeReactorMono reactor = this.CyNukeReactors[r];
                totalActiveRods += reactor.ActiveRodCount;
                maxRods += reactor.MaxActiveSlots;
            }

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
            if (this.CyNukeReactors == null || this.CyNukeReactors.Count == 0)
                return 0f;

            float powerDeficit = requestedPower;
            float producedPower = 0f;

            for (int r = 0; r < this.CyNukeReactors.Count; r++)
            {
                CyNukeReactorMono reactor = this.CyNukeReactors[r];
                if (!reactor.HasPower())
                    continue;

                producedPower += reactor.GetPower(ref powerDeficit);
            }

            return producedPower;
        }

        #endregion
    }
}
