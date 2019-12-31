namespace MidGameBatteries.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using CustomBatteries.Items;
    using Harmony;

    internal static class EnergyMixinPatcher
    {
        internal static void Patch(HarmonyInstance harmony)
        {
            QuickLogger.Debug($"{nameof(EnergyMixinPatcher)} Applying Harmony Patches");

            MethodInfo energyMixinNotifyHasBattery = AccessTools.Method(typeof(EnergyMixin), nameof(EnergyMixin.NotifyHasBattery));
            MethodInfo notifyHasBatteryPostfixMethod = AccessTools.Method(typeof(EnergyMixinPatcher), nameof(EnergyMixinPatcher.NotifyHasBatteryPostfix));

            var harmonyNotifyPostfix = new HarmonyMethod(notifyHasBatteryPostfixMethod);
            harmony.Patch(energyMixinNotifyHasBattery, postfix: harmonyNotifyPostfix); // Patches the EnergyMixin NotifyHasBattery method

            MethodInfo energyMixStartMethod = AccessTools.Method(typeof(EnergyMixin), nameof(EnergyMixin.Start));
            MethodInfo startPostfixMethod = AccessTools.Method(typeof(EnergyMixinPatcher), nameof(EnergyMixinPatcher.StartPostfix));

            var harmonyStartPostfix = new HarmonyMethod(startPostfixMethod);
            harmony.Patch(energyMixStartMethod, postfix: harmonyStartPostfix); // Patches the EnergyMixin Start method
        }

        public static void NotifyHasBatteryPostfix(ref EnergyMixin __instance, InventoryItem item)
        {
            if (CbCore.PowerCellTechTypes.Count == 0)
                return;

            // For vehicles that show a battery model when one is equipped,
            // this will replicate the model for the normal Power Cell so it doesn't look empty

            // Null checks added on every step of the way
            TechType? itemInSlot = item?.item?.GetTechType();

            if (itemInSlot.HasValue && CbCore.PowerCellTechTypes.Contains(itemInSlot.Value))
                __instance.batteryModels[0].model.SetActive(true);

            // Perhaps later a more suiteable model could be added with a more appropriate skin.
            // This is functional for now.
        }

        public static void StartPostfix(ref EnergyMixin __instance)
        {
            // This is necessary to allow the new batteries to be compatible with tools and vehicles

            if (!__instance.allowBatteryReplacement)
                return; // Battery replacement not allowed - No need to make changes

            if (CbCore.BatteryTechTypes.Count == 0)
                return;

            List<TechType> compatibleBatteries = __instance.compatibleBatteries;

            if (compatibleBatteries.Contains(TechType.Battery) &&
                !compatibleBatteries.Contains(CbCore.LastModdedBattery))
            {
                // If the regular Battery is compatible with this item, then modded batteries should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbCore.BatteryTechTypes);
                return;
            }

            if (compatibleBatteries.Contains(TechType.PowerCell) &&
                !compatibleBatteries.Contains(CbCore.LastModdedPowerCell))
            {
                // If the regular Power Cell is compatible with this item, then modded power cells should also be compatible
                AddMissingTechTypesToList(compatibleBatteries, CbCore.PowerCellTechTypes);
            }
        }

        private static void AddMissingTechTypesToList(List<TechType> compatibleTechTypes, List<TechType> toBeAdded)
        {
            for (int i = toBeAdded.Count - 1; i >= 0; i--)
            {
                TechType entry = toBeAdded[i];

                if (compatibleTechTypes.Contains(entry))
                    return;

                compatibleTechTypes.Add(entry);
            }
        }
    }
}
