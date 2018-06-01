namespace CyclopsNuclearPower
{
    using System.Collections.Generic;    
    using Harmony;
    using UnityEngine;

    internal static class CyNukReactor
    {
        public const string Slot1 = "Module1";
        public const string Slot2 = "Module2";
        public const string Slot3 = "Module3";
        public const string Slot4 = "Module4";
        public const string Slot5 = "Module5";
        public const string Slot6 = "Module6";

        internal const float NoCharge = 0f;
        internal const float MaxCharge = 6000f;        

        internal const float ChargeRate = 0.1f;

        internal static readonly Dictionary<string, Battery> ReactorBatteries = new Dictionary<string, Battery>(6)
        {
               { Slot1, null },
               { Slot2, null },
               { Slot3, null },
               { Slot4, null },
               { Slot5, null },
               { Slot6, null }
        };
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher2
    {
        // This is a copy of the private dictionary in SubRoot used to access the module slots.
        internal static readonly string[] SlotNames = new string[]
        {
                CyNukReactor.Slot1,
                CyNukReactor.Slot2,
                CyNukReactor.Slot3,
                CyNukReactor.Slot4,
                CyNukReactor.Slot5,
                CyNukReactor.Slot6
        };

        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            bool cyclopsHasPowerCells = __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Normal;

            if (!cyclopsHasPowerCells)
            {
                // Don't drain if there are no batteries to charge
                // Potential issue if the player somehow managed to actually drain all their power cells
                return;
            }

            float powerDeficit = Mathf.Abs(__instance.powerRelay.GetPower() - __instance.powerRelay.GetMaxPower());

            if (powerDeficit == 0f)
            {
                return; // Don't drain on full charge
            }

            Equipment modules = __instance.upgradeConsole.modules;

            for (int i = 0; i < 6; i++)
            {
                string slot = SlotNames[i];
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == QPatch.CyReactorRodType && powerDeficit > 0 &&
                    CyNukReactor.ReactorBatteries[slot].charge > CyNukReactor.NoCharge)
                {
                    float chargeAmt = CyNukReactor.ChargeRate;

                    if (CyNukReactor.ReactorBatteries[slot].charge > chargeAmt)
                        CyNukReactor.ReactorBatteries[slot].charge -= chargeAmt;
                    else // Similar to how the Nuclear Reactor does this
                    {                        
                        chargeAmt = CyNukReactor.ReactorBatteries[slot].charge;

                        InventoryItem inventoryItem = modules.RemoveItem(slot, true, false);
                        UnityEngine.Object.Destroy(inventoryItem.item.gameObject);
                        modules.AddItem(slot, SpawnDepletedRod(), true);

                        CyNukReactor.ReactorBatteries[slot].charge = CyNukReactor.NoCharge;                        
                    }

                    powerDeficit -= chargeAmt;

                    __instance.powerRelay.AddEnergy(chargeAmt, out float amtStored);
                }
            }

        }

        private static InventoryItem SpawnDepletedRod()
        {
            GameObject prefabForTechType = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod, true);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefabForTechType);
            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("OnSubModulesChanged")]
    internal class SubRoot_OnSubModulesChanged_Patcher
    {
        public static void Postfix(ref SubRoot __instance, ref string slot, ref InventoryItem item)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            Equipment modules = __instance.upgradeConsole.modules;

            if (item.item == null)
            {
                return;
            }

            TechType itemType = item.item.GetTechType();            

            if (itemType != QPatch.CyReactorRodType)
            {
                return; // This method only handled the Cyclops reactor rods
            }

            var typeInSlot = modules.GetTechTypeInSlot(slot);            

            if (typeInSlot == TechType.None && CyNukReactor.ReactorBatteries[slot] != null)
            {
                // CyclopsNuclearModule Removed
                CyNukReactor.ReactorBatteries[slot] = null;
            }
            else if (typeInSlot == QPatch.CyReactorRodType && CyNukReactor.ReactorBatteries[slot] == null)
            {
                // CyclopsNuclearModule Added
                var battery = item.item.GetComponent<Battery>();
                CyNukReactor.ReactorBatteries[slot] = battery;
            }

        }
    }

}
