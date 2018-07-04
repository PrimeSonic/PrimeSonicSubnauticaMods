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
        private const float SpeedBonusPerModule = 0.40f;

        private const int BonusModuleCount = 2; // Extra bonus to common module upgrades

        internal static Vector3[] UpgradedVehicleColors
        {
            get
            {
                return new Vector3[]
                            {
                                new Vector3(0f, 0f, 0f),
                                new Vector3(1f, 1f, 1f),
                                new Vector3(0.5f, 0.5f, 0.5f),
                                new Vector3(0.5f, 0.4f, 0.6f),
                                new Vector3(0.1f, 0.7f, 0.9f)
                            };
            }
        }

        #region Crush Depth Upgrades

        internal static void UpgradeSeaMoth(SeaMoth seamoth, TechType techType)
        {
            if (techType != TechType.VehicleHullModule1 &&
                techType != TechType.VehicleHullModule2 &&
                techType != TechType.VehicleHullModule3 &&
                techType != SeaMothMk3.SeamothHullModule4 &&
                techType != SeaMothMk3.SeamothHullModule5)
                return; // Not a depth module. No need to update anything here.

            UpgradeSeaMoth(seamoth, true);
        }

        internal static void UpgradeSeaMoth(SeaMoth seamoth, bool announce = false)
        {
            TechType seamothType = seamoth.GetComponent<TechTag>().type;

            if (seamothType != SeaMothMk2.TechTypeID && seamothType != SeaMothMk3.TechTypeID)
                return; // This is a normal Seamoth. Do not upgrade.

            float minimumCrush = 0f;

            if (seamothType == SeaMothMk2.TechTypeID)
                minimumCrush = 700f;
            else if (seamothType == SeaMothMk3.TechTypeID)
                minimumCrush = 1500f;

            OverrideCrushDepth(seamoth.crushDamage, minimumCrush, announce);
        }

        internal static void UpgradeExosuit(Exosuit exosuit, TechType techType)
        {
            if (techType != TechType.ExoHullModule1 &&
                techType != TechType.ExoHullModule2)
                return; // Not a depth module. No need to update anything here.

            UpgradeExosuit(exosuit, true);
        }

        internal static void UpgradeExosuit(Exosuit exosuit, bool announce = false)
        {
            bool isUpgradedExosuit = exosuit.GetComponent<TechTag>().type == ExosuitMk2.TechTypeID;

            if (!isUpgradedExosuit)
                return; // This is a normal Prawn Suit. Do not upgrade.

            OverrideCrushDepth(exosuit.crushDamage, 800f, announce);
        }

        private static void OverrideCrushDepth(CrushDamage crushDamage, float minimumBonus, bool announce)
        {
            // Can set a minimum crush depth without upgrades
            float bonusCrushDepth = Mathf.Max(minimumBonus, crushDamage.extraCrushDepth);
                        
            float origCrush = crushDamage.crushDepth;
            
            crushDamage.SetExtraCrushDepth(bonusCrushDepth);

            float nextCrush = crushDamage.crushDepth;

            if (announce && !Mathf.Approximately(origCrush, nextCrush))
            {
                ErrorMessage.AddMessage(Language.main.GetFormat("CrushDepthNow", crushDamage.crushDepth));
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
                UpdateArmorRating(vehicle, armorModuleCount, true);
            }
            else if (techType == SpeedBooster.TechTypeID || techType == TechType.VehiclePowerUpgradeModule) // Set power efficiency rating
            {
                int speedBoosterCount = GetModuleCount(vehicle.modules, SpeedBooster.TechTypeID, isUpgradedVehicle);
                int powerModuleCount = GetModuleCount(vehicle.modules, TechType.VehiclePowerUpgradeModule, isUpgradedVehicle);

                UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount, true);

                if (techType == SpeedBooster.TechTypeID) // Set speed rating
                {
                    UpdateSpeedRating(vehicle, speedBoosterCount, true);
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
            UpdateArmorRating(vehicle, armorModuleCount, false);

            // Set power efficiency rating
            UpdatePowerRating(vehicle, speedBoosterCount, powerModuleCount, false);

            // Set speed rating
            UpdateSpeedRating(vehicle, speedBoosterCount, false);
        }

        internal static bool IsUpgradedVehicle(Vehicle vehicle)
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

        private static void UpdateSpeedRating(Vehicle vehicle, int speedBoosterCount, bool announement)
        {
            float speedMultiplier = 1f + speedBoosterCount * SpeedBonusPerModule;

            vehicle.forwardForce = speedMultiplier * ForwardForce;
            //vehicle.backwardForce = speedMultiplier * BackwardForce;
            //vehicle.sidewardForce = speedMultiplier * SidewardForce;

            //vehicle.sidewaysTorque = speedMultiplier * SidewaysTorque;
            //vehicle.verticalForce = speedMultiplier * VerticalForce;
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
            float efficiencyPenalty = Mathf.Max(1f, 1f * speedBoosterCount);
            float powerRating = efficiencyBonus / efficiencyPenalty;
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
            bool isUpgradedVehicle = IsUpgradedVehicle(vehicle);
            int armorModuleCount = GetModuleCount(vehicle.modules, TechType.VehicleArmorPlating, isUpgradedVehicle);

            return damage * (1f - 0.20f * armorModuleCount);
        }

        #endregion
    }
}
