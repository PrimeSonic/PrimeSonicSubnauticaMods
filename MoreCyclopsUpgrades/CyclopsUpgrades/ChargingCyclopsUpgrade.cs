namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Managers;
    using UnityEngine;

    internal class ChargingUpgradeHandler : UpgradeHandler
    {
        public ChargingUpgradeHandler(TechType techType) : base(techType)
        {
            IsPowerProducer = true;
        }

        public virtual float ChargeCyclops(SubRoot cyclops, ref float availablePower, ref float powerDeficit)
        {
            if (this.Count == 0)
                return 0f;

            if (powerDeficit < PowerManager.MinimalPowerValue)
                return availablePower; // Surplus power

            if (availablePower < PowerManager.MinimalPowerValue)
                return 0f;

            availablePower *= this.Count;

            cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);

            return Mathf.Max(0f, availablePower - powerDeficit); // Surplus power
        }
    }
}
