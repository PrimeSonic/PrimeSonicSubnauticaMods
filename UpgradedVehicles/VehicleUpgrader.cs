namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using UnityEngine;

    internal class VehicleUpgrader
    {
        private static TechType SeamothHullModule4 = TechType.UnusedOld;
        private static TechType SeamothHullModule5 = TechType.UnusedOld;
        private static bool HasModdedDepthModules = false;

        private static readonly HashSet<TechType> CommonUpgradeModules = new HashSet<TechType>()
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

        internal static void SetSpeedBooster(Craftable speedModule)
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
            QuickLogger.Message("Additional Seamoth Depth Modules from MoreSeamothUpgrades registered");

            CommonUpgradeModules.Add(SeamothHullModule4 = seamothDepth4);
            CommonUpgradeModules.Add(SeamothHullModule5 = seamothDepth5);
            HasModdedDepthModules = true;
        }

        internal static void SetBonusSpeedMultipliers(float seamothMultiplier, float exosuitMultiplier)
        {
            _seamothBonusSpeedMultiplier = seamothMultiplier;
            _exosuitBonusSpeedMultiplier = exosuitMultiplier;
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

        private Vehicle ParentVehicle = null;
        private Equipment UpgradeModules = null;

        private DealDamageOnImpact _dmgOnImpact;
        private DealDamageOnImpact DmgOnImpact => _dmgOnImpact ?? (_dmgOnImpact = ParentVehicle.GetComponent<DealDamageOnImpact>());

        private LiveMixin _lifeMix;
        private LiveMixin LifeMix => _lifeMix ?? (_lifeMix = ParentVehicle.GetComponent<LiveMixin>());

        private int DepthIndex = -1;
        private bool IsSeamoth = false;
        private bool IsExosuit = false;
        private float GeneralDamageReduction { get; set; } = 1f;

        private static float _seamothBonusSpeedMultiplier = 0.15f;
        private static float _exosuitBonusSpeedMultiplier = 0.20f;

        /// <summary>
        /// Calculates the new maximum hit points based on the current depth index.
        /// </summary>
        /// <returns></returns>
        private float MaxHitPoints()
        {
            //             Depth Index
            //              0      1      2      3      4*     5*
            //  ExoSuit    600    750    900
            //  Seamoth++  200    280    360    440    520    600
            //  Seamoth    200    300    400    500

            // Original BaseHP values from LiveMixin in asset.resources

            float bonusHpRatio = 0f;
            float baseHp = 100f;

            if (IsExosuit)
            {
                bonusHpRatio = 0.25f;
                baseHp = 600f;
            }

            if (IsSeamoth)
            {
                bonusHpRatio = HasModdedDepthModules ? 0.4f : 0.5f;
                baseHp = 200f;
            }

            return baseHp * (1f + DepthIndex * bonusHpRatio);
        }

        /// <summary>
        /// Calculates the speed multiplier based on the number of speed modules and current depth index.
        /// </summary>
        private float SpeedMultiplierBonus(float speedBoosterCount)
        {
            float speedBoosterRatio = 0f;
            float bonusSpeedRatio = 0f;

            if (IsExosuit)
            {
                bonusSpeedRatio = _exosuitBonusSpeedMultiplier;
                speedBoosterRatio = 0.45f;
            }

            if (IsSeamoth)
            {
                bonusSpeedRatio = _seamothBonusSpeedMultiplier;
                speedBoosterRatio = 0.35f;
            }

            return 1f + // Base 100%
                   speedBoosterCount * speedBoosterRatio + // Bonus from Speed Boosters
                   DepthIndex * bonusSpeedRatio; // Bonus from Depth Modules
        }

        /// <summary>
        /// Calculates the engine efficiency penalty based off the number of speed modules and the current depth index.
        /// </summary>
        private float EfficiencyPentalty(float speedBoosterCount)
        {
            //  Speed Modules
            //    0    100%
            //    1    210%
            //    2    320%
            //    3    430%
            //    4    540%
            //    5    650%
            //    6    760%
            //    7    870%
            //    8    980%
            //    9    1090%

            return 1f + (1.10f * speedBoosterCount);
        }

        /// <summary>
        /// Calculates the engine efficiency bonus based off the number of power modules and the current depth index.
        /// </summary>
        private float EfficiencyBonus(float powerModuleCount)
        {
            //                Depth Index
            //  Power Modules  0       1        2      3       4*      5*
            //        0       100%    115%    130%    145%    160%    175%
            //        1       200%    215%    230%    245%    260%    275%
            //        2       300%    315%    330%    345%    360%    375%
            //        3       400%    415%    430%    445%    460%    475%
            //        4       500%    515%    530%    545%    560%    575%
            //        5       600%    615%    630%    645%    660%    675%
            //        6       700%    715%    730%    745%    760%    775%
            //        7       800%    815%    830%    845%    860%    875%
            //        8       900%    915%    930%    945%    960%    975%
            //        9       1000%   1015%   1030%   1045%   1060%   1075%

            return 1f + powerModuleCount + DepthIndex * 0.15f;
        }

        /// <summary>
        /// Calculates the general damage reduction fraction based off the number of hull armor modules and the current depth index.
        /// </summary>
        private float GeneralArmorFraction(float armorModuleCount)
        {
            //               Depth Index
            // Armor Modules  0      1      2      3      4*     5*
            //      0        100%   95%    90%    85%    80%    75%
            //      1        90%    85%    80%    75%    70%    65%
            //      2        82%    77%    72%    67%    62%    57%
            //      3        76%    71%    66%    61%    56%    51%
            //      4        70%    65%    60%    55%    50%    45%
            //      5        66%    61%    56%    51%    46%    41%
            //      6        64%    59%    54%    49%    44%    39%
            //      7        63%    58%    53%    48%    43%    38%
            //      8        62%    57%    52%    47%    42%    37%
            //      9        61%    56%    51%    46%    41%    36%

            float damageReduction = 1f;

            float reduction = 0.10f;
            while (armorModuleCount-- > 0f)
            {
                damageReduction -= reduction;
                reduction = Mathf.Max(0.01f, reduction - 0.02f);
            }

            return damageReduction - (0.05f * DepthIndex);
        }

        /// <summary>
        /// Calculates the impact damage reduction fraction based off the number of hull armor modules and the current depth index.
        /// </summary>
        private float ImpactArmorFraction(float armorModuleCount)
        {
            //                Depth Index
            //  Armor Modules  0         1         2         3         4*        5*
            //        0      100.00%   87.06%    75.79%    65.98%    57.43%    50.00%
            //        1      50.00%    43.53%    37.89%    32.99%    28.72%    25.00%
            //        2      25.00%    21.76%    18.95%    16.49%    14.36%    12.50%
            //        3      12.50%    10.88%    09.47%    08.25%    07.18%    06.25%
            //        4      06.25%    05.44%    04.74%    04.12%    03.59%    03.13%
            //        5      03.13%    02.72%    02.37%    02.06%    01.79%    01.56%
            //        6      01.56%    01.36%    01.18%    01.03%    00.90%    00.78%
            //        7      00.78%    00.68%    00.59%    00.52%    00.45%    00.39%
            //        8      00.39%    00.34%    00.30%    00.26%    00.22%    00.20%
            //        9      00.20%    00.17%    00.15%    00.13%    00.11%    00.10%

            return Mathf.Pow(0.5f, armorModuleCount + (DepthIndex * 0.2f));
        }

        internal bool Initialize<T>(T vehicle) where T : Vehicle
        {
            if (vehicle == null)
            {
                QuickLogger.Debug("Initialize Vehicle: vehicle null");
                return false;
            }

            this.InstanceID = vehicle.GetInstanceID();
            ParentVehicle = vehicle;

            IsSeamoth = vehicle is SeaMoth;
            IsExosuit = vehicle is Exosuit;

            BaseForwardForce = vehicle.forwardForce;
            BaseOnGroundForceMultiplier = vehicle.onGroundForceMultiplier;

            if (vehicle.modules == null)
            {
                QuickLogger.Debug("Initialize Vehicle: modules null");
                return false;
            }

            UpgradeModules = vehicle.modules;

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
            if (IsExosuit)
            {
                if (UpgradeModules.GetCount(TechType.ExoHullModule2) > 0)
                    return 2;

                if (UpgradeModules.GetCount(TechType.ExoHullModule1) > 0)
                    return 1;
            }
            else if (IsSeamoth)
            {
                if (HasModdedDepthModules)
                {
                    if (UpgradeModules.GetCount(SeamothHullModule5) > 0)
                        return 5;

                    if (UpgradeModules.GetCount(SeamothHullModule4) > 0)
                        return 4;
                }

                if (UpgradeModules.GetCount(TechType.VehicleHullModule3) > 0)
                    return 3;

                if (UpgradeModules.GetCount(TechType.VehicleHullModule2) > 0)
                    return 2;

                if (UpgradeModules.GetCount(TechType.VehicleHullModule1) > 0)
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

            bool updateHp = DepthIndex != nextDepthIndex;
            bool updateArmor = updateHp || upgradeModule == TechType.VehicleArmorPlating;
            bool updateSpeed = updateHp || upgradeModule == SpeedBooster.SpeedBoosterTechType;
            bool updateEfficiency = updateHp || updateSpeed || upgradeModule == TechType.VehiclePowerUpgradeModule;

            DepthIndex = nextDepthIndex;

            if (updateHp) // Hit Points
            {
                UpdateHitPoints();
            }

            if (updateArmor) // Armor
            {
                int armorModuleCount = UpgradeModules.GetCount(TechType.VehicleArmorPlating);

                UpdateArmorRating(armorModuleCount);
            }

            if (updateEfficiency) // Efficiency
            {
                int speedBoosterCount = UpgradeModules.GetCount(SpeedBooster.SpeedBoosterTechType);
                int powerModuleCount = UpgradeModules.GetCount(TechType.VehiclePowerUpgradeModule);

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

            ParentVehicle.forwardForce = speedMultiplier * BaseForwardForce;
            ParentVehicle.onGroundForceMultiplier = speedMultiplier * BaseOnGroundForceMultiplier;

            ErrorMessage.AddMessage($"Now running at {speedMultiplier * 100f:00}% speed");
        }

        private void UpdatePowerRating(int speedBoosterCount, int powerModuleCount)
        {
            float powerRating = EfficiencyBonus(powerModuleCount) / EfficiencyPentalty(speedBoosterCount);

            ParentVehicle.SetPrivateField("enginePowerRating", powerRating);

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
