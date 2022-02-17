namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Handlers;
    using UnityEngine;
    using UpgradedVehicles.SaveData;

    public class VehicleUpgrader : MonoBehaviour
    {
#if BELOWZERO
        private static Type SeaTruckSpeedManager => Type.GetType("SeaTruckSpeedUpgrades.SeaTruckSpeedManager,SeaTruckSpeedUpgrades", false, false);
        
        // Going for the private field rather than the public property, because the private field is static and the public property isn't
        private static FieldInfo SpeedManagerAccelerationsInfo => SeaTruckSpeedManager?.GetField("accelerations", BindingFlags.NonPublic | BindingFlags.Static);
        private static Dictionary<TechType, float> _speedManagerAccelerations;

        private static Dictionary<TechType, float> SpeedManagerAccelerations
        {
            get
            {
                if (_speedManagerAccelerations == null)
                {
                    if (SeaTruckSpeedManager != null)
                        _speedManagerAccelerations = (Dictionary<TechType, float>)(SpeedManagerAccelerationsInfo.GetValue(null));
                }

                return _speedManagerAccelerations;
            }
        }
#endif

        private static readonly Dictionary<TechType, float> VehicleEfficiencyBonuses = new Dictionary<TechType, float>()
        {
#if BELOWZERO
            { TechType.SeaTruckUpgradeEnergyEfficiency, 1f },
#endif
            { TechType.VehiclePowerUpgradeModule, 1f }
        };

        private static readonly Dictionary<TechType, float> VehicleEfficiencyPenalties = new Dictionary<TechType, float>();

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
            // HullArmorUpgrades added during patching
            // Mk4 and Mk5 Seamoth depth modules optionally added during patching
#if BELOWZERO
            TechType.SeaTruckUpgradeHull3,
            TechType.SeaTruckUpgradeHull2,
            TechType.SeaTruckUpgradeHull1,
#endif
        };

        //internal static readonly IDictionary<TechType, int> ArmorPlatingModules = new Dictionary<TechType, int>()
        private static readonly IDictionary<TechType, int> ArmorPlatingModules = new Dictionary<TechType, int>()
        {
            { TechType.VehicleArmorPlating, 1 }
            // HullArmorUpgrades added during patching
        };

#if BELOWZERO
        // These are only used for calculating impact damage, as Senna's mod already accounts for non-impact damage.
        private static readonly IDictionary<TechType, int> SeatruckArmourModules = new Dictionary<TechType, int>();
#endif

        // Added during patching
        public static readonly Dictionary<TechType, float> SpeedBoostingModules = new();

        private static readonly IDictionary<TechType, int> SeamothDepthModules = new Dictionary<TechType, int>()
        {
            { TechType.VehicleHullModule3, 3 },
            { TechType.VehicleHullModule2, 2 },
            { TechType.VehicleHullModule1, 1 },
            // Depth Modules Mk4 and Mk5 optionaly added during patching
        };

        private static readonly IDictionary<TechType, int> SeaTruckDepthModules = new Dictionary<TechType, int>()
        {
#if BELOWZERO
            { TechType.SeaTruckUpgradeHull3, 3 },
            { TechType.SeaTruckUpgradeHull2, 2 },
            { TechType.SeaTruckUpgradeHull1, 1 },
            // As with the Seamoth, Mk4, Mk5 and Mk6 are added during patching if necessary
#endif
        };

        // This isn't used for much right now, but it opens the way for expanded Exosuit hull modules
        private static readonly Dictionary<TechType, int> ExosuitDepthModules = new()
        {
            { TechType.ExoHullModule2, 2 },
            { TechType.ExoHullModule1, 1 },
        };

        private static readonly HashSet<TechType> DepthUpgradeModules = new HashSet<TechType>
        {
            TechType.ExoHullModule2,
            TechType.ExoHullModule1,
            TechType.VehicleHullModule3,
            TechType.VehicleHullModule2,
            TechType.VehicleHullModule1,
            // Depth Modules Mk4 and Mk5 optionaly added during patching
#if BELOWZERO
            TechType.SeaTruckUpgradeHull3,
            TechType.SeaTruckUpgradeHull2,
            TechType.SeaTruckUpgradeHull1,
#endif
        };

        internal static void SetBonusSpeedMultipliers(IUpgradeOptions upgradeOptions)
        {
            _seamothBonusSpeedMultiplier = upgradeOptions.SeamothBonusSpeedMultiplier;
#if BELOWZERO
            _seatruckBonusSpeedMultiplier = upgradeOptions.SeatruckBonusSpeedMultiplier;
#endif
            _exosuitBonusSpeedMultiplier = upgradeOptions.ExosuitBonusSpeedMultiplier;
        }

        // Original values from the Vehicle class
        private const float BaseSeamothSpeed = 11.5f;
#if SUBNAUTICA
        private const float BaseExosuitSwimSpeed = 6f;
#elif BELOWZERO
        private const float BaseExosuitSwimSpeed = 8.2f;
#endif
        private const float BaseExosuitWalkSpeed = 4f;
        private const float BaseSeaTruckAcceleration = 17.5f;

        private MonoBehaviour _vehicle = null;
        public MonoBehaviour ParentVehicle
        {
            get
            {
                return _vehicle;
            }
            set
            {
                if (value is Vehicle)
                    _vehicle = value;
#if BELOWZERO
                else if (value is SeaTruckMotor stm && stm.truckSegment != null && stm.truckSegment.isMainCab)
                    _vehicle = value;
#endif
            }
        }
        private Equipment _modules = null;
        public Equipment UpgradeModules
        {
            get
            {
                if (_modules == null)
                {
                    if (_vehicle is Vehicle v)
                        _modules = v.modules;
#if BELOWZERO
                    else if (_vehicle is SeaTruckMotor stm)
                        _modules = stm.upgrades?.modules;
#endif
                }

                return _modules;
            }
        }

        private DealDamageOnImpact _dmgOnImpact;
        private DealDamageOnImpact DmgOnImpact => _dmgOnImpact ?? (_dmgOnImpact = ParentVehicle.GetComponent<DealDamageOnImpact>());

        public enum EVehicleType
        {
            None,
            Seamoth,
            Exosuit,
            Seatruck
        }

        protected EVehicleType currentVehicleType;

        //private bool IsSeamoth = false;
        //private bool IsExosuit = false;
        public float GeneralDamageReduction { get; protected set; } = 1f;

        private static float _seamothBonusSpeedMultiplier = 0.15f;
#if BELOWZERO
        private static float _seatruckBonusSpeedMultiplier = 0.15f;
#endif
        private static float _exosuitBonusSpeedMultiplier = 0.20f;

        public int DepthIndex { get; private set; } = -1;
        public float SpeedMultiplier { get; private set; } = 1f;
        public float EfficiencyPenalty { get; private set; } = 1f;
        public float EfficiencyBonus { get; private set; } = 1f;
        public float GeneralArmorFraction { get; private set; } = 1f;
        public float ImpactArmorFraction { get; private set; } = 1f;
        public float PowerRating { get; private set; } = 1f;
        
        private LiveMixin _lifemix = null;
        public LiveMixin LifeMix {
            get
            {
                if (_lifemix == null)
                {
                    if (ParentVehicle is Vehicle v)
                        _lifemix = v.liveMixin;
#if BELOWZERO
                    else if (ParentVehicle is SeaTruckMotor stm)
                        _lifemix = stm.liveMixin;
#endif
                }

                return _lifemix;
            }
        }

        /// <summary>
        /// Add the specified TechType to the dictionary of efficiency-improving modules with the specified value
        /// </summary>
        public static bool AddEfficiencyBonus(TechType module, float value, bool bForce = false)
        {
            if (VehicleEfficiencyBonuses.ContainsKey(module) && !bForce)
                return false;

            VehicleEfficiencyBonuses[module] = value;
            return CommonUpgradeModules.Add(module);
        }

        /// <summary>
        /// Add the specified TechType as a speed booster, with the specified strength
        /// </summary>
        public static bool AddSpeedModifier(TechType module, float value, float efficiencyValue = 0f, bool bForce = false)
        {
            if (SpeedBoostingModules.ContainsKey(module) && !bForce)
                return false;

            SpeedBoostingModules[module] = value;
            if (efficiencyValue > 0f)
                VehicleEfficiencyPenalties[module] = efficiencyValue;
            return CommonUpgradeModules.Add(module);
        }

        /// <summary>
        /// Add the specified TechType as an armour module, with the specified strength
        /// </summary>
        public static bool AddArmourModule(TechType module, int value, bool bForce = false)
        {
            if (ArmorPlatingModules.ContainsKey(module) && !bForce)
                return false;

            ArmorPlatingModules[module] = value;
            return CommonUpgradeModules.Add(module);
        }

        /// <summary>
        /// Add the specified TechType as a depth module, adding it to the common set, the depth upgrades set, and one of the vehicle-specific set based on the value of the vehicleType
        /// </summary>
        public static bool AddDepthModule(TechType module, int value, EVehicleType vehicleType, bool bForce = false)
        {
            if (!bForce && DepthUpgradeModules.Contains(module))
                return false;

            switch (vehicleType)
            {
                case EVehicleType.Exosuit:
                    ExosuitDepthModules.Add(module, value);
                    break;

                case EVehicleType.Seamoth:
                    SeamothDepthModules.Add(module, value);
                    break;
#if BELOWZERO
                case EVehicleType.Seatruck:
                    SeaTruckDepthModules.Add(module, value);
                    break;
#endif
            }
            return (CommonUpgradeModules.Add(module) && DepthUpgradeModules.Add(module));
        }

        /// <summary>
        /// Calculates the speed multiplier based on the number of speed modules and current depth index.
        /// </summary>
        private float GetSpeedMultiplierBonus(float speedBoosterCount)
        {
            float speedBoosterRatio = 0f;
            float bonusSpeedRatio = 0f;
#if BELOWZERO
            float SennaSpeedMultiplier = 1f;
#endif

            switch (currentVehicleType)
            {
                case EVehicleType.Exosuit:
                    bonusSpeedRatio = _exosuitBonusSpeedMultiplier;
                    speedBoosterRatio = 0.45f;
                    break;

                case EVehicleType.Seamoth:
                    bonusSpeedRatio = _seamothBonusSpeedMultiplier;
                    speedBoosterRatio = 0.35f;
                    break;

#if BELOWZERO
                case EVehicleType.Seatruck:
                    bonusSpeedRatio = _seatruckBonusSpeedMultiplier;
                    speedBoosterRatio = 0.35f;
                    break;
#endif
            }

#if SUBNAUTICA
            return 1f + // Base 100%
            speedBoosterCount * speedBoosterRatio + // Bonus from Speed Boosters
                   this.DepthIndex * bonusSpeedRatio; // Bonus from Depth Modules
#elif BELOWZERO
            if (SeaTruckSpeedManager != null)
            {
                SennaSpeedMultiplier = 1f;

                foreach (KeyValuePair<TechType, float> armorModule in SpeedManagerAccelerations)
                {
                    TechType techType = armorModule.Key;
                    if (this.UpgradeModules.GetCount(techType) > 0)
                    {
                        float multiplier = armorModule.Value;
                        SennaSpeedMultiplier = Math.Max(SennaSpeedMultiplier, multiplier);
                    }
                }

                //ErrorMessage.AddMessage($"GetImpactArmorFraction: armorModuleCount = {armorModuleCount}");
            }

            return SennaSpeedMultiplier +
            speedBoosterCount * speedBoosterRatio + // Bonus from Speed Boosters
                   this.DepthIndex * bonusSpeedRatio; // Bonus from Depth Modules
#endif
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

            return this.EfficiencyBonus = 1f + powerModuleCount + this.DepthIndex * 0.15f;
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
#if BELOWZERO
            int max = 0;

            foreach (KeyValuePair<TechType, int> armorModule in SeatruckArmourModules)
            {
                TechType techType = armorModule.Key;
                if (this.UpgradeModules.GetCount(techType) > 0)
                {
                    int multiplier = armorModule.Value;
                    max = Math.Max(max, multiplier);
                }
            }

            armorModuleCount += (float)max;
#endif
            return this.ImpactArmorFraction = Mathf.Pow(0.5f, armorModuleCount + (this.DepthIndex * 0.2f));
        }

        internal void Initialize<TVehicle>(ref TVehicle vehicle) where TVehicle : MonoBehaviour
        {
            QuickLogger.Debug("Initialized vehicle");
            ParentVehicle = vehicle;

            if (vehicle is Exosuit)
                currentVehicleType = EVehicleType.Exosuit;
#if BELOWZERO
            else if (vehicle is SeaTruckMotor)
                currentVehicleType = EVehicleType.Seatruck;
#endif
            else if (vehicle is SeaMoth)
                currentVehicleType = EVehicleType.Seamoth;

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

            int max = 0;
            switch (this.currentVehicleType)
            {
                case EVehicleType.Exosuit:
                    if (this.UpgradeModules.GetCount(TechType.ExoHullModule2) > 0)
                        max = 2;
                    else if (this.UpgradeModules.GetCount(TechType.ExoHullModule1) > 0)
                        max = 1;
                    break;

                case EVehicleType.Seamoth:
                    foreach (KeyValuePair<TechType, int> seamothDepth in SeamothDepthModules)
                    {
                        TechType techType = seamothDepth.Key;
                        if (this.UpgradeModules.GetCount(techType) > 0)
                        {
                            int depthIndex = seamothDepth.Value;
                            max = Math.Max(max, depthIndex);
                        }
                    }
                    break;

#if BELOWZERO
                case EVehicleType.Seatruck:
                    foreach (KeyValuePair<TechType, int> seaTruckDepth in SeaTruckDepthModules)
                    {
                        TechType tt = seaTruckDepth.Key;
                        if (this.UpgradeModules.GetCount(tt) > 0)
                        {
                            int depthIndex = seaTruckDepth.Value;
                            max = Math.Max(max, depthIndex);
                        }
                    }
                    break;
#endif
            }

            return max;
        }

        internal void UpgradeVehicle<TVehicle>(TechType upgradeModule, ref TVehicle vehicle)
            where TVehicle : MonoBehaviour
        {
            if (vehicle is not Vehicle
#if BELOWZERO
                && vehicle is not SeaTruckMotor
#endif
                )
                return;

            if (ParentVehicle != vehicle)
                Initialize(ref vehicle);

            if (!CommonUpgradeModules.Contains(upgradeModule))
            {
                // Not an upgrade module we handle here
                return;
            }

            //TechType speedBooster;
            //TechTypeHandler.TryGetModdedTechType("SpeedModule", out speedBooster);
            bool updateAll = DepthUpgradeModules.Contains(upgradeModule);
            bool updateArmor = updateAll || ArmorPlatingModules.ContainsKey(upgradeModule);
            bool updateSpeed = updateAll || SpeedBoostingModules.ContainsKey(upgradeModule);
            bool updateEfficiency = updateAll || updateSpeed || VehicleEfficiencyBonuses.ContainsKey(upgradeModule);

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
                float speedBoosterCount = 0f;
                float powerModuleValue = 0f;
                float powerPenaltyValue = 0f;

                foreach (var kvp in VehicleEfficiencyBonuses)
                {
                    powerModuleValue += this.UpgradeModules.GetCount(kvp.Key) * kvp.Value;
                }

                foreach (var kvp in SpeedBoostingModules)
                {
                    speedBoosterCount += this.UpgradeModules.GetCount(kvp.Key) * kvp.Value;
                    powerPenaltyValue += VehicleEfficiencyPenalties.GetOrDefault(kvp.Key, 0f);
                }

                UpdatePowerRating(powerPenaltyValue, powerModuleValue);

                if (updateSpeed) // Speed
                {
                    UpdateSpeedRating(speedBoosterCount);
                }
            }
        }

        private void UpdateSpeedRating(float speedBoosterCount)
        {
            this.SpeedMultiplier = GetSpeedMultiplierBonus(speedBoosterCount);

            if (ParentVehicle is Vehicle v)
            {
                if (v is SeaMoth)
                {
                    v.forwardForce = this.SpeedMultiplier * BaseSeamothSpeed;
                    ErrorMessage.AddMessage($"Seamoth Speed: {v.forwardForce}m/s ({this.SpeedMultiplier * 100f:00}%)");
                }
                else if (v is Exosuit)
                {
                    v.forwardForce = this.SpeedMultiplier * BaseExosuitSwimSpeed;
                    ErrorMessage.AddMessage($"Prawn Suit Water Speed: {v.forwardForce}m/s ({this.SpeedMultiplier * 100f:00}%)");
                    v.onGroundForceMultiplier = this.SpeedMultiplier * BaseExosuitWalkSpeed;
                    ErrorMessage.AddMessage($"Prawn Suit Land Speed: {v.onGroundForceMultiplier}m/s ({this.SpeedMultiplier * 100f:00}%)");

                }
            }
#if BELOWZERO
            else if (ParentVehicle is SeaTruckMotor stm)
            {
                stm.acceleration = this.SpeedMultiplier * BaseSeaTruckAcceleration;
                ErrorMessage.AddMessage($"SeaTruck engine acceleration: {stm.acceleration}m/s ({this.SpeedMultiplier * 100f:00}%)");
            }
#endif
        }

        private void UpdatePowerRating(float speedBoosterCount, float powerModuleValue)
        {
            this.PowerRating = GetEfficiencyBonus(powerModuleValue) / GetEfficiencyPentalty(speedBoosterCount);

            if (ParentVehicle is Vehicle v)
            {
                v.enginePowerRating = this.PowerRating;
            }
#if BELOWZERO
            else if (ParentVehicle is SeaTruckMotor stm)
            {
                float baseEfficiency = (powerModuleValue > 0 ? Constants.kSeaTruckPowerEfficiencyFactor : 1f);

                stm.powerEfficiencyFactor = baseEfficiency * (1 / this.PowerRating);
            }
#endif

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
