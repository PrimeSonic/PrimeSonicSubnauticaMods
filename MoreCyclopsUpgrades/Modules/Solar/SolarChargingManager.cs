namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    internal static class SolarChargingManager
    {
        // This may seem like little, but it can actually keep a Cyclops in shallow water topped up even without the Eficiency module.
        const float baseSolarChargingFactor = 0.03f;

        internal static float UserChargeRate { get; set; } = 1f;

        public static void UpdateSolarCharger(ref SubRoot __instance)
        {
            Equipment modules = __instance.upgradeConsole.modules;
            int numberOfSolarChargers = 0; // Yes, they stack!

            for (int i = 0; i < SlotHelper.SlotCount; i++)
            {
                string slot = SlotHelper.SlotNames[i];
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                if (techTypeInSlot == SolarCharger.CySolarChargerTechType)
                {
                    numberOfSolarChargers++; // Yes, they stack!                    
                }
            }

            if (numberOfSolarChargers > 0)
            {
                // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
                // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.
                DayNightCycle main = DayNightCycle.main;
                if (main == null)
                {
                    return; // This was probably put here for safety
                }

                // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.
                float proximityToSurface = Mathf.Clamp01((200f + __instance.transform.position.y) / 200f);
                float localLightScalar = main.GetLocalLightScalar();

                float chargeAmt = baseSolarChargingFactor * localLightScalar * proximityToSurface * numberOfSolarChargers * UserChargeRate;
                // Yes, the charge rate does scale linearly with the number of solar chargers.
                // I figure, you'd be giving up a lot of slots for good upgrades to do it so you might as well get the benefit.
                // So no need to bother with coding in dimishing returns.

                __instance.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            }
        }
    }
}
