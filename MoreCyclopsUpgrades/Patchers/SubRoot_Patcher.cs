namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using Harmony;
    using UnityEngine;
    using SMLHelper.V2.Utility;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
                return true; // Safety check

            Equipment modules = __instance.upgradeConsole.modules;

            PowerManager.RechargeCyclops(ref __instance, modules, UpgradeConsoleCache.AuxUpgradeConsoles);

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
            if (__instance.upgradeConsole == null)
                return true; // safety check

            PowerManager.UpdatePowerSpeedRating(ref __instance, UpgradeConsoleCache.AuxUpgradeConsoles);

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

            if (__instance.upgradeConsole == null || !cyclopsLife.IsAlive())
                return true; // safety check

            __instance.shieldUpgrade = false;
            __instance.sonarUpgrade = false;
            __instance.vehicleRepairUpgrade = false;
            __instance.decoyTubeSizeIncreaseUpgrade = false;

            HandleToggleableUpgrades(__instance, __instance.upgradeConsole.modules);

            foreach (AuxUpgradeConsole auxConsole in UpgradeConsoleCache.AuxUpgradeConsoles)
                HandleToggleableUpgrades(__instance, auxConsole.Modules);

            // No need to execute original method anymore
            return false; // Completely override the method and do not continue with original execution
        }

        private static void HandleToggleableUpgrades(SubRoot __instance, Equipment modules)
        {
            var subControl = __instance.GetAllComponentsInChildren<SubControl>();

            List<TechType> upgradeList = new List<TechType>(SlotHelper.SlotNames.Length);

            foreach (string slot in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);
                switch (techTypeInSlot)
                {
                    case TechType.CyclopsShieldModule:
                        __instance.shieldUpgrade = true;
                        break;
                    case TechType.CyclopsSonarModule:
                        __instance.sonarUpgrade = true;
                        break;
                    case TechType.CyclopsSeamothRepairModule:
                        __instance.vehicleRepairUpgrade = true;
                        break;
                    case TechType.CyclopsDecoyModule:
                        __instance.decoyTubeSizeIncreaseUpgrade = true;
                        break;
                        // CyclopsThermalReactorModule handled in PowerManager.RechargeCyclops
                        // CyclopsSpeedModule handled in PowerManager.UpdatePowerSpeedRating
                }

                upgradeList.Add(techTypeInSlot);
            }

            if (__instance.slotModSFX != null)
            {
                __instance.slotModSFX.Play();
            }

            __instance.BroadcastMessage("RefreshUpgradeConsoleIcons", upgradeList.ToArray(), SendMessageOptions.RequireReceiver);
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetExtraDepth")]
    internal class SubRoot_SetExtraDepth_Patcher
    {
        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
                return true; // safety check

            Equipment coreModules = __instance.upgradeConsole.modules;

            float bonusCrushDepth = GetMaxBonusCrushDepth(coreModules, UpgradeConsoleCache.AuxUpgradeConsoles);

            CrushDamage component = __instance.gameObject.GetComponent<CrushDamage>();
            component.SetExtraCrushDepth(bonusCrushDepth);

            return false; // Completely override the method and do not continue with original execution
            // The original method execution sucked anyways :P
        }

        private static float GetMaxBonusCrushDepth(Equipment coreModules, IList<AuxUpgradeConsole> auxUpgradeConsoles)
        {
            float bonusCrushDepth = 0f;
            Equipment modules = coreModules;

            // Do one large loop for all upgrade consoles
            for (int moduleIndex = -1; moduleIndex < auxUpgradeConsoles.Count; moduleIndex++)
            {
                if (moduleIndex > -1)
                    modules = auxUpgradeConsoles[moduleIndex].Modules;

                foreach (string slot in SlotHelper.SlotNames)
                {
                    TechType techTypeInSlot = modules.GetTechTypeInSlot(slot);

                    if (ExtraCrushDepths.ContainsKey(techTypeInSlot))
                        bonusCrushDepth = Mathf.Max(bonusCrushDepth, ExtraCrushDepths[techTypeInSlot]);
                }
            }

            return bonusCrushDepth;
        }

        // This is a straight copy of the values in the original
        private static readonly Dictionary<TechType, float> ExtraCrushDepths = new Dictionary<TechType, float>
        {
            { TechType.HullReinforcementModule, 800f },
            { TechType.HullReinforcementModule2, 1600f },
            { TechType.HullReinforcementModule3, 2800f },
            { TechType.CyclopsHullModule1, 400f },
            { TechType.CyclopsHullModule2, 800f },
            { TechType.CyclopsHullModule3, 1200f }
        };
    }

}
