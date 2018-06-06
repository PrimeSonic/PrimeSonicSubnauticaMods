namespace MoreCyclopsUpgrades
{
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// This class handles keeping track of the nuclear batteries.
    /// </summary>
    internal static class NuclearChargingManager
    {        
        private const float BaseChargeRate = 0.15f;        
        internal const float MaxCharge = 12000f; // Less than the normal 20k for balance

        /// <summary>
        /// Updates the nuclear battery charges and replaces them with Depleted Reactor Rods when they fully drain.
        /// </summary>
        public static void ChargeFromNuclear(ref SubRoot cyclops, Equipment modules, string slotName, ref float powerDeficit)
        {
            BatteryState batteryState = BatteryChargeManager.DrainBattery(ref cyclops, modules, slotName, BaseChargeRate, ref powerDeficit);

            if (batteryState == BatteryState.Empty) // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {                
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedRod(), true);
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
