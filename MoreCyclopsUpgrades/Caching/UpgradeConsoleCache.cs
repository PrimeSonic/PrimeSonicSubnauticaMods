namespace MoreCyclopsUpgrades.Caching
{
    using System;
    using System.Collections.Generic;
    using Monobehaviors;
    using UnityEngine;

    internal class UpgradeConsoleCache
    {
        private static readonly List<Battery> SolarBatteries = new List<Battery>();
        private static readonly List<Battery> ThermalBatteries = new List<Battery>();
        private static readonly List<NuclearModuleDetails> NuclearReactorModules = new List<NuclearModuleDetails>();

        private static List<AuxUpgradeConsole> TempCache = new List<AuxUpgradeConsole>();

        internal static float BonusCrushDepth { get; private set; } = 0f;

        internal static bool HasChargingModules { get; private set; } = false;
        internal static bool HasSolarModules { get; private set; } = false;
        internal static bool HasThermalModules { get; private set; } = false;
        internal static bool HasNuclearModules { get; private set; } = false;

        internal static int PowerIndex { get; private set; } = 0;

        internal static int SpeedIndex { get; private set; } = 0;

        internal static int SolarModuleCount { get; private set; } = 0;
        internal static int ThermalModuleCount { get; private set; } = 0;

        internal static IEnumerable<Battery> SolarMk2Batteries => SolarBatteries;
        internal static IEnumerable<Battery> ThermalMk2Batteries => ThermalBatteries;
        internal static IEnumerable<NuclearModuleDetails> NuclearModules => NuclearReactorModules;

        internal static IEnumerable<Battery> ReserveBatteries
        {
            get
            {
                foreach (Battery battery in SolarMk2Batteries)
                    yield return battery;

                foreach (Battery battery in ThermalMk2Batteries)
                    yield return battery;

                foreach (NuclearModuleDetails module in NuclearModules)
                    yield return module.NuclearBattery;
            }
        }

        internal static IEnumerable<Equipment> UpgradeConsoles
        {
            get
            {
                if (Cyclops.upgradeConsole != null)
                    yield return Cyclops.upgradeConsole.modules;

                foreach (AuxUpgradeConsole aux in AuxUpgradeConsoles)
                    yield return aux.Modules;
            }
        }

        internal static bool IsSynced(SubRoot cyclops) => ReferenceEquals(cyclops, Cyclops);

        private static List<AuxUpgradeConsole> AuxUpgradeConsoles = new List<AuxUpgradeConsole>();
        private static SubRoot Cyclops = null;

        internal static void SyncUpgradeConsoles(SubRoot cyclops)
        {
            if (cyclops == null)
                return;

            TempCache.Clear();

            AuxUpgradeConsole[] auxUpgradeConsoles = cyclops.GetAllComponentsInChildren<AuxUpgradeConsole>();

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
            {
                if (TempCache.Contains(auxConsole))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    auxConsole.ParentCyclops = cyclops;
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");
                }
            }

            if (!ReferenceEquals(Cyclops, cyclops) || TempCache.Count != AuxUpgradeConsoles.Count)
            {
                Cyclops = cyclops;
                AuxUpgradeConsoles.Clear();
                AuxUpgradeConsoles.AddRange(TempCache);
            }
        }

        internal static void ClearModuleCache()
        {
            BonusCrushDepth = 0f;

            PowerIndex = 0;
            SpeedIndex = 0;

            SolarModuleCount = 0;
            ThermalModuleCount = 0;

            SolarBatteries.Clear();
            ThermalBatteries.Clear();
            NuclearReactorModules.Clear();

            HasChargingModules = false;
            HasSolarModules = false;
            HasThermalModules = false;
            HasNuclearModules = false;
        }

        internal static void AddSpeedModule() => SpeedIndex++;

        internal static void AddPowerMk1Module() => PowerIndex = Math.Max(PowerIndex, 1);

        internal static void AddPowerMk2Module() => PowerIndex = Math.Max(PowerIndex, 2);

        internal static void AddPowerMk3Module() => PowerIndex = Math.Max(PowerIndex, 3);

        internal static void AddSolarModule()
        {
            SolarModuleCount++;
            HasChargingModules = true;
            HasSolarModules = true;
        }

        internal static void AddThermalModule()
        {
            ThermalModuleCount++;
            HasChargingModules = true;
            HasThermalModules = true;
        }

        internal static void AddSolarMk2Module(Battery battery)
        {
            SolarBatteries.Add(battery);
            HasChargingModules = true;
            HasSolarModules = true;
        }

        internal static void AddThermalMk2Module(Battery battery)
        {
            ThermalBatteries.Add(battery);
            HasChargingModules = true;
            HasThermalModules = true;
        }

        internal static void AddNuclearModule(Equipment parentModule, string slotName, Battery nuclearBattery)
        {
            NuclearReactorModules.Add(new NuclearModuleDetails(parentModule, slotName, nuclearBattery));
            HasChargingModules = true;
            HasNuclearModules = true;
        }

        internal static void AddDepthModule(TechType depthModule) => BonusCrushDepth = Mathf.Max(BonusCrushDepth, ExtraCrushDepths[depthModule]);

        // This is a straight copy of the values in the original
        private static readonly Dictionary<TechType, float> ExtraCrushDepths = new Dictionary<TechType, float>
        {
            { TechType.HullReinforcementModule, 800f },
            { TechType.HullReinforcementModule2, 1600f },
            { TechType.HullReinforcementModule3, 2800f },
            { TechType.CyclopsHullModule1, 400f },
            { TechType.CyclopsHullModule2, 800f },
            { TechType.CyclopsHullModule3, 1200f }
        };
    }
}
