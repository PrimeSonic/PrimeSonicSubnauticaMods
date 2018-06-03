namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// This class handles keeping track of the nuclear batteries.
    /// </summary>
    internal static class NuclearChargingManager
    {
        private class NuclearBatterySlots : Dictionary<int, Battery>
        {
            public NuclearBatterySlots() : base(SlotHelper.SlotCount)
            {
                this[0] = null;
                this[1] = null;
                this[2] = null;
                this[3] = null;
                this[4] = null;
                this[5] = null;
            }
        }

        private const float NoCharge = 0f;        
        private const float ChargeRate = 0.15f; // This is pretty damn fast but it makes sense for what it is.

        internal const float MaxCharge = 12000f; // Less than the normal 20k for balance

        // Just in case someone decides to use this mod with more than one Cyclops in the game.
        private static readonly Dictionary<int, NuclearBatterySlots> CyclopsConsoles = new Dictionary<int, NuclearBatterySlots>();

        /// <summary>
        /// Keeps track of the nuclear batteries so they can be easily updated as they charge the Cyclops.
        /// </summary>        
        public static void SetNuclearBatterySlots(ref SubRoot __instance)
        {
            int cyclopsId = __instance.GetInstanceID();

            if (!CyclopsConsoles.ContainsKey(cyclopsId))
                CyclopsConsoles[cyclopsId] = new NuclearBatterySlots();

            Equipment modules = __instance.upgradeConsole.modules;

            for (int slot = 0; slot < SlotHelper.SlotCount; slot++)
            {
                string slotName = SlotHelper.SlotNames[slot];

                TechType typeInSlot = modules.GetTechTypeInSlot(slotName);

                var item = modules.GetItemInSlot(slotName);

                if (typeInSlot == TechType.None && // Slot is now empty
                    CyclopsConsoles[cyclopsId][slot] != null) // Nuclear battery for this slot was not empty
                {
                    // Remove nuclear battery from slot
                    CyclopsConsoles[cyclopsId][slot] = null;
                }
                else if (typeInSlot == NuclearCharger.CyNukBatteryType && // Slot now has a Cyclops Nuclear Module
                    CyclopsConsoles[cyclopsId][slot] == null) // There was no nuclear battery on this slot
                {
                    // Add nuclear battery to slot
                    CyclopsConsoles[cyclopsId][slot] = item.item.GetComponent<Battery>();
                }
            }
        }

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

            for (int slot = 0; slot < SlotHelper.SlotCount; slot++)
            {
                string slotName = SlotHelper.SlotNames[slot];

                var bat = modules.GetItemInSlot(slotName);

                var batteryInSlot = CyclopsConsoles[cyclopsId][slot];

                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (batteryInSlot == null || // No known battery reference
                    techTypeInSlot != NuclearCharger.CyNukBatteryType || // Type is equipment slot is not a nuclear battery module
                    batteryInSlot.charge == NoCharge) // The battery module is empty
                    continue; // Skip this slot

                if (powerDeficit > 0) // There is still power left to charge                    
                {
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
