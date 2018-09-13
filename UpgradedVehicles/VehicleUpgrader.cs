namespace UpgradedVehicles
{
    using Common;
    using UnityEngine;

    internal class VehicleUpgrader : MonoBehaviour
    {
        internal static TechType SeamothHullModule4 { get; private set; } = TechType.UnusedOld;
        internal static TechType SeamothHullModule5 { get; private set; } = TechType.UnusedOld;
        internal static bool HasModdedDepthModules { get; private set; } = false;

        // Original values from the Vehicle class
        private float ForwardForce = 13f;
        private float OnGroundForceMultiplier = 1f;

        // Original value from VehicleMotor class
        private float KSpeedScalar = 100f;

        private Vehicle ParentVehicle { get; set; } = null;
        private Equipment UpgradeModules { get; set; } = null;
        private DealDamageOnImpact DmgOnImpact { get; set; } = null;
        private VehicleMotor Motor { get; set; } = null;
        private LiveMixin LifeMix { get; set; } = null;

        private int DepthIndex { get; set; } = -1;
        private float OriginalMaxHP { get; set; } = -1f;
        private bool IsSeamoth { get; set; } = false;
        private bool IsExosuit { get; set; } = false;

        private float ExtraSpeedBonus()
        {
            if (this.IsExosuit)
                return this.DepthIndex * 0.25f;

            if (this.IsSeamoth)
                return this.DepthIndex * 0.20f;

            return 0f;
        }

        private float ExtraEfficiencyBonus()
        {
            if (this.IsExosuit)
                return this.DepthIndex * 0.15f;

            if (this.IsSeamoth)
                return this.DepthIndex * 0.10f;

            return 0f;
        }

        private float GeneralDamageReduction { get; set; } = 1f;

        internal void Initialize<T>(T vehicle) where T : Vehicle
        {
            if (this.ParentVehicle != null)
                return; // Already initialized

            this.ParentVehicle = vehicle;
            this.UpgradeModules = vehicle.modules;

            this.DmgOnImpact = vehicle.GetComponent<DealDamageOnImpact>();
            this.Motor = vehicle.GetComponent<VehicleMotor>();
            this.LifeMix = vehicle.GetComponent<LiveMixin>();

            this.OriginalMaxHP = vehicle.GetComponent<LiveMixin>().maxHealth;

            this.IsSeamoth = vehicle is SeaMoth;
            this.IsExosuit = vehicle is Exosuit;

            ForwardForce = vehicle.forwardForce;
            OnGroundForceMultiplier = vehicle.onGroundForceMultiplier;

            KSpeedScalar = this.Motor.kSpeedScalar;
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

        internal void UpgradeVehicle(TechType techType)
        {
            int nextDepthIndex = CalculateDepthModuleIndex();

            if (this.DepthIndex != nextDepthIndex) // Bonus HP from Depth modules
            {
                this.DepthIndex = nextDepthIndex;

                UpdateHitPoints();
            }
            else if (techType == TechType.VehicleArmorPlating) // Armor
            {
                int armorModuleCount = this.UpgradeModules.GetCount(TechType.VehicleArmorPlating);

                UpdateArmorRating(armorModuleCount);
            }
            else if (techType == SpeedBooster.SpeedBoosterTechType || // Speed
                techType == TechType.VehiclePowerUpgradeModule) // Efficiency
            {
                int speedBoosterCount = this.UpgradeModules.GetCount(SpeedBooster.SpeedBoosterTechType);
                int powerModuleCount = this.UpgradeModules.GetCount(TechType.VehiclePowerUpgradeModule);

                UpdatePowerRating(speedBoosterCount, powerModuleCount);

                if (techType == SpeedBooster.SpeedBoosterTechType) // Speed
                {
                    UpdateSpeedRating(speedBoosterCount);
                }
            }
        }

        private void UpdateHitPoints()
        {
            float originalMaxHp = this.LifeMix.data.maxHealth;
            float originalHp = this.LifeMix.health;

            float bonusHealth = this.DepthIndex * 0.5f;

            float nextMaxHp = this.OriginalMaxHP + bonusHealth;
            float nextHp = originalHp / originalMaxHp * nextMaxHp;

            this.LifeMix.data.maxHealth = nextMaxHp;
            this.LifeMix.health = nextHp;

            ErrorMessage.AddMessage($"Vehicle durability is now {nextMaxHp}");
        }

        private void UpdateSpeedRating(int speedBoosterCount)
        {
            float speedMultiplier = 1f + 0.35f * speedBoosterCount + ExtraSpeedBonus();

            this.ParentVehicle.forwardForce = speedMultiplier * ForwardForce;
            this.ParentVehicle.onGroundForceMultiplier = speedMultiplier * OnGroundForceMultiplier;

            if (this.Motor != null)
            {
                this.Motor.kSpeedScalar = speedMultiplier * KSpeedScalar;
                this.Motor.kMaxSpeed = speedMultiplier * KSpeedScalar;
            }

            ErrorMessage.AddMessage($"Now running at {speedMultiplier * 100f:00}% speed");
        }

        private void UpdatePowerRating(int speedBoosterCount, int powerModuleCount)
        {
            float efficiencyBonus = (powerModuleCount + 1f) + ExtraEfficiencyBonus();

            float powerRating;

            if (speedBoosterCount == 0)
                powerRating = efficiencyBonus;
            else
            {
                float efficiencyPenalty = 1.05f;

                while (--speedBoosterCount > 0)
                {
                    efficiencyPenalty += 1f + 0.15f * speedBoosterCount;
                }

                powerRating = efficiencyBonus / efficiencyPenalty;
            }

            this.ParentVehicle.SetPrivateField("enginePowerRating", powerRating);

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
        }

        private void UpdateArmorRating(int armorModuleCount)
        {
            this.GeneralDamageReduction = 1f - (0.05f * this.DepthIndex);

            this.DmgOnImpact.mirroredSelfDamageFraction = Mathf.Pow(0.5f, ++armorModuleCount);

            float reduction = 0.10f;
            while (--armorModuleCount > 0)
            {
                this.GeneralDamageReduction -= Mathf.Max(0.01f, reduction);
                reduction -= 0.02f;
            }

            ErrorMessage.AddMessage($"Impact damage reduced by {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f:00}%");
            ErrorMessage.AddMessage($"General damage reduced by {(1f - this.GeneralDamageReduction) * 100f:00}%");
        }

        internal float ReduceIncomingDamage(float damage) => damage * this.GeneralDamageReduction;

        internal static void SetModdedDepthModules(TechType seamothDepth4, TechType seamothDepth5)
        {
            SeamothHullModule4 = seamothDepth4;
            SeamothHullModule5 = seamothDepth5;
            HasModdedDepthModules = true;
        }
    }
}
