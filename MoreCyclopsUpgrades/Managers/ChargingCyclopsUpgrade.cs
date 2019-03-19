namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using UnityEngine;

    internal class ChargingCyclopsUpgrade : CyclopsUpgrade
    {
        public const float MinimalPowerValue = 0.001f;

        public ChargingCyclopsUpgrade(TechType techType) : base(techType)
        {
            IsPowerProducer = true;
        }

        public virtual float ChargeCyclops(SubRoot cyclops, ref float availablePower, ref float powerDeficit)
        {
            if (this.Count == 0)
                return 0f;

            if (powerDeficit < MinimalPowerValue)
                return availablePower; // Surplus power

            if (availablePower < MinimalPowerValue)
                return 0f;

            availablePower *= this.Count;

            cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);

            return Mathf.Max(0f, availablePower - powerDeficit); // Surplus power
        }
    }
}
