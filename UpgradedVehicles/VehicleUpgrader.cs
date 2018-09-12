namespace UpgradedVehicles
{
    using UnityEngine;
    using Common;

    internal class VehicleUpgrader
    {
        // Original values from the Vehicle class
        private const float ForwardForce = 13f;
        private const float OnGroundForceMultiplier = 1f;

        // Original value from VehicleMotor class
        private const float KSpeed = 100f;
        private const float SpeedBonusPerModule = 0.75f;

        internal static void UpgradeVehicle(Vehicle vehicle, TechType techType)
        {
            if (techType == TechType.VehicleArmorPlating) // Set armor rating
            {
                int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);
                UpdateArmorRating(vehicle, armorModuleCount, true);
            }
            else if (techType == SpeedBooster.SpeedBoosterTechType|| techType == TechType.VehiclePowerUpgradeModule) // Set power efficiency rating
            {
                int speedBoosterCount = vehicle.modules.GetCount(SpeedBooster.SpeedBoosterTechType);
                int powerModuleCount = vehicle.modules.GetCount(TechType.VehiclePowerUpgradeModule);

                UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount, true);

                if (techType == SpeedBooster.SpeedBoosterTechType) // Set speed rating
                {
                    UpdateSpeedRating(vehicle, speedBoosterCount, true);
                }
            }
        }

        internal static void UpgradeVehicle(Vehicle vehicle)
        {
            int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);
            int speedBoosterCount = vehicle.modules.GetCount(SpeedBooster.SpeedBoosterTechType);
            int powerModuleCount = vehicle.modules.GetCount(TechType.VehiclePowerUpgradeModule);

            // Set armor rating
            UpdateArmorRating(vehicle, armorModuleCount, false);

            // Set power efficiency rating
            UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount, false);

            // Set speed rating
            UpdateSpeedRating(vehicle, speedBoosterCount, false);
        }

        private static void UpdateSpeedRating(Vehicle vehicle, int speedBoosterCount, bool announement)
        {
            float speedMultiplier = 1f + speedBoosterCount * SpeedBonusPerModule;

            vehicle.forwardForce = speedMultiplier * ForwardForce;
            vehicle.onGroundForceMultiplier = speedMultiplier * OnGroundForceMultiplier;

            var motor = vehicle.GetComponent<VehicleMotor>();
            if (motor != null)
            {
                motor.kSpeedScalar = speedMultiplier * KSpeed;
                motor.kMaxSpeed = speedMultiplier * KSpeed;
            }

            if (announement)
                ErrorMessage.AddMessage($"Speed rating is now at {speedMultiplier * 100:00}%");
        }

        private static void UpdatePowerRating(Vehicle vehicle, int speedBoosterCount, int powerModuleCount, bool announement)
        {
            float efficiencyBonus = Mathf.Max(1f, 1f * powerModuleCount);

            float powerRating;

            if (speedBoosterCount == 0)
                powerRating = efficiencyBonus;
            else
            {
                float efficiencyPenalty = 1f * speedBoosterCount;
                powerRating = efficiencyBonus / efficiencyPenalty;
            }

            vehicle.SetPrivateField("enginePowerRating", powerRating);

            if (announement)
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private static void UpdateArmorRating(Vehicle vehicle, int armorModuleCount, bool announement)
        {
            var component = vehicle.GetComponent<DealDamageOnImpact>();
            component.mirroredSelfDamageFraction = 0.5f * Mathf.Pow(0.5f, armorModuleCount);

            if (announement)
                ErrorMessage.AddMessage($"Armor rating is now {1f / component.mirroredSelfDamageFraction}");
        }

        internal static float ReduceIncomingDamage(Vehicle vehicle, float damage)
        {
            int armorModuleCount = vehicle.modules.GetCount(TechType.VehicleArmorPlating);

            return damage * (1f - 0.15f * armorModuleCount);
        }
    }
}
