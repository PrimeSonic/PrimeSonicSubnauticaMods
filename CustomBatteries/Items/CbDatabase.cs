using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomBatteries.API;
using UnityEngine;

namespace CustomBatteries.Items
{
    internal static class CbDatabase
    {
        public const string BatteryCraftTab = "BatteryTab";
        public const string PowCellCraftTab = "PowCellTab";
        public const string ElecCraftTab = "Electronics";
        public const string ResCraftTab = "Resources";

        public static readonly string[] BatteryCraftPath = new[] { ResCraftTab, BatteryCraftTab };
        public static readonly string[] PowCellCraftPath = new[] { ResCraftTab, PowCellCraftTab };

        public static string ExecutingFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static List<CbCore> BatteryItems { get; } = new List<CbCore>();
        public static List<CbCore> PowerCellItems { get; } = new List<CbCore>();

        public static Dictionary<TechType, CBModelData> BatteryModels { get; } = new Dictionary<TechType, CBModelData>();
        public static Dictionary<TechType, CBModelData> PowerCellModels { get; } = new Dictionary<TechType, CBModelData>();

        public static HashSet<TechType> TrackItems { get; } = new HashSet<TechType>();

        private static bool? _placeBatteriesFeatureEnabled = null;

        public static bool PlaceBatteriesFeatureEnabled
        {
            get
            {
                if (_placeBatteriesFeatureEnabled == null || !_placeBatteriesFeatureEnabled.HasValue)
                {
                    var decorationsMod = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Where((x) => x.Metadata.Name == "DecorationsMod" && x.Instance.enabled).FirstOrFallback(null);
                    Assembly decorationsModAssembly = null;
                    if (decorationsMod != null)
                    {
                        decorationsModAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(x=> x.Location == decorationsMod.Location).FirstOrFallback(decorationsModAssembly);
                    }
                    if (decorationsModAssembly != null)
                    {
                        Type decorationsModConfig = decorationsModAssembly.GetType("DecorationsMod.ConfigSwitcher", false);
                        if (decorationsModConfig != null)
                        {
                            FieldInfo enablePlaceBatteriesField = decorationsModConfig.GetField("EnablePlaceBatteries", BindingFlags.Public | BindingFlags.Static);
                            if (enablePlaceBatteriesField != null)
                                _placeBatteriesFeatureEnabled = (bool)enablePlaceBatteriesField.GetValue(null);
                        }
                    }
                }
                return _placeBatteriesFeatureEnabled != null && _placeBatteriesFeatureEnabled.Value;
            }
        }
    }
}
