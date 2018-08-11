namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using Harmony;
    using UnityEngine;
    using SMLHelper.V2.Utility;
    using Caching;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            PowerManager.RechargeCyclops(ref __instance);

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            PowerManager.UpdatePowerSpeedRating(ref __instance);

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetCyclopsUpgrades")]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            var cyclopsLife = (LiveMixin)__instance.GetInstanceField("live");

            if (!cyclopsLife.IsAlive())
                return true; // safety check

            if (!UpgradeConsoleCache.IsSynced(__instance)) // Because this might run before CyclopsUpgradeConsoleHUDManager.RefreshScreen
                UpgradeConsoleCache.SyncUpgradeConsoles(__instance); // Be prepared to sync here once

            __instance.shieldUpgrade = false;
            __instance.sonarUpgrade = false;
            __instance.vehicleRepairUpgrade = false;
            __instance.decoyTubeSizeIncreaseUpgrade = false;

            bool hasFireSupression = false;

            UpgradeConsoleCache.ClearModuleCache();
            
            foreach (Equipment modules in UpgradeConsoleCache.UpgradeConsoles)
                HandleUpgrades(__instance, modules, ref hasFireSupression);

            var cyclopsHUD = __instance.GetComponentInChildren<CyclopsHolographicHUD>();
            cyclopsHUD.fireSuppressionSystem.SetActive(hasFireSupression);

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }

        private static void HandleUpgrades(SubRoot cyclops, Equipment modules, ref bool fireSupressionSystem)
        {
            List<TechType> upgradeList = new List<TechType>(SlotHelper.SlotNames.Length);

            foreach (string slot in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                // Handle standard modules

                switch (techTypeInSlot)
                {
                    case TechType.CyclopsShieldModule:
                        cyclops.shieldUpgrade = true;
                        break;
                    case TechType.CyclopsSonarModule:
                        cyclops.sonarUpgrade = true;
                        break;
                    case TechType.CyclopsSeamothRepairModule:
                        cyclops.vehicleRepairUpgrade = true;
                        break;
                    case TechType.CyclopsDecoyModule:
                        cyclops.decoyTubeSizeIncreaseUpgrade = true;
                        break;
                    case TechType.CyclopsFireSuppressionModule:
                        fireSupressionSystem = true;
                        break;
                    case TechType.CyclopsThermalReactorModule:
                        UpgradeConsoleCache.AddThermalModule();
                        break;
                    case TechType.PowerUpgradeModule:
                        UpgradeConsoleCache.AddPowerMk1Module();
                        break;
                    case TechType.HullReinforcementModule:
                    case TechType.HullReinforcementModule2:
                    case TechType.HullReinforcementModule3:
                    case TechType.CyclopsHullModule1:
                    case TechType.CyclopsHullModule2:
                    case TechType.CyclopsHullModule3:
                        UpgradeConsoleCache.AddDepthModule(techTypeInSlot);
                        break;
                }

                // Handle modded modules

                if (techTypeInSlot == CyclopsModule.SolarChargerID) // Solar
                {
                    UpgradeConsoleCache.AddSolarModule();
                }
                else if (techTypeInSlot == CyclopsModule.SolarChargerMk2ID) // Solar Mk2
                {
                    UpgradeConsoleCache.AddSolarMk2Module(PowerManager.GetBatteryInSlot(modules, slot));
                }
                else if (techTypeInSlot == CyclopsModule.ThermalChargerMk2ID) // Thermal Mk2
                {
                    UpgradeConsoleCache.AddThermalMk2Module(PowerManager.GetBatteryInSlot(modules, slot));
                }
                else if (techTypeInSlot == CyclopsModule.NuclearChargerID) // Nuclear
                {
                    UpgradeConsoleCache.AddNuclearModule(modules, slot, PowerManager.GetBatteryInSlot(modules, slot));
                }
                else if (techTypeInSlot == CyclopsModule.SpeedBoosterModuleID) // Speed booster
                {
                    UpgradeConsoleCache.AddSpeedModule();
                }
                else if (techTypeInSlot == CyclopsModule.PowerUpgradeMk2ID) // Power MK2
                {
                    UpgradeConsoleCache.AddPowerMk2Module();
                }
                else if (techTypeInSlot == CyclopsModule.PowerUpgradeMk3ID) // Power Mk3
                {
                    UpgradeConsoleCache.AddPowerMk3Module();
                }

                upgradeList.Add(techTypeInSlot);
            }

            if (cyclops.slotModSFX != null)
            {
                cyclops.slotModSFX.Play();
            }

            cyclops.BroadcastMessage("RefreshUpgradeConsoleIcons", upgradeList.ToArray(), SendMessageOptions.RequireReceiver);
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetExtraDepth")]
    internal class SubRoot_SetExtraDepth_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            CrushDamage component = __instance.gameObject.GetComponent<CrushDamage>();

            component.SetExtraCrushDepth(UpgradeConsoleCache.BonusCrushDepth);

            return false; // Completely override the method and do not continue with original execution
            // The original method execution sucked anyways :P
        }


    }

}
