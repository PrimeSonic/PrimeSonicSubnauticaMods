namespace MoreCyclopsUpgrades
{
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// This class handles keeping track of the nuclear batteries.
    /// </summary>
    internal static class NuclearChargingManager
    {
        private const float NoCharge = 0f;
        private const float BaseChargeRate = 0.15f;        
        internal const float MaxCharge = 12000f; // Less than the normal 20k for balance

        /// <summary>
        /// Updates the nuclear battery charges and replaces them with Depleted Reactor Rods when they fully drain.
        /// </summary>
        public static void ChargeFromNuclear(ref SubRoot cyclops, Equipment modules, string slotName, ref float powerDeficit)
        {
            if (powerDeficit <= 0f) // No power deficit left to charge
                return; // Exit

            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            if (batteryInSlot.charge == NoCharge) // The battery has no charge left
                return; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, BaseChargeRate);

            if (batteryInSlot.charge > chargeAmt)
            {
                batteryInSlot.charge -= chargeAmt;
            }
            else // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {
                chargeAmt = batteryInSlot.charge;
                batteryInSlot.charge = NoCharge; // Just in case something goes wrong below

                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedRod(), true);
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }

        private static InventoryItem SpawnDepletedRod()
        {
            GameObject prefabForTechType = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod, true);
            GameObject gameObject = Object.Instantiate(prefabForTechType);
            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }

}
