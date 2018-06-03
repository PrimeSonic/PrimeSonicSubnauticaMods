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
        private const float ChargeRate = 0.15f; // This is pretty damn fast but it makes sense for what it is.
        internal const float MaxCharge = 12000f; // Less than the normal 20k for balance

        /// <summary>
        /// Updates the nuclear battery charges and replaces them with Depleted Reactor Rods when they fully drain.
        /// </summary>
        public static void UpdateNuclearBatteryCharges(ref SubRoot __instance)
        {
            int cyclopsId = __instance.GetInstanceID();

            float powerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();

            if (powerDeficit == 0f)
            {
                return; // Don't drain on full charge
            }

            Equipment modules = __instance.upgradeConsole.modules;
            if (powerDeficit > 0) // There is still power left to charge                    
            {

                foreach (string slotName in SlotHelper.SlotNames)
                {
                    TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                    if (techTypeInSlot != NuclearCharger.CyNukBatteryType)
                        continue; // Type in equipment slot is not a nuclear battery module. Skip.

                    Battery batteryInSlot = modules.GetItemInSlot(slotName).item.GetComponent<Battery>();

                    if (batteryInSlot.charge == NoCharge) // The battery module is empty
                        continue; // Empty battery. Skip.

                    float chargeAmt = Mathf.Min(powerDeficit, ChargeRate);

                    if (batteryInSlot.charge > chargeAmt)
                    {
                        batteryInSlot.charge -= chargeAmt;
                    }
                    else // Similar to how the Nuclear Reactor handles depleated reactor rods
                    {
                        chargeAmt = batteryInSlot.charge;

                        InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                        Object.Destroy(inventoryItem.item.gameObject);
                        modules.AddItem(slotName, SpawnDepletedRod(), true);

                        batteryInSlot.charge = NoCharge;
                    }

                    powerDeficit -= chargeAmt; // This is to prevent draining more than needed when topping up the batteries mid-cycle

                    __instance.powerRelay.AddEnergy(chargeAmt, out float amtStored);
                }
            }
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
