namespace MidGameBatteries
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class DeepLithiumBattery : Craftable
    {
        private const string ClassIDString = "DeepLithiumBattery";
        protected const float BatteryCapacity = 250f;

        public DeepLithiumBattery()
            : base(classId: ClassIDString,
                   friendlyName: "Deep Lithium Battery",
                   description: "A stronger battery created from rare materials.")
        {
        }

        protected DeepLithiumBattery(string classId, string friendlyName, string description) 
            : base(classId, friendlyName, description)
        {
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Electronics;
        public override string AssetsFolder { get; } = @"MidGameBatteries/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "Resources", "Electronics" };
        public override TechType RequiredForUnlock { get; } = TechType.WhiteMushroom;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.LithiumIonBattery);
            var obj = GameObject.Instantiate(prefab);

            var battery = obj.GetComponent<Battery>();
            battery._capacity = BatteryCapacity;
            battery.name = ClassIDString;

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(3)
                {
                    new Ingredient(TechType.WhiteMushroom, 3),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Magnetite, 1),
                }
            };
        }
    }
}
