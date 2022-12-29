namespace DataBoxScannerFix
{
    using System;
    using System.Reflection;
    using BepInEx;
    using HarmonyLib;
    using UnityEngine;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "DataBox Scanner Fix",
            AUTHOR = "PrimeSonic",
            GUID = "com.dataBoxscannerfix.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            try
            {
                var dbScannerFixAssembly = Assembly.GetExecutingAssembly();

                var harmony = new Harmony(GUID);
                
                harmony.PatchAll(dbScannerFixAssembly);

                Console.WriteLine($"[DataBoxScannerFix] Patching complete v{dbScannerFixAssembly.GetName().Version}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[DataBoxScannerFix] Error during patching:{Environment.NewLine}{e}");
            }
        }
    }

    [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Start))]
    internal class ResourceTracker_Patcher
    {
        [HarmonyPostfix]
        internal static void PostFix(ref ResourceTracker __instance)
        {
            bool isDataBox = __instance.overrideTechType == TechType.Databox || __instance.techType == TechType.Databox;

            if (!isDataBox)
                return; // Not a data box, early exit

            BlueprintHandTarget blueprint = __instance.GetComponentInParent<BlueprintHandTarget>();

            if (blueprint == null)
                return; // safety check, but shouldn't happen

            if (!blueprint.used)
                return; // blueprint still unused

            __instance.OnBreakResource(); // call this to invoke the "Unregister" method
        }
    }

    [HarmonyPatch(typeof(BlueprintHandTarget), nameof(BlueprintHandTarget.UnlockBlueprint))]
    internal class BlueprintHandTarget_Patcher
    {
        [HarmonyPrefix]
        internal static bool PreFix(ref BlueprintHandTarget __instance)
        {
            __instance.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);

            return true;
        }
    }
}
