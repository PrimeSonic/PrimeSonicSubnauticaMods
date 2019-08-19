namespace DataBoxScannerFix
{
    using System;
    using System.Reflection;
    using Harmony;
    using UnityEngine;

    public static class QPatch
    {

        public static void Patch()
        {
            try
            {
                var harmony = HarmonyInstance.Create("com.dataBoxscannerfix.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch(Exception e)
            {
                Console.WriteLine($"[DataBoxScannerFix]{Environment.NewLine}{e}");
            }
        }
    }

    [HarmonyPatch(typeof(ResourceTracker), "Start")]
    internal class ResourceTracker_Patcher
    {
        [HarmonyPostfix] // Fix scanner blips for a loaded save
        internal static void PostFix(ref ResourceTracker __instance)
        {
            bool isDataBox = __instance.techType == TechType.Databox || __instance.overrideTechType == TechType.Databox;

            if (!isDataBox)
                return; // Not a data box, early exit

            var blueprint = __instance.GetComponentInParent<BlueprintHandTarget>();

            if (blueprint == null)
                return; // safety check, but shouldn't happen

            if (!blueprint.used)
                return; // blueprint still unused

            __instance.Unregister();
        }
    }

    [HarmonyPatch(typeof(BlueprintHandTarget), "UnlockBlueprint")]
    internal class BlueprintHandTarget_Patcher
    {
        [HarmonyPrefix] // Fix scanner blips when opening a Databox
        internal static bool PreFix(ref BlueprintHandTarget __instance)
        {
            __instance.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);

            return true;
        }
    }
}
