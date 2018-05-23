namespace CyclopsSolarPower
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRootPatcher
    {
        // This may seem like little, but it can actually keep a Cyclops in shallow water topped up even without the Eficiency module.
        const float baseSolarChargingFactor = 0.02f;

        // This is a copy of the private dictionary in SubRoot used to access the module slots.
        private static readonly string[] SlotNames = new string[]
        {
                "Module1",
                "Module2",
                "Module3",
                "Module4",
                "Module5",
                "Module6"
        };

        // The method UpdateThermalReactorCharge() is called on every Update() call, regardless of whether or not a ThermalReactor module is equipped or not.        
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)// && __instance.live.IsAlive()) // unable to access private variable live from here, live with it
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            // Normally, the Cyclops SetCyclopsUpgrades() method sets simple bool field for each equipped item.
            // Since it doesn't look like we have the option to inject our own private fields into the class to look at later, this is the next best thing.
            // This is the same way the SubRoot class inspects what upgrades it has equipped.

            Equipment modules = __instance.upgradeConsole.modules;
            int numberOfSolarChargers = 0; // Yes, they stack!

            for (int i = 0; i < 6; i++)
            {
                string slot = SlotNames[i];
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);                

                if (techTypeInSlot == QPatch.CySolarChargerTechType)
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
                                
                float chargeAmt = baseSolarChargingFactor * localLightScalar * proximityToSurface * numberOfSolarChargers;
                // Yes, the charge rate does scale linearly with the number of solar chargers.
                // I figure, you'd be giving up a lot of slots for good upgrades to do it so you might as well get the benefit.
                // So no need to bother with coding in dimishing returns.

                __instance.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            }
        }
    }
}
