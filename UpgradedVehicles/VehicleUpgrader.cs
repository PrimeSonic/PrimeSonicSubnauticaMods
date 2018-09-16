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

        // Original value from LiveMixin
        private float BaseMaxHP = -1f;

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

        private float SpeedBonus(int speedBoosterCount)
        {
            if (this.IsExosuit)
                return speedBoosterCount * 0.40f;

            if (this.IsSeamoth)
                return speedBoosterCount * 0.30f;

            return 0.35f * speedBoosterCount;
        }

        private float ExtraSpeedBonus()
        {
            if (this.IsExosuit)
                return this.DepthIndex * 0.25f;

            if (this.IsSeamoth)
                return this.DepthIndex * 0.20f;

            return 0f;
        }

        private float EfficiencyPentalty(int speedBoosterCount)
        {
            if (speedBoosterCount == 0)
                return 1f;

            if (this.IsExosuit)
                return 1.15f * speedBoosterCount;

            if (this.IsSeamoth)
                return 1.10f * speedBoosterCount;

            return 1f * speedBoosterCount;
        }

        private float EfficiencyBonus(int powerModuleCount)
        {
            if (powerModuleCount == 0)
                return 1f;

            return 1f + powerModuleCount;
        }

        private float ExtraEfficiencyBonus()
        {
            if (this.IsExosuit)
                return this.DepthIndex * 0.15f;

            if (this.IsSeamoth)
                return this.DepthIndex * 0.10f;

            return 0f;
        }

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
            else if (BaseMaxHP == -1f)
            {
                BaseMaxHP = this.LifeMix.maxHealth;
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

            if (updateHp) // Hit Points
            {
                this.DepthIndex = nextDepthIndex;

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
            if (BaseMaxHP == -1f)
            {
                QuickLogger.Debug($"VehicleUpgrader BaseMaxHP was not correctly initialized", true);
                return;
            }

            float originalMaxHp = this.LifeMix.data.maxHealth;
            float originalHp = this.LifeMix.health;

            float nextMaxHp = BaseMaxHP * (1f + this.DepthIndex * 0.5f);
            float nextHp = originalHp / originalMaxHp * nextMaxHp;

            this.LifeMix.data.maxHealth = nextMaxHp;
            this.LifeMix.health = nextHp;

            ErrorMessage.AddMessage($"Vehicle durability is now {nextMaxHp}");
        }

        private void UpdateSpeedRating(int speedBoosterCount)
        {
            float speedMultiplier = 1f + SpeedBonus(speedBoosterCount) + ExtraSpeedBonus();

            this.ParentVehicle.forwardForce = speedMultiplier * BaseForwardForce;
            this.ParentVehicle.onGroundForceMultiplier = speedMultiplier * BaseOnGroundForceMultiplier;

            ErrorMessage.AddMessage($"Now running at {speedMultiplier * 100f:00}% speed");
        }

        private void UpdatePowerRating(int speedBoosterCount, int powerModuleCount)
        {
            float efficiencyBonus = EfficiencyBonus(powerModuleCount) + ExtraEfficiencyBonus();

            float efficiencyPentalty = EfficiencyPentalty(speedBoosterCount);

            float powerRating = efficiencyBonus / efficiencyPentalty;

            this.ParentVehicle.SetPrivateField("enginePowerRating", powerRating);

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private void UpdateArmorRating(int armorModuleCount)
        {
            this.GeneralDamageReduction = 1f - (0.05f * this.DepthIndex);

            this.DmgOnImpact.mirroredSelfDamageFraction = Mathf.Pow(0.5f, armorModuleCount + (this.DepthIndex * 0.75f));

            float reduction = 0.10f;
            while (armorModuleCount-- > 0)
            {
                this.GeneralDamageReduction -= Mathf.Max(0.01f, reduction);
                reduction -= 0.02f;
            }

            ErrorMessage.AddMessage($"Armor rating is now {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f + (1f - this.GeneralDamageReduction) * 100f:00}");

            //ErrorMessage.AddMessage($"Impact damage reduced by {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f:00}%");
            //ErrorMessage.AddMessage($"General damage reduced by {(1f - this.GeneralDamageReduction) * 100f:00}%");
        }

        internal float ReduceIncomingDamage(float damage) => damage * this.GeneralDamageReduction;
    }
}
