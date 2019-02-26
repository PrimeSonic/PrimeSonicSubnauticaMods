namespace MoreCyclopsUpgrades.SaveData
{
    using System.Collections.Generic;
    using System.IO;
    using Common.EasyMarkup;
    using MoreCyclopsUpgrades.Monobehaviors;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class CyBioReactorSaveData : EmPropertyCollection
    {
        private const string KeyName = "CBR";
        private const string ReactorBatterChargeKey = "BRP";
        private const string MaterialsKey = "MAT";
        private readonly string ID;

        private readonly EmProperty<float> _batteryCharge;
        private EmPropertyCollectionList<EmModuleSaveData> _materials;

        private static ICollection<EmProperty> GetDefinitions => new List<EmProperty>()
        {
            new EmProperty<float>(ReactorBatterChargeKey, 0),
            new EmPropertyCollectionList<EmModuleSaveData>(MaterialsKey)
        };

        public CyBioReactorSaveData(ICollection<EmProperty> definitions) : base("CyBioReactor", definitions)
        {
            _batteryCharge = (EmProperty<float>)Properties[ReactorBatterChargeKey];
            _materials = (EmPropertyCollectionList<EmModuleSaveData>)Properties[MaterialsKey];
        }

        public CyBioReactorSaveData(string preFabID) : this(GetDefinitions)
        {
            ID = preFabID;
        }

        public void SaveMaterialsProcessing(IEnumerable<BioEnergy> materialsInProcessor)
        {
            _materials.Values.Clear();

            foreach (BioEnergy item in materialsInProcessor)
            {
                _materials.Add(new EmModuleSaveData
                {
                    ItemID = (int)item.Item.GetTechType(),
                    RemainingCharge = item.Energy
                });
            }
        }

        public List<BioEnergy> GetMaterialsInProcessing()
        {
            var list = new List<BioEnergy>();

            foreach (EmModuleSaveData savedItem in _materials.Values)
            {
                var techTypeID = (TechType)savedItem.ItemID;
                var gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));

                Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);

                list.Add(new BioEnergy(pickupable, savedItem.RemainingCharge));
            }

            return list;
        }

        public float ReactorBatterCharge
        {
            get => Mathf.Min(_batteryCharge.Value, CyBioReactorMono.MaxPower);
            set => _batteryCharge.Value = Mathf.Min(value, CyBioReactorMono.MaxPower);
        }

        private string SaveDirectory => Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "CyBioReactor");
        private string SaveFile => Path.Combine(this.SaveDirectory, ID + ".txt");

        public void Save() => this.Save(this.SaveDirectory, this.SaveDirectory);

        public bool Load() => this.Load(this.SaveDirectory, this.SaveDirectory);

        internal override EmProperty Copy() => new CyBioReactorSaveData(this.CopyDefinitions);
    }
}
