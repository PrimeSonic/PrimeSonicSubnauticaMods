namespace UpgradedVehicles
{
    using Harmony;
    using UnityEngine;
    using Common;
    using System.Reflection;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_Patcher
    {
        private static readonly string[] slotIDs = new string[]
        {
            "SeamothModule1",
            "SeamothModule2",
            "SeamothModule3",
            "SeamothModule4"
        };

        public static void PostFix(ref SeaMoth __instance, int slotID, TechType techType, bool added)
        {
            var tag = __instance.GetComponent<TechTag>();

            if (tag.type != SeaMothMk2.TechTypeID)
                return;

            // Minimum crush depth of 900 without upgrades
            float extraCrush = __instance.crushDamage.extraCrushDepth;
            __instance.crushDamage.SetExtraCrushDepth(Mathf.Min(700f, extraCrush));

            // Solar Charger always on
            __instance.CancelInvoke("UpdateSolarRecharge");
            __instance.InvokeRepeating("UpdateSolarRecharge", 1f, 1f);            

            // All four storage modules always on
            for (int i = 0; i < 4; i++)
            {            
                SeamothStorageInput seamothStorageInput = __instance.storageInputs[i];
                seamothStorageInput.SetEnabled(true);
            }

            // Minimum of +2 to engine eficiency
            int powerModuleCount = __instance.modules.GetCount(TechType.VehiclePowerUpgradeModule);            
            powerModuleCount += 2;
            DealDamageOnImpact component = __instance.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, (float)powerModuleCount);

            // Minimum of +2 to armor
            int armorModuleCount = __instance.modules.GetCount(TechType.VehicleArmorPlating);
            armorModuleCount += 2;
            float powerRating = 1f + 1f * armorModuleCount;
            __instance.SetPrivateField("enginePowerRating", powerRating, BindingFlags.FlattenHierarchy);

        }
    }
}
