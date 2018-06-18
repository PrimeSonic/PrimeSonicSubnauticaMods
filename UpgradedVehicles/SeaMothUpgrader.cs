namespace UpgradedVehicles
{
    using UnityEngine;
    using Common;
    using System.Reflection;
    using System;

    internal class SeaMothUpgrader
    {
        internal static void UpgradeSeaMoth(SeaMoth seamoth)
        {
            Console.WriteLine($"[UpgradedVehicles] UpgradeSeaMoth : Start");

            // Minimum crush depth of 900 without upgrades
            float extraCrush = seamoth.crushDamage.extraCrushDepth;
            seamoth.crushDamage.SetExtraCrushDepth(Mathf.Min(700f, extraCrush));

            // All four storage modules always on
            for (int i = 0; i < 4; i++)
            {
                SeamothStorageInput seamothStorageInput = seamoth.storageInputs[i];
                seamothStorageInput.SetEnabled(true);
            }

            // Minimum of +2 to engine eficiency
            int powerModuleCount = seamoth.modules.GetCount(TechType.VehiclePowerUpgradeModule);
            powerModuleCount += 2;
            DealDamageOnImpact component = seamoth.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, (float)powerModuleCount);


            int armorModuleCount = seamoth.modules.GetCount(TechType.VehicleArmorPlating);
            armorModuleCount += 2;
            float powerRating = 1f + 1f * armorModuleCount;
            seamoth.SetPrivateField("enginePowerRating", powerRating, BindingFlags.FlattenHierarchy);

            Console.WriteLine($"[UpgradedVehicles] UpgradeSeaMoth : Finish");
        }
    }
}
