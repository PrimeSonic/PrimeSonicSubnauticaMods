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
        private static int LastKnownAuxUpgradeConsoleCount = 0;

        [HarmonyPrefix]
        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
                return true; // Safety check

            Equipment modules = __instance.upgradeConsole.modules;

            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            // This is a dirty workaround to get a reference to the Cyclops in the AuxUpgradeConsole
            if (auxUpgradeConsoles.Length != LastKnownAuxUpgradeConsoleCount)
            {
                foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
                    auxConsole.ParentCyclops = __instance;

                if (LastKnownAuxUpgradeConsoleCount < auxUpgradeConsoles.Length)
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");

                LastKnownAuxUpgradeConsoleCount = auxUpgradeConsoles.Length;
            }

            PowerCharging.RechargeCyclops(ref __instance, modules, auxUpgradeConsoles);

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

            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            PowerIndexManager.UpdatePowerIndex(ref __instance, auxUpgradeConsoles);

            return false; // Completely override the method and do not continue with original execution
        }
    }

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetCyclopsUpgrades")]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            var cyclopsLife = (LiveMixin)__instance.GetInstanceField("live");

            if (!cyclopsLife.IsAlive())
                return; // safety check

            // This Postfix patch only handles the auxiliary upgrade consoles and isn't a full replacement
            // The original methos still handles upgrade modules in the core upgrade console            
            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
                HandleToggleableUpgrades(__instance, auxConsole.Modules);
        }

        private static void HandleToggleableUpgrades(SubRoot __instance, Equipment modules)
        {
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
                        // CyclopsThermalReactorModule handled in PowerCharging                      
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

            AuxUpgradeConsole[] auxUpgradeConsoles = __instance.GetAllComponentsInChildren<AuxUpgradeConsole>();

            float bonusCrushDepth = GetMaxBonusCrushDepth(coreModules, auxUpgradeConsoles);

            CrushDamage component = __instance.gameObject.GetComponent<CrushDamage>();
            component.SetExtraCrushDepth(bonusCrushDepth);

            return false; // Completely override the method and do not continue with original execution
            // The original method execution sucked anyways :P
        }

        private static float GetMaxBonusCrushDepth(Equipment coreModules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            float bonusCrushDepth = 0f;
            Equipment modules = coreModules;

            // Do one large loop for all upgrade consoles
            for (int moduleIndex = -1; moduleIndex < auxUpgradeConsoles.Length; moduleIndex++)
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
