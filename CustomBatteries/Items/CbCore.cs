namespace CustomBatteries.Items
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal abstract class CbCore : ModPrefab
    {
        public static string ExecutingFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static List<TechType> BatteryTechTypes { get; } = new List<TechType>();
        public static bool HasBatteries { get; protected set; } = false;
        public static TechType SampleBattery => BatteryTechTypes[0];

        public static List<TechType> PowerCellTechTypes { get; } = new List<TechType>();        
        public static bool HasPowerCells { get; protected set; } = false;
        public static TechType SamplePowerCell => PowerCellTechTypes[0];


        protected abstract TechType PrefabType { get; } // Should only ever be Battery or PowerCell        
        protected abstract EquipmentType ChargerType { get; } // Should only ever be BatteryCharger or PowerCellCharger

        public TechType RequiredForUnlock { get; } = TechType.None;

        public TechData BlueprintRecipe { get; set; }

        public float PowerCapacity { get; set; }

        public string FriendlyName { get; set; }

        public string Description { get; set; }

        public string IconFileName { get; set; }

        public string PluginPackName { get; set; }

        protected CbCore(string classId)
            : base(classId, $"{classId}PreFab", TechType.None)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}BatteryCell";

            return obj;
        }

        protected static void CreateIngredients(IEnumerable<TechType> parts, List<Ingredient> partsList)
        {
            if (parts == null)
                return;

            foreach (TechType part in parts)
            {
                Ingredient priorIngredient = partsList.Find(i => i.techType == part);

                if (priorIngredient != null)
                    priorIngredient.amount++;
                else
                    partsList.Add(new Ingredient(part, 1));
            }
        }

        protected abstract void AddToList();

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(this.ClassID, this.FriendlyName, this.Description, false);

            PrefabHandler.RegisterPrefab(this);

            CraftDataHandler.SetTechData(this.TechType, this.BlueprintRecipe);

            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);

            CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);

            SpriteHandler.RegisterSprite(this.TechType, IOUtilities.Combine(ExecutingFolder, this.PluginPackName, this.IconFileName));

            AddToList();
        }
    }
}
