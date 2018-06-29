namespace UpgradedVehicles
{
    using UnityEngine;
    using Common;
    using UpgradedVehicles.Modules;
    using System.Collections.Generic;

    internal class VehicleUpgrader
    {
        private static readonly IList<TechType> SeamothDepthModules = new List<TechType>(5)
        {
            TechType.VehicleHullModule1,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule3
        };

        private const float ForwardForces = 13f;
        private const float BackwardForces = 5f;
        private const float SidewardForces = 11.5f;
        private const float SidewaysTorque = 8.5f;
        private const float VerticalForce = 11f;
        private const float OnGroundForceMultiplier = 1f;

        private const float BonusSpeed = 1.25f;

        internal static void UpgradeSeaMoth(SeaMoth seamoth, TechType techType)
        {
            bool isUpgradedSeamoth = seamoth.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (!isUpgradedSeamoth)
                return; // This is a normal Seamoth. Do not upgrade.

            if (!SeamothDepthModules.Contains(techType))
                return; // Not a depth module. No need to update anything here.

            UpgradeSeamothCrushDepth(seamoth, 700f);
        }

        internal static void UpgradeSeaMoth(SeaMoth seamoth)
        {
            bool isUpgradedSeamoth = seamoth.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (!isUpgradedSeamoth)
                return; // This is a normal Seamoth. Do not upgrade.

            UpgradeSeamothCrushDepth(seamoth, 700f);
        }

        private static void UpgradeSeamothCrushDepth(SeaMoth seamoth, float minimumBonus = 0f)
        {
            // Minimum crush depth of 900 without upgrades            
            float bonusCrushDepth = Mathf.Max(minimumBonus, seamoth.crushDamage.extraCrushDepth);
            seamoth.crushDamage.SetExtraCrushDepth(bonusCrushDepth);
        }

        internal static void UpgradeVehicle(Vehicle vehicle, TechType techType)
        {
            TechType vehicleTechType = vehicle.GetComponent<TechTag>().type;
            bool isUpgradedVehicle = vehicleTechType == SeaMothMk2.TechTypeID || vehicleTechType == ExosuitMk2.TechTypeID;

            if (techType == TechType.VehicleArmorPlating) // Set armor rating
            {
                int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);

                if (isUpgradedVehicle)
                    armorModuleCount += 2; // Minium of +2 to armor plating on upgraded vehicles

                UpdateArmorRating(vehicle, armorModuleCount);
            }
            else if (techType == SpeedBooster.TechTypeID || techType == TechType.VehiclePowerUpgradeModule) // Set power efficiency rating
            {
                int speedBoosterCount = vehicle.modules.GetCount(SpeedBooster.TechTypeID);
                int powerModuleCount = vehicle.modules.GetCount(TechType.VehiclePowerUpgradeModule);

                if (isUpgradedVehicle)
                {
                    speedBoosterCount += 2; // Minimum of +2 to speed boost on upgraded vehicles
                    powerModuleCount += 2; // Minimum of +2 to engine eficiency on upgraded vehicles
                }

                UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount);

                if (techType == SpeedBooster.TechTypeID) // Set speed rating
                {
                    UpdateSpeedRating(vehicle, speedBoosterCount);
                }
            }
        }

        internal static void UpgradeVehicle(Vehicle vehicle)
        {
            TechType vehicleTechType = vehicle.GetComponent<TechTag>().type;

            bool isUpgradedVehicle = vehicleTechType == SeaMothMk2.TechTypeID || vehicleTechType == ExosuitMk2.TechTypeID;

            // Set armor rating

            int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);

            if (isUpgradedVehicle)
                armorModuleCount += 2; // Minium of +2 to armor plating on upgraded vehicles

            UpdateArmorRating(vehicle, armorModuleCount);

            // Set power efficiency rating

            int speedBoosterCount = vehicle.modules.GetCount(SpeedBooster.TechTypeID);
            int powerModuleCount = vehicle.modules.GetCount(TechType.VehiclePowerUpgradeModule);

            if (isUpgradedVehicle)
            {
                speedBoosterCount += 2; // Minimum of +2 to speed boost on upgraded vehicles
                powerModuleCount += 2; // Minimum of +2 to engine eficiency on upgraded vehicles
            }

            UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount);

            // Set speed rating

            UpdateSpeedRating(vehicle, speedBoosterCount);
        }

        private static void UpdateSpeedRating(Vehicle vehicle, int speedBoosterCount)
        {
            float speedMultiplier = Mathf.Max(1f, speedBoosterCount * BonusSpeed);

            vehicle.forwardForce = speedMultiplier * ForwardForces;
            vehicle.backwardForce = speedMultiplier * BackwardForces;
            vehicle.sidewardForce = speedMultiplier * SidewardForces;

            vehicle.sidewaysTorque = speedMultiplier * SidewaysTorque;
            vehicle.verticalForce = speedMultiplier * VerticalForce;
            vehicle.onGroundForceMultiplier = speedMultiplier * OnGroundForceMultiplier;

            ErrorMessage.AddMessage($"Speed rating is now at {speedMultiplier:00}%");
        }

        private static void UpdatePowerRating(Vehicle vehicle, int speedBoosterCount, int powerModuleCount)
        {
            float efficiencyBonus = Mathf.Max(1f, 1f * powerModuleCount);
            float efficiencyPenalty = Mathf.Max(1f, +1f * speedBoosterCount);
            float powerRating = efficiencyBonus / efficiencyPenalty;
            vehicle.SetPrivateField("enginePowerRating", powerRating);

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private static void UpdateArmorRating(Vehicle vehicle, int armorModuleCount)
        {
            var component = vehicle.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, armorModuleCount);

            ErrorMessage.AddMessage($"Armor rating is now at {component.mirroredSelfDamageFraction:00}");
        }
    }
}
