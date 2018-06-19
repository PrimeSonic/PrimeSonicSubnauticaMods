namespace UpgradedVehicles
{
    using UnityEngine;
    using Common;
    using System.Reflection;
    using System;

    internal class VehicleUpgrader
    {
        internal const float SeaMothNormalHP = 200f;
        internal const float SeaMothMk2HP = 400f;



        internal static void UpgradeSeaMoth(SeaMoth seamoth)
        {
            if (seamoth.GetComponent<PrefabIdentifier>().ClassId != SeaMothMk2.NameID)
            {
                Console.WriteLine($"[UpgradedVehicles] UpgradeSeaMoth : Skipped");

                return; // This is a normal Seamoth. Do not upgrade.
            }

            Console.WriteLine($"[UpgradedVehicles] UpgradeSeaMoth : Start");

            // Minimum crush depth of 900 without upgrades
            float extraCrush = seamoth.crushDamage.extraCrushDepth;

            if (extraCrush < 700f)
                extraCrush = 700f;

            seamoth.crushDamage.SetExtraCrushDepth(extraCrush);

            var deluxeStorage = seamoth.GetComponent<SeaMothStorageDeluxe>();
            Console.WriteLine($"[UpgradedVehicles] DeluxeStorage : {deluxeStorage}");

            // All four storage modules always on
            for (int i = 0; i < 4; i++)
            {
                SeamothStorageInput seamothStorageInput = seamoth.storageInputs[i];
                seamothStorageInput.SetEnabled(true);

                if (deluxeStorage.Storages[i] == null)
                    deluxeStorage.Storages[i] = new SeamothStorageContainer();

                deluxeStorage.Storages[i].enabled = true;
            }

            Console.WriteLine($"[UpgradedVehicles] UpgradeSeaMoth : Finish");
        }

        internal static void UpgradeVehicle(Vehicle vehicle)
        {
            var nameID = vehicle.GetComponent<PrefabIdentifier>().ClassId;

            if (nameID != SeaMothMk2.NameID) //  ExoSuitMk2
            {
                Console.WriteLine($"[UpgradedVehicles] UpgradeVehicle : Skipped");

                return; // This is a normal Seamoth. Do not upgrade.
            }

            // Minimum of +2 to engine eficiency
            int powerModuleCount = vehicle.modules.GetCount(TechType.VehiclePowerUpgradeModule);
            powerModuleCount += 2;
            DealDamageOnImpact component = vehicle.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, powerModuleCount);

            // Minium of +2 to armor plating
            int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);
            armorModuleCount += 2;
            float powerRating = 1f + 1f * armorModuleCount;
            vehicle.SetPrivateField("enginePowerRating", powerRating);
        }
    }
}
