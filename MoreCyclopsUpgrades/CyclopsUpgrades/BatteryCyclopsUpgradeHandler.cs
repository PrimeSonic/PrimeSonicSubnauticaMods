namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class BatteryCyclopsUpgradeHandler : ChargingUpgradeHandler
    {
        public delegate void BatteryDrainedEvent(BatteryDetails details);
        public BatteryDrainedEvent OnBatteryDrained;

        internal const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.

        internal IList<BatteryDetails> Batteries { get; } = new List<BatteryDetails>();

        internal readonly bool BatteryRecharges;

        public float TotalBatteryCharge { get; protected set; } = 0f;
        public float TotalBatteryCapacity { get; protected set; } = 0f;

        internal bool BatteryHasCharge => this.Count > 0 && this.TotalBatteryCharge > MinimalPowerValue;

        public BatteryCyclopsUpgradeHandler(TechType techType, bool canRecharge) : base(techType)
        {
            BatteryRecharges = canRecharge;
            OnClearUpgrades += (SubRoot cyclops) =>
            {
                this.TotalBatteryCharge = 0f;
                this.TotalBatteryCapacity = 0f;
                this.Batteries.Clear();
            };

            OnUpgradeCounted += (SubRoot cyclops, Equipment modules, string slot) =>
            {
                var details = new BatteryDetails(modules, slot, modules.GetItemInSlot(slot).item.GetComponent<Battery>());
                this.Batteries.Add(details);
                this.TotalBatteryCapacity += details.BatteryRef._capacity;
            };
        }

        public override float ChargeCyclops(SubRoot cyclops, ref float availablePower, ref float powerDeficit)
        {
            if (this.Count == 0)
                return 0f;

            availablePower *= Mk2ChargeRateModifier;

            this.TotalBatteryCharge = 0f;
            foreach (BatteryDetails details in this.Batteries)
            {
                cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);

                Battery battery = details.BatteryRef;

                battery._charge = Mathf.Min(battery._capacity, battery._charge + availablePower);
                this.TotalBatteryCharge += battery._charge;
            }

            return Mathf.Max(0f, availablePower - powerDeficit); // Surplus power
        }

        public void ChargeCyclops(SubRoot cyclops, float drainingRate, ref float powerDeficit)
        {
            if (powerDeficit < MinimalPowerValue) // No power deficit left to charge
                return; // Exit

            this.TotalBatteryCharge = 0f;
            foreach (BatteryDetails details in this.Batteries)
            {
                Battery battery = details.BatteryRef;

                if (battery._charge < MinimalPowerValue) // The battery has no charge left
                    continue; // Skip this battery

                // Mathf.Min is to prevent accidentally taking too much power from the battery
                float chargeAmt = Mathf.Min(powerDeficit, drainingRate);

                if (battery._charge > chargeAmt)
                {
                    battery._charge -= chargeAmt;
                }
                else // Battery about to be fully drained
                {
                    chargeAmt = battery._charge; // Take what's left
                    battery._charge = 0f; // Set battery to empty

                    OnBatteryDrained?.Invoke(details);
                }

                this.TotalBatteryCharge += battery._charge;
                powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            }
        }

        public void RechargeBatteries(ref float surplusPower)
        {
            if (!BatteryRecharges)
                return;

            this.TotalBatteryCharge = 0f;
            bool batteryCharged = false;
            foreach (BatteryDetails details in this.Batteries)
            {
                this.TotalBatteryCharge += details.BatteryRef._charge;

                if (batteryCharged)
                    continue;

                if (surplusPower < MinimalPowerValue)
                    continue;

                if (details.IsFull)
                    continue;

                Battery batteryToCharge = details.BatteryRef;
                batteryToCharge._charge = Mathf.Min(batteryToCharge._capacity, batteryToCharge._charge + surplusPower);
                surplusPower -= (batteryToCharge._capacity - batteryToCharge._charge);
                batteryCharged = true;
            }
        }
    }
}
