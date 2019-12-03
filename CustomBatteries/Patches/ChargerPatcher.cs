namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using Harmony;

    public static class ChargerPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            QuickLogger.Debug($"{nameof(ChargerPatcher)} Applying Harmony Patches");

            MethodInfo batteryChargerInitializeMethod = AccessTools.Method(typeof(BatteryCharger), nameof(BatteryCharger.Initialize));
            MethodInfo powerCellChargerInitializeMethod = AccessTools.Method(typeof(PowerCellCharger), nameof(PowerCellCharger.Initialize));

            MethodInfo initializePreFixMethod = AccessTools.Method(typeof(ChargerPatcher), nameof(ChargerPatcher.InitializePrefix));

            harmony.Patch(batteryChargerInitializeMethod, prefix: new HarmonyMethod(initializePreFixMethod)); // Patches the BatteryCharger Initialize method
            harmony.Patch(powerCellChargerInitializeMethod, prefix: new HarmonyMethod(initializePreFixMethod)); // Patches the PowerCellCharger Initialize method
        }

        public static void InitializePrefix(HashSet<TechType> compatibleTech, List<TechType> toBeAdded)
        {
            if (toBeAdded.Count == 0)
                return;

            // Make sure all custom batteries are allowed in the battery charger
            if (!compatibleTech.Contains(toBeAdded[toBeAdded.Count - 1]))
            {
                for (int i = toBeAdded.Count - 1; i >= 0; i--)
                {
                    TechType entry = toBeAdded[i];
                    if (compatibleTech.Contains(entry))
                        return;

                    compatibleTech.Add(entry);
                }
            }
        }
    }
}
