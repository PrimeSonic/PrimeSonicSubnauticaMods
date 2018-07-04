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
            if (techType != TechType.VehicleHullModule1 &&
                techType != TechType.VehicleHullModule2 &&
                techType != TechType.VehicleHullModule3 &&
                techType != SeaMothMk3.SeamothHullModule4 &&
                techType != SeaMothMk3.SeamothHullModule5)
                return; // Not a depth module. No need to update anything here.

            UpgradeSeaMoth(seamoth);
        }

        internal static void UpgradeSeaMoth(SeaMoth seamoth)
        {
            TechType seamothType = seamoth.GetComponent<TechTag>().type;

            if (seamothType != SeaMothMk2.TechTypeID && seamothType != SeaMothMk3.TechTypeID)
                return; // This is a normal Seamoth. Do not upgrade.

            float minimumCrush = 0f;

            if (seamothType == SeaMothMk2.TechTypeID)
                minimumCrush = 700f;
            else if (seamothType == SeaMothMk3.TechTypeID)
                minimumCrush = 1500f;

            OverrideCrushDepth(seamoth, minimumCrush);
        }

        internal static void UpgradeExosuit(Exosuit exosuit, TechType techType)
        {
            if (techType != TechType.ExoHullModule1 &&
                techType != TechType.ExoHullModule2)
                return; // Not a depth module. No need to update anything here.

            UpgradeExosuit(exosuit);
        }

        internal static void UpgradeExosuit(Exosuit exosuit)
        {
            bool isUpgradedExosuit = exosuit.GetComponent<TechTag>().type == ExosuitMk2.TechTypeID;

            if (!isUpgradedExosuit)
                return; // This is a normal Prawn Suit. Do not upgrade.

            OverrideCrushDepth(exosuit, 800f);
        }

        private static void OverrideCrushDepth(Vehicle vehicle, float minimumBonus)
        {
            // Can set a minimum crush depth without upgrades
            float bonusCrushDepth = Mathf.Max(minimumBonus, vehicle.crushDamage.extraCrushDepth);

            float origCrush = vehicle.crushDamage.crushDepth;

            vehicle.crushDamage.SetExtraCrushDepth(bonusCrushDepth);

            float nextCrush = vehicle.crushDamage.crushDepth;

            if (!Mathf.Approximately(origCrush, nextCrush))
            {
                ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", nextCrush));
            }
        }

        internal static void UpgradeVehicle(Vehicle vehicle, TechType techType)
        {
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);

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

        private static bool IsUpgradedVehicle(Vehicle vehicle)
        {
            TechType vehicleTechType = vehicle.GetComponent<TechTag>().type;
            bool isUpgradedVehicle = vehicleTechType == SeaMothMk2.TechTypeID ||
                                     vehicleTechType == ExosuitMk2.TechTypeID ||
                                     vehicleTechType == SeaMothMk3.TechTypeID;
            return isUpgradedVehicle;
        }

        internal static void UpgradeVehicle(Vehicle vehicle)
        {
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);

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
