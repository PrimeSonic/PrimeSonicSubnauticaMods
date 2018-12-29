namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal abstract class DeepLithiumBase : Craftable
    {
        public static TechType BatteryID { get; protected set; }
        public static TechType PowerCellID { get; protected set; }

        protected DeepLithiumBase(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Electronics;
        public override string AssetsFolder { get; } = @"MidGameBatteries/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "Resources", "Electronics" };
        public override TechType RequiredForUnlock { get; } = TechType.WhiteMushroom;

        protected GameObject CreateBattery(TechType prefabType, float capacity)
        {
            GameObject prefab = CraftData.GetPrefabForTechType(prefabType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = capacity;
            battery.name = $"{this.ClassID}Battery";

            return obj;
        }
    }
}
