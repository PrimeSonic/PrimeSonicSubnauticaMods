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

        private const int BonusModuleCount = 2; // Extra bonus to common module upgrades

        #region Crush Depth Upgrades

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

        #endregion

        #region Common Upgrades

        internal static void UpgradeVehicle(Vehicle vehicle, TechType techType)
        {
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);

            if (techType == TechType.VehicleArmorPlating) // Set armor rating
            {
                int armorModuleCount = GetModuleCount(vehicle.modules, TechType.VehicleArmorPlating, isUpgradedVehicle);
                UpdateArmorRating(vehicle, armorModuleCount);
            }
            else if (techType == SpeedBooster.TechTypeID || techType == TechType.VehiclePowerUpgradeModule) // Set power efficiency rating
            {
                int speedBoosterCount = GetModuleCount(vehicle.modules, SpeedBooster.TechTypeID, isUpgradedVehicle);
                int powerModuleCount = GetModuleCount(vehicle.modules, TechType.VehiclePowerUpgradeModule, isUpgradedVehicle);

                UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount);

                if (techType == SpeedBooster.TechTypeID) // Set speed rating
                {
                    UpdateSpeedRating(vehicle, speedBoosterCount);
                }
            }
        }

        internal static void UpgradeVehicle(Vehicle vehicle)
        {
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);
            
            int armorModuleCount = GetModuleCount(vehicle.modules, TechType.VehicleArmorPlating, isUpgradedVehicle);
            int speedBoosterCount = GetModuleCount(vehicle.modules, SpeedBooster.TechTypeID, isUpgradedVehicle);
            int powerModuleCount = GetModuleCount(vehicle.modules, TechType.VehiclePowerUpgradeModule, isUpgradedVehicle);

            // Set armor rating
            UpdateArmorRating(vehicle, armorModuleCount);

            // Set power efficiency rating
            UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount);

            // Set speed rating
            UpdateSpeedRating(vehicle, speedBoosterCount);
        }
        
        private static bool IsUpgradedVehicle(Vehicle vehicle)
        {
            TechType vehicleTechType = vehicle.GetComponent<TechTag>().type;
            bool isUpgradedVehicle = vehicleTechType == SeaMothMk2.TechTypeID ||
                                     vehicleTechType == ExosuitMk2.TechTypeID ||
                                     vehicleTechType == SeaMothMk3.TechTypeID; // This one will be a nonsense TechType if it wasn't added
            return isUpgradedVehicle;
        }

        private static int GetModuleCount(Equipment modules, TechType moduleType, bool isUpgradedVehicle)
        {
            return modules.GetCount(moduleType) + (isUpgradedVehicle ? BonusModuleCount : 0);
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
            
            ErrorMessage.AddMessage($"Armor rating is now {1f / component.mirroredSelfDamageFraction}");
        }

        internal static float ReduceIncomingDamage(Vehicle vehicle, float damage)
        {
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);
            int armorModuleCount = GetModuleCount(vehicle.modules, TechType.VehicleArmorPlating, isUpgradedVehicle);

            return damage * (1f - 0.15f * armorModuleCount);
        }

        #endregion
    }
}
