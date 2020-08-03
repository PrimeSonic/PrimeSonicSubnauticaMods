namespace MoreCyclopsUpgrades.Patchers
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Common;
    using HarmonyLib;
    using Managers;
    using UnityEngine.UI;

    [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update))]
    internal class CyclopsHelmHUDManager_Update_Patcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            QuickLogger.Debug("Transpiling CyclopsHelmHUDManager.Update");

            // Prevent the powerText from being updated as part of the normal Update method            

            bool passedNoiseBar = false;
            bool startPatching = false;
            bool donePatching = false;
            short injected = 0;
            FieldInfo noiseBarField = typeof(CyclopsHelmHUDManager).GetField(nameof(CyclopsHelmHUDManager.noiseBar));
            MethodInfo fillAmount = typeof(Image).GetProperty(nameof(Image.fillAmount)).GetSetMethod();
            MethodInfo quickUpdateMethod = typeof(CyclopsHelmHUDManager_Update_Patcher).GetMethod(nameof(CyclopsHelmHUDManager_Update_Patcher.QuickUpdate));

            foreach (CodeInstruction instruction in instructions)
            {
                if (!passedNoiseBar)
                {
                    passedNoiseBar = instruction.opcode == OpCodes.Ldfld && instruction.operand.Equals(noiseBarField);
                    yield return instruction;
                }
                else if (!startPatching)
                {
                    startPatching = instruction.opcode == OpCodes.Callvirt && instruction.operand.Equals(fillAmount);

                    // Return the same instructions up to this line ::: this.noiseBar.fillAmount = Mathf.Lerp(this.noiseBar.fillAmount, noisePercent, Time.deltaTime);
                    yield return instruction;
                }
                else if (!donePatching)
                {
                    // From here we replace all the code responsible for checking lastPowerPctUsedForString and updating powerText.text

                    // We will continue with replacements until we pass the last line of this code ::: this.lastPowerPctUsedForString = num; }
                    donePatching = instruction.opcode == OpCodes.Stfld; // CyclopsHelmHUDManager::lastPowerPctUsedForString

                    // Injecting a call to QuickUpdate to allow the CyclopsHUDManager to handle this
                    if (injected == 0)
                    {
                        injected++;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                    }
                    else if (injected == 1)
                    {
                        injected++;
                        yield return new CodeInstruction(OpCodes.Call, quickUpdateMethod);
                    }
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Nop);
                    }
                }
                else
                {
                    // Allow the original code to continue after the injection and removal of the powerText updates
                    yield return instruction;
                }
            }
        }

        public static void QuickUpdate(CyclopsHelmHUDManager hudManager)
        {
            CyclopsManager.GetManager(ref hudManager.subRoot)?.HUD?.FastUpdate(hudManager);
        }
    }

    [HarmonyPatch(typeof(CyclopsHolographicHUD), nameof(CyclopsHolographicHUD.RefreshUpgradeConsoleIcons))]
    internal class CyclopsHolographicHUD_RefreshUpgradeConsoleIcons_Patcher
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> inputs)
        {
            // Replaces the entire method with just a return statement, effectively making the method do nothing.
            // Replacing this with a custom handler is necessary as RefreshUpgradeConsoleIcons wouldn't work with the AuxUpgradeConsole.

            yield return new CodeInstruction(OpCodes.Ret); // Now handled by the UpgradeManager
        }
    }

    [HarmonyPatch(typeof(CyclopsUpgradeConsoleHUDManager), nameof(CyclopsUpgradeConsoleHUDManager.RefreshScreen))]
    internal class CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher
    {
        // Energy display now handled by CyclopsHUDManager
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            QuickLogger.Debug("Transpiling CyclopsUpgradeConsoleHUDManager.RefreshScreen");

            MethodInfo updatePowerMethod = typeof(CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher).GetMethod(nameof(CyclopsUpgradeConsoleHUDManager_RefreshScreen_Patcher.UpdatePowerDisplays));

            // Skip over the updates to energyCur.text to be handled by the CyclopsHUDManager
            foreach (CodeInstruction instruction in instructions)
            {
                // Return the original code until we start loading the energyCur field ::: this.energyCur.text = IntStringCache.GetStringForInt((int)this.subRoot.powerRelay.GetPower());
                if (instruction.opcode == OpCodes.Ldfld &&
                    instruction.operand is FieldInfo fieldInfo &&
                    fieldInfo.Name == nameof(CyclopsUpgradeConsoleHUDManager.energyCur))
                {
                    yield return new CodeInstruction(OpCodes.Call, updatePowerMethod);
                    yield return new CodeInstruction(OpCodes.Ret);
                    break; // Since the code we are replacing is at the end of the method, we'll just stop here with an early ret.
                }

                yield return instruction;
            }
        }

        public static void UpdatePowerDisplays(CyclopsUpgradeConsoleHUDManager consoleHUDManager)
        {
            PdaOverlayManager.UpdateIconOverlays();

            CyclopsManager.GetManager(ref consoleHUDManager.subRoot)?.HUD?.SlowUpdate(consoleHUDManager);
        }
    }
}
