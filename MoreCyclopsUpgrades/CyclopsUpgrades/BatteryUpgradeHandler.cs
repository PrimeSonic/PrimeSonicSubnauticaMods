namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Managers;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BatteryUpgradeHandler : ChargingUpgradeHandler
    {
        public delegate void BatteryDrainedEvent(BatteryDetails details);
        public BatteryDrainedEvent OnBatteryDrained;

        internal IList<BatteryDetails> Batteries { get; } = new List<BatteryDetails>();

        internal readonly bool BatteryRecharges;
        private float tempTotalCharge;
        public float TotalBatteryCharge { get; protected set; } = 0f;
        public float TotalBatteryCapacity { get; protected set; } = 0f;

        internal bool BatteryHasCharge => this.Count > 0 && this.TotalBatteryCharge > PowerManager.MinimalPowerValue;

        public BatteryUpgradeHandler(TechType techType, bool canRecharge) : base(techType)
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
                this.TotalBatteryCharge += details.BatteryRef._charge;
                this.TotalBatteryCapacity += details.BatteryRef._capacity;
            };
        }

        public override float ChargeCyclops(SubRoot cyclops, ref float availablePower, ref float powerDeficit)
        {
            if (this.Count == 0)
                return 0f;

            availablePower *= PowerManager.Mk2ChargeRateModifier;

            tempTotalCharge = 0f;
            foreach (BatteryDetails details in this.Batteries)
            {
                cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);

                Battery battery = details.BatteryRef;

                battery._charge = Mathf.Min(battery._capacity, battery._charge + availablePower);
                tempTotalCharge += battery._charge;
            }

            this.TotalBatteryCharge = tempTotalCharge;
            return Mathf.Max(0f, availablePower - powerDeficit); // Surplus power
        }

        public void ChargeCyclops(SubRoot cyclops, float drainingRate, ref float powerDeficit)
        {
            if (powerDeficit < PowerManager.MinimalPowerValue) // No power deficit left to charge
                return; // Exit

            tempTotalCharge = 0f;
            foreach (BatteryDetails details in this.Batteries)
            {
                Battery battery = details.BatteryRef;

                if (battery._charge < PowerManager.MinimalPowerValue) // The battery has no charge left
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

                tempTotalCharge += battery._charge;
                powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            }
            this.TotalBatteryCharge = tempTotalCharge;
        }

        public void RechargeBatteries(ref float surplusPower)
        {
            if (!BatteryRecharges)
                return;

            tempTotalCharge = 0f;
            bool batteryCharged = false;
            foreach (BatteryDetails details in this.Batteries)
            {
                tempTotalCharge += details.BatteryRef._charge;

                if (batteryCharged)
                    continue;

                if (surplusPower < PowerManager.MinimalPowerValue)
                    continue;

                if (details.IsFull)
                    continue;

                Battery batteryToCharge = details.BatteryRef;
                batteryToCharge._charge = Mathf.Min(batteryToCharge._capacity, batteryToCharge._charge + surplusPower);
                surplusPower -= (batteryToCharge._capacity - batteryToCharge._charge);
                batteryCharged = true;
            }

            this.TotalBatteryCharge = tempTotalCharge;
        }
    }
}
