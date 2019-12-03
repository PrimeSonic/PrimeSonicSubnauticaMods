namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CustomBatteries.Items;
    using Harmony;

    public static class ChargerPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            QuickLogger.Debug($"{nameof(ChargerPatcher)} Applying Harmony Patches");

            MethodInfo batteryChargerInitializeMethod = AccessTools.Method(typeof(BatteryCharger), nameof(BatteryCharger.Initialize));
            MethodInfo powerCellChargerInitializeMethod = AccessTools.Method(typeof(PowerCellCharger), nameof(PowerCellCharger.Initialize));

            MethodInfo initializeBatteryChargerPreFix = AccessTools.Method(typeof(ChargerPatcher), nameof(ChargerPatcher.PrefixBatteryCharger));
            MethodInfo initializePowerCellChargerPrefix = AccessTools.Method(typeof(ChargerPatcher), nameof(ChargerPatcher.PrefixPowerCellCharger));

            harmony.Patch(batteryChargerInitializeMethod, prefix: new HarmonyMethod(initializeBatteryChargerPreFix)); // Patches the BatteryCharger Initialize method
            harmony.Patch(powerCellChargerInitializeMethod, prefix: new HarmonyMethod(initializePowerCellChargerPrefix)); // Patches the PowerCellCharger Initialize method
        }

        public static void PrefixBatteryCharger()
        {
            InitializePrefix(BatteryCharger.compatibleTech, CbCore.BatteryTechTypes);
        }

        public static void PrefixPowerCellCharger()
        {
            InitializePrefix(PowerCellCharger.compatibleTech, CbCore.PowerCellTechTypes);
        }

        public static void InitializePrefix(HashSet<TechType> compatibleTech, List<TechType> toBeAdded)
        {
            if (toBeAdded.Count == 0)
                return;

            // Make sure all custom batteries are allowed in the battery charger
            if (!compatibleTech.Contains(toBeAdded[toBeAdded.Count - 1]))
            {
                // Checks in reverse order to account for the (unlikely) event that an external mod patches later than expected
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
