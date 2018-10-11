namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using UnityEngine;

    internal class VehicleUpgrader
    {
        internal static TechType SeamothHullModule4 { get; private set; } = TechType.UnusedOld;
        internal static TechType SeamothHullModule5 { get; private set; } = TechType.UnusedOld;
        internal static bool HasModdedDepthModules { get; private set; } = false;

        internal static readonly HashSet<TechType> CommonUpgradeModules = new HashSet<TechType>()
        {
            TechType.ExoHullModule2,
            TechType.ExoHullModule1,
            TechType.VehicleHullModule3,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule1,
            TechType.VehiclePowerUpgradeModule,
            TechType.VehicleArmorPlating,
            // SpeedBooster added during patching
            // Mk4 and Mk5 Seamoth depth modules optionally added durting patching
        };

        internal static void SetSpeedBooster(SpeedBooster speedModule)
        {
            if (!speedModule.IsPatched)
            {
                QuickLogger.Debug($"SpeedBooster was not patched", true);
                return;
            }

            CommonUpgradeModules.Add(speedModule.TechType);
        }

        internal static void SetModdedDepthModules(TechType seamothDepth4, TechType seamothDepth5)
        {
            CommonUpgradeModules.Add(SeamothHullModule4 = seamothDepth4);
            CommonUpgradeModules.Add(SeamothHullModule5 = seamothDepth5);
            HasModdedDepthModules = true;
        }

        private static List<VehicleUpgrader> Upgraders = new List<VehicleUpgrader>();

        private static VehicleUpgrader CreateNewUpgrader<T>(T vehicle) where T : Vehicle
        {
            var upgrader = new VehicleUpgrader
            {
                InstanceID = vehicle.GetInstanceID()
            };

            if (upgrader.Initialize(vehicle))
            {
                Upgraders.Add(upgrader);
                QuickLogger.Debug("CreateNewUpgrader: Initialize successful");
                return upgrader;
            }
            else
            {
                QuickLogger.Debug("CreateNewUpgrader: Initialize failed");
                return null;
            }
        }

        public static VehicleUpgrader GetUpgrader<T>(T vehicle) where T : Vehicle
        {
            VehicleUpgrader vehicleUpgrader = Upgraders.Find(v => v.InstanceID == vehicle.GetInstanceID());

            if (vehicleUpgrader == null)
            {
                QuickLogger.Debug("GetUpgrader: Creating new upgrader");
                vehicleUpgrader = CreateNewUpgrader(vehicle);
            }
            else
            {
                QuickLogger.Debug("GetUpgrader: Upgrader okay to send back");
            }

            return vehicleUpgrader;
        }

        // Original values from the Vehicle class
        private float BaseForwardForce = -1f;
        private float BaseOnGroundForceMultiplier = -1f;

        public int InstanceID { get; private set; }

        private Vehicle ParentVehicle { get; set; } = null;
        private Equipment UpgradeModules { get; set; } = null;

        private DealDamageOnImpact _dmgOnImpact;
        private DealDamageOnImpact DmgOnImpact => _dmgOnImpact ?? (_dmgOnImpact = this.ParentVehicle.GetComponent<DealDamageOnImpact>());

        private LiveMixin _lifeMix;
        private LiveMixin LifeMix => _lifeMix ?? (_lifeMix = this.ParentVehicle.GetComponent<LiveMixin>());

        private int DepthIndex { get; set; } = -1;
        private bool IsSeamoth { get; set; } = false;
        private bool IsExosuit { get; set; } = false;
        private float GeneralDamageReduction { get; set; } = 1f;

        private float MaxHitPoints()
        {
            // Original BaseHP values from LiveMixin in asset.resources

            float bonusHpRatio = 0f;
            float baseHp = 100f;

            if (this.IsExosuit)
            {
                bonusHpRatio = 0.25f;
                baseHp = 600f;
            }

            if (this.IsSeamoth)
            {
                bonusHpRatio = HasModdedDepthModules ? 0.4f : 0.5f;
                baseHp = 200f;
            }

            return baseHp * (1f + this.DepthIndex * bonusHpRatio);
        }

        private float SpeedMultiplierBonus(float speedBoosterCount)
        {
            float speedRatio = 0f;
            float extraSpeedRatio = 0f;

            if (this.IsExosuit)
            {
                extraSpeedRatio = 0.25f;
                speedRatio = 0.45f;
            }

            if (this.IsSeamoth)
            {
                extraSpeedRatio = 0.20f;
                speedRatio = 0.35f;
            }

            return 1f + speedBoosterCount * speedRatio + this.DepthIndex * extraSpeedRatio;
        }

        private float EfficiencyPentalty(float speedBoosterCount)
        {
            float penaltyRatio = 1f;

            if (this.IsExosuit)
                penaltyRatio = 1.15f;

            if (this.IsSeamoth)
                penaltyRatio = 1.10f;

            return 1f + (penaltyRatio * speedBoosterCount);
        }

        private float EfficiencyBonus(float powerModuleCount)
        {
            float extraBonusRatio = 0f;

            if (this.IsExosuit)
                extraBonusRatio = 0.15f;

            if (this.IsSeamoth)
                extraBonusRatio = 0.10f;

            return 1f + powerModuleCount + this.DepthIndex * extraBonusRatio;
        }

        private float GeneralArmorFraction(float armorModuleCount)
        {
            float damageReduction = 1f;

            float reduction = 0.10f;
            while (armorModuleCount-- > 0f)
            {
                damageReduction -= reduction;
                reduction = Mathf.Max(0.01f, reduction - 0.02f);
            }

            return damageReduction - (0.05f * this.DepthIndex);
        }

        private float ImpactArmorFraction(float armorModuleCount) => Mathf.Pow(0.5f, armorModuleCount + (this.DepthIndex * 0.75f));

        internal bool Initialize<T>(T vehicle) where T : Vehicle
        {
            if (vehicle == null)
            {
                QuickLogger.Debug("Initialize Vehicle: vehicle null");
                return false;
            }

            this.InstanceID = vehicle.GetInstanceID();
            this.ParentVehicle = vehicle;

            this.IsSeamoth = vehicle is SeaMoth;
            this.IsExosuit = vehicle is Exosuit;

            BaseForwardForce = vehicle.forwardForce;
            BaseOnGroundForceMultiplier = vehicle.onGroundForceMultiplier;

            if (vehicle.modules == null)
            {
                QuickLogger.Debug("Initialize Vehicle: modules null");
                return false;
            }

            this.UpgradeModules = vehicle.modules;

            if (this.DmgOnImpact == null)
            {
                QuickLogger.Debug("Initialize Vehicle: DealDamageOnImpact null");
                return false;
            }

            if (this.LifeMix == null)
            {
                QuickLogger.Debug("Initialize Vehicle: LiveMixin null");
                return false;
            }

            return true;
        }

        private int CalculateDepthModuleIndex()
        {
            if (this.IsExosuit)
            {
                if (this.UpgradeModules.GetCount(TechType.ExoHullModule2) > 0)
                    return 2;

                if (this.UpgradeModules.GetCount(TechType.ExoHullModule1) > 0)
                    return 1;
            }
            else if (this.IsSeamoth)
            {
                if (HasModdedDepthModules)
                {
                    if (this.UpgradeModules.GetCount(SeamothHullModule5) > 0)
                        return 5;

                    if (this.UpgradeModules.GetCount(SeamothHullModule4) > 0)
                        return 4;
                }

                if (this.UpgradeModules.GetCount(TechType.VehicleHullModule3) > 0)
                    return 3;

                if (this.UpgradeModules.GetCount(TechType.VehicleHullModule2) > 0)
                    return 2;

                if (this.UpgradeModules.GetCount(TechType.VehicleHullModule1) > 0)
                    return 1;
            }

            return 0;
        }

        internal void UpgradeVehicle(TechType upgradeModule)
        {
            if (!CommonUpgradeModules.Contains(upgradeModule))
            {
                // Not an upgrade module we handle here
                return;
            }

            int nextDepthIndex = CalculateDepthModuleIndex();

            bool updateHp = this.DepthIndex != nextDepthIndex;
            bool updateArmor = updateHp || upgradeModule == TechType.VehicleArmorPlating;
            bool updateSpeed = updateHp || upgradeModule == SpeedBooster.SpeedBoosterTechType;
            bool updateEfficiency = updateHp || updateSpeed || upgradeModule == TechType.VehiclePowerUpgradeModule;

            this.DepthIndex = nextDepthIndex;

            if (updateHp) // Hit Points
            {
                UpdateHitPoints();
            }

            if (updateArmor) // Armor
            {
                int armorModuleCount = this.UpgradeModules.GetCount(TechType.VehicleArmorPlating);

                UpdateArmorRating(armorModuleCount);
            }

            if (updateEfficiency) // Efficiency
            {
                int speedBoosterCount = this.UpgradeModules.GetCount(SpeedBooster.SpeedBoosterTechType);
                int powerModuleCount = this.UpgradeModules.GetCount(TechType.VehiclePowerUpgradeModule);

                UpdatePowerRating(speedBoosterCount, powerModuleCount);

                if (updateSpeed) // Speed
                {
                    UpdateSpeedRating(speedBoosterCount);
                }
            }
        }

        private void UpdateHitPoints()
        {
            float originalMaxHp = this.LifeMix.data.maxHealth;
            float originalHp = this.LifeMix.health;

            float nextMaxHp = MaxHitPoints();

            float nextHp;

            if (originalMaxHp <= originalHp)
            {
                nextHp = nextMaxHp;
            }
            else
            {
                float ratio = originalHp / originalMaxHp;
                nextHp = Mathf.Min(ratio * nextMaxHp, nextMaxHp);
            }

            this.LifeMix.data.maxHealth = nextMaxHp;
            this.LifeMix.health = nextHp;

            ErrorMessage.AddMessage($"Vehicle durability is now {nextMaxHp}");
        }

        private void UpdateSpeedRating(float speedBoosterCount)
        {
            float speedMultiplier = SpeedMultiplierBonus(speedBoosterCount);

            this.ParentVehicle.forwardForce = speedMultiplier * BaseForwardForce;
            this.ParentVehicle.onGroundForceMultiplier = speedMultiplier * BaseOnGroundForceMultiplier;

            ErrorMessage.AddMessage($"Now running at {speedMultiplier * 100f:00}% speed");
        }

        private void UpdatePowerRating(int speedBoosterCount, int powerModuleCount)
        {
            float powerRating = EfficiencyBonus(powerModuleCount) / EfficiencyPentalty(speedBoosterCount);

            this.ParentVehicle.SetPrivateField("enginePowerRating", powerRating);

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private void UpdateArmorRating(int armorModuleCount)
        {
            this.GeneralDamageReduction = GeneralArmorFraction(armorModuleCount);

            this.DmgOnImpact.mirroredSelfDamageFraction = ImpactArmorFraction(armorModuleCount);

            ErrorMessage.AddMessage($"Armor rating is now {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f + (1f - this.GeneralDamageReduction) * 100f:00}");
        }

        internal float ReduceIncomingDamage(float damage) => damage * this.GeneralDamageReduction;
    }
}
