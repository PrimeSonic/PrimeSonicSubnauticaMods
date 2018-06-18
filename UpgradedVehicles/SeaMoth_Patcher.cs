namespace UpgradedVehicles
{
    using System;
    using Harmony;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        public static void PostFix(ref SeaMoth __instance)
        {
            var tag = __instance.GetComponent<TechTag>();
            Console.WriteLine($"[UpgradedVehicles] Patch on OnUpgradeModuleChange : TechType={tag.type}");

            if (tag.type != SeaMothMk2.TechTypeID)
                return;

            SeaMothUpgrader.UpgradeSeaMoth(__instance);
        }
    }

}
