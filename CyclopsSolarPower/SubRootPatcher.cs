namespace CyclopsSolarPower
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRootPatcher
    {
        const float baseSolarChargingFactor = 0.01f;

        // This is a replication of the private dictionary in SubRoot
        private static readonly string[] SlotNames = new string[]
        {
                "Module1",
                "Module2",
                "Module3",
                "Module4",
                "Module5",
                "Module6"
        };

        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)// && __instance.live.IsAlive()) // unable to access private variable live from here
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }            

            Equipment modules = __instance.upgradeConsole.modules;
            int numberOfSolarChargers = 0;
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
                // The code here mostly replicates the UpdateSolarRecharge() method from the SeaMoth class,
                // with consessions made for the differences between the Seamoth and Cyclops upgrade modules.
                DayNightCycle main = DayNightCycle.main;
                if (main == null)
                {
                    return; // This was probably put here for safety
                }

                float proximityToSurface = Mathf.Clamp01((200f + __instance.transform.position.y) / 200f);
                float localLightScalar = main.GetLocalLightScalar();                

                float chargeAmt = baseSolarChargingFactor * localLightScalar * proximityToSurface * numberOfSolarChargers;
                __instance.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            }
        }
    }
}
