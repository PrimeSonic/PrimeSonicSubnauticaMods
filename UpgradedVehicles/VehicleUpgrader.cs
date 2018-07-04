namespace UpgradedVehicles
{
    using UnityEngine;
    using Common;

    internal class VehicleUpgrader
    {
        // Original values from the Vehicle class
        private const float ForwardForce = 13f;
        private const float BackwardForce = 5f;
        private const float SidewardForce = 11.5f;
        private const float SidewaysTorque = 8.5f;
        private const float VerticalForce = 11f;
        private const float OnGroundForceMultiplier = 1f;

        private const float BonusSpeed = 0.40f; //40% bonus
        internal const string BonusSpeedText = "40";

        internal static void UpgradeSeaMoth(SeaMoth seamoth, TechType techType)
        {
            bool isUpgradedSeamoth = seamoth.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (!isUpgradedSeamoth)
                return; // This is a normal Seamoth. Do not upgrade.

            if (techType != TechType.VehicleHullModule1 &&
                techType != TechType.VehicleHullModule2 &&
                techType != TechType.VehicleHullModule3)
                return; // Not a depth module. No need to update anything here.

            UpgradeCrushDepth(seamoth, 700f);
        }

        internal static void UpgradeSeaMoth(SeaMoth seamoth)
        {
            bool isUpgradedSeamoth = seamoth.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (!isUpgradedSeamoth)
                return; // This is a normal Seamoth. Do not upgrade.

            UpgradeCrushDepth(seamoth, 700f);
        }

        internal static void UpgradeExosuit(Exosuit exosuit, TechType techType)
        {
            bool isUpgradedExosuit = exosuit.GetComponent<TechTag>().type == ExosuitMk2.TechTypeID;

            if (!isUpgradedExosuit)
                return; // This is a normal Prawn Suit. Do not upgrade.

            if (techType != TechType.ExoHullModule1 &&
                techType != TechType.ExoHullModule2)
                return; // Not a depth module. No need to update anything here.

            UpgradeCrushDepth(exosuit, 800f);
        }

        internal static void UpgradeExosuit(Exosuit exosuit)
        {
            bool isUpgradedExosuit = exosuit.GetComponent<TechTag>().type == ExosuitMk2.TechTypeID;

            if (!isUpgradedExosuit)
                return; // This is a normal Prawn Suit. Do not upgrade.

            UpgradeCrushDepth(exosuit, 800f);
        }

        private static void UpgradeCrushDepth(Vehicle vehicle, float minimumBonus = 0f)
        {
            // Minimum crush depth of 900 without upgrades            
            float bonusCrushDepth = Mathf.Max(minimumBonus, vehicle.crushDamage.extraCrushDepth);
            vehicle.crushDamage.SetExtraCrushDepth(bonusCrushDepth);
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
            float speedMultiplier = 1f + speedBoosterCount * BonusSpeed;

            vehicle.forwardForce = speedMultiplier * ForwardForce;
            vehicle.backwardForce = speedMultiplier * BackwardForce;
            //vehicle.sidewardForce = speedMultiplier * SidewardForce;

            //vehicle.sidewaysTorque = speedMultiplier * SidewaysTorque;
            vehicle.verticalForce = speedMultiplier * VerticalForce;
            vehicle.onGroundForceMultiplier = speedMultiplier * OnGroundForceMultiplier;

            ErrorMessage.AddMessage($"Speed rating is now at {speedMultiplier * 100:00}%");
        }

        private static void UpdatePowerRating(Vehicle vehicle, int speedBoosterCount, int powerModuleCount)
        {
            float efficiencyBonus = Mathf.Max(1f, 1f * powerModuleCount);
            float efficiencyPenalty = Mathf.Max(1f, 1f * speedBoosterCount);
            float powerRating = efficiencyBonus / efficiencyPenalty;
            vehicle.SetPrivateField("enginePowerRating", powerRating);

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private static void UpdateArmorRating(Vehicle vehicle, int armorModuleCount)
        {
            var component = vehicle.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, armorModuleCount);

            ErrorMessage.AddMessage($"Armor rating is now at {1f / component.mirroredSelfDamageFraction}");
        }
    }
}
