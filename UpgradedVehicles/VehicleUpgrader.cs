namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using Common;
    using UnityEngine;
    using UpgradedVehicles.SaveData;

    public class VehicleUpgrader : MonoBehaviour
    {
        internal static readonly ICollection<TechType> CommonUpgradeModules = new List<TechType>()
        {
            TechType.ExoHullModule2,
            TechType.ExoHullModule1,
            TechType.VehicleHullModule3,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule1,
            TechType.VehiclePowerUpgradeModule,
            TechType.VehicleArmorPlating,
            // SpeedBooster added during patching
            // HullArmorUpgrades added during patching
            // Mk4 and Mk5 Seamoth depth modules optionally added during patching
        };

        internal static readonly IDictionary<TechType, int> ArmorPlatingModules = new Dictionary<TechType, int>()
        {
            { TechType.VehicleArmorPlating, 1 }
            // HullArmorUpgrades added during patching
        };

        // Added during patching
        internal static TechType SpeedBoostingModule;

        internal static IDictionary<TechType, int> SeamothDepthModules = new Dictionary<TechType, int>()
        {
            { TechType.VehicleHullModule3, 3 },
            { TechType.VehicleHullModule2, 2 },
            { TechType.VehicleHullModule1, 1 },
            // Depth Modules Mk4 and Mk5 optionaly added during patching
        };

        internal static ICollection<TechType> DepthUpgradeModules = new List<TechType>
        {
            TechType.ExoHullModule2,
            TechType.ExoHullModule1,
            TechType.VehicleHullModule3,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule1
            // Depth Modules Mk4 and Mk5 optionaly added during patching
        };

        internal static void SetBonusSpeedMultipliers(IUpgradeOptions upgradeOptions)
        {
            _seamothBonusSpeedMultiplier = upgradeOptions.SeamothBonusSpeedMultiplier;
            _exosuitBonusSpeedMultiplier = upgradeOptions.ExosuitBonusSpeedMultiplier;
        }

        // Original values from the Vehicle class
        private const float BaseSeamothSpeed = 11.5f;
        private const float BaseExosuitSwimSpeed = 6f;
        private const float BaseExosuitWalkSpeed = 4f;

        private Vehicle ParentVehicle = null;
        private Equipment UpgradeModules => ParentVehicle.modules;

        private DealDamageOnImpact _dmgOnImpact;
        private DealDamageOnImpact DmgOnImpact => _dmgOnImpact ?? (_dmgOnImpact = ParentVehicle.GetComponent<DealDamageOnImpact>());

        private bool IsSeamoth = false;
        private bool IsExosuit = false;
        internal float GeneralDamageReduction { get; private set; } = 1f;

        private static float _seamothBonusSpeedMultiplier = 0.15f;
        private static float _exosuitBonusSpeedMultiplier = 0.20f;

        public int DepthIndex { get; private set; } = -1;
        public float SpeedMultiplier { get; private set; } = 1f;
        public float EfficiencyPenalty { get; private set; } = 1f;
        public float EfficientyBonus { get; private set; } = 1f;
        public float GeneralArmorFraction { get; private set; } = 1f;
        public float ImpactArmorFraction { get; private set; } = 1f;
        public float PowerRating { get; private set; } = 1f;
        public LiveMixin LifeMix => ParentVehicle.liveMixin;

        /// <summary>
        /// Calculates the speed multiplier based on the number of speed modules and current depth index.
        /// </summary>
        private float GetSpeedMultiplierBonus(float speedBoosterCount)
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
                   this.DepthIndex * bonusSpeedRatio; // Bonus from Depth Modules
        }

        /// <summary>
        /// Calculates the engine efficiency penalty based off the number of speed modules and the current depth index.
        /// </summary>
        private float GetEfficiencyPentalty(float speedBoosterCount)
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

            return this.EfficiencyPenalty = 1f + (1.10f * speedBoosterCount);
        }

        /// <summary>
        /// Calculates the engine efficiency bonus based off the number of power modules and the current depth index.
        /// </summary>
        private float GetEfficiencyBonus(float powerModuleCount)
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

            return this.EfficientyBonus = 1f + powerModuleCount + this.DepthIndex * 0.15f;
        }

        /// <summary>
        /// Calculates the general damage reduction fraction based off the number of hull armor modules and the current depth index.
        /// </summary>
        private float GetGeneralArmorFraction(float armorModuleCount)
        {
            //               Depth Index
            // Armor Modules  0      1      2      3      4*     5*
            //      0          0%    5%    10%    15%    20%    25%
            //      1         10%   15%    20%    25%    30%    35%
            //      2         20%   25%    30%    35%    40%    45%
            //      3         30%   35%    40%    45%    50%    55%
            //      4         40%   45%    50%    55%    60%    65%
            //      5         50%   55%    60%    65%    70%    75%
            //      6         60%   65%    70%    75%    80%    85%
            //      7         70%   75%    80%    85%    90%    95%
            //      8         80%   85%    90%    95%    99%    99%
            //      9         90%   95%    99%    99%    99%    99%

            float reduction = 0.10f * armorModuleCount;
            float bonus = 0.05f * this.DepthIndex;

            float damageReduction = Mathf.Max(1f - reduction - bonus, 0.01f);

            return this.GeneralArmorFraction = damageReduction;
        }

        /// <summary>
        /// Calculates the impact damage reduction fraction based off the number of hull armor modules and the current depth index.
        /// </summary>
        private float GetImpactArmorFraction(float armorModuleCount)
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

            return this.ImpactArmorFraction = Mathf.Pow(0.5f, armorModuleCount + (this.DepthIndex * 0.2f));
        }

        internal void Initialize<TVehicle>(ref TVehicle vehicle) where TVehicle : Vehicle
        {
            QuickLogger.Debug("Initialized vehicle");
            ParentVehicle = vehicle;

            IsSeamoth = vehicle is SeaMoth;
            IsExosuit = vehicle is Exosuit;

            if (this.UpgradeModules == null)
            {
                QuickLogger.Warning("Initialize Vehicle - UpgradeModules missing", true);
                return;
            }

            if (this.DmgOnImpact == null)
            {
                QuickLogger.Warning("Initialize Vehicle - DealDamageOnImpact missing", true);
                return;
            }

            if (this.LifeMix == null)
            {
                QuickLogger.Warning("Initialize Vehicle - LiveMixin missing", true);
                return;
            }
        }

        private int CalculateArmorPlatingAmount()
        {
            int max = 0;

            foreach (KeyValuePair<TechType, int> armorModule in ArmorPlatingModules)
            {
                TechType techType = armorModule.Key;
                if (this.UpgradeModules.GetCount(techType) > 0)
                {
                    int multiplier = armorModule.Value;
                    max = Math.Max(max, multiplier);
                }
            }

            return max;
        }

        private int CalculateDepthModuleIndex()
        {
            if (this.UpgradeModules == null)            
                return -1; // Missing UpgradeModules, cannot calculate

            if (IsExosuit)
            {
                if (this.UpgradeModules.GetCount(TechType.ExoHullModule2) > 0)
                    return 2;

                if (this.UpgradeModules.GetCount(TechType.ExoHullModule1) > 0)
                    return 1;
            }
            else if (IsSeamoth)
            {
                int max = 0;
                foreach (KeyValuePair<TechType, int> seamothDepth in SeamothDepthModules)
                {
                    TechType techType = seamothDepth.Key;
                    if (this.UpgradeModules.GetCount(techType) > 0)
                    {
                        int depthIndex = seamothDepth.Value;
                        max = Math.Max(max, depthIndex);
                    }
                }

                return max;
            }

            return 0;
        }

        internal void UpgradeVehicle<TVehicle>(TechType upgradeModule, ref TVehicle vehicle)
            where TVehicle : Vehicle
        {
            if (ParentVehicle != vehicle)
                Initialize(ref vehicle);

            if (!CommonUpgradeModules.Contains(upgradeModule))
            {
                // Not an upgrade module we handle here
                return;
            }

            bool updateAll = DepthUpgradeModules.Contains(upgradeModule);
            bool updateArmor = updateAll || ArmorPlatingModules.ContainsKey(upgradeModule);
            bool updateSpeed = updateAll || upgradeModule == SpeedBoostingModule;
            bool updateEfficiency = updateAll || updateSpeed || upgradeModule == TechType.VehiclePowerUpgradeModule;
            QuickLogger.Debug($"updateAll:{updateAll} updateArmor:{updateArmor} updateSpeed:{updateSpeed} updateEfficiency:{updateEfficiency}");

            int newIndex = CalculateDepthModuleIndex();

            if (newIndex < 0)
            {
                QuickLogger.Error("UpgradeVehicle - UpgradeModules missing - Unable to upgrade");
                return;
            }

            this.DepthIndex = newIndex;

            if (updateArmor) // Armor
            {
                int armorModuleCount = CalculateArmorPlatingAmount();

                UpdateArmorRating(armorModuleCount);
            }

            if (updateEfficiency) // Efficiency
            {
                int speedBoosterCount = this.UpgradeModules.GetCount(SpeedBoostingModule);
                int powerModuleCount = this.UpgradeModules.GetCount(TechType.VehiclePowerUpgradeModule);

                UpdatePowerRating(speedBoosterCount, powerModuleCount);

                if (updateSpeed) // Speed
                {
                    UpdateSpeedRating(speedBoosterCount);
                }
            }
        }

        private void UpdateSpeedRating(float speedBoosterCount)
        {
            this.SpeedMultiplier = GetSpeedMultiplierBonus(speedBoosterCount);

            if (this.IsSeamoth)
            {
                ParentVehicle.forwardForce = this.SpeedMultiplier * BaseSeamothSpeed;
                ErrorMessage.AddMessage($"Seamoth Speed: {ParentVehicle.forwardForce}m/s ({this.SpeedMultiplier * 100f:00}%)");
            }
            else if (this.IsExosuit)
            {
                ParentVehicle.forwardForce = this.SpeedMultiplier * BaseExosuitSwimSpeed;
                ErrorMessage.AddMessage($"Prawn Suit Water Speed: {ParentVehicle.forwardForce}m/s ({this.SpeedMultiplier * 100f:00}%)");
                ParentVehicle.onGroundForceMultiplier = this.SpeedMultiplier * BaseExosuitWalkSpeed;
                ErrorMessage.AddMessage($"Prawn Suit Land Speed: {ParentVehicle.onGroundForceMultiplier}m/s ({this.SpeedMultiplier * 100f:00}%)");

            }
        }

        private void UpdatePowerRating(int speedBoosterCount, int powerModuleCount)
        {
            this.PowerRating = GetEfficiencyBonus(powerModuleCount) / GetEfficiencyPentalty(speedBoosterCount);

            ParentVehicle.enginePowerRating = this.PowerRating;

            ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", this.PowerRating));
        }

        private void UpdateArmorRating(int armorModuleCount)
        {
            this.GeneralDamageReduction = GetGeneralArmorFraction(armorModuleCount);

            this.DmgOnImpact.mirroredSelfDamageFraction = GetImpactArmorFraction(armorModuleCount);

            ErrorMessage.AddMessage($"Armor rating is now {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f + (1f - this.GeneralDamageReduction) * 100f:00}");
        }
    }
}
