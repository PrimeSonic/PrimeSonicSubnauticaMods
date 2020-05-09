namespace CyclopsBioReactor.SaveData
{
    using System.Collections.Generic;
    using System.IO;
    using EasyMarkup;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class CyBioReactorSaveData : EmPropertyCollection
    {
        private const string ReactorBatterChargeKey = "BRP";
        private const string MaterialsKey = "MAT";
        private const string BoostCountKey = "BC";
        private readonly string ID;

        private readonly EmProperty<float> _batteryCharge;
        private readonly EmProperty<int> _boosterCount;
        private readonly EmPropertyCollectionList<EmModuleSaveData> _materials;

        private static ICollection<EmProperty> GetDefinitions => new List<EmProperty>()
        {
            new EmProperty<float>(ReactorBatterChargeKey, 0),
            new EmProperty<int>(BoostCountKey, 0),
            new EmPropertyCollectionList<EmModuleSaveData>(MaterialsKey)
        };

        public CyBioReactorSaveData(ICollection<EmProperty> definitions) : base("CyBioReactor", definitions)
        {
            _batteryCharge = (EmProperty<float>)Properties[ReactorBatterChargeKey];
            _boosterCount = (EmProperty<int>)Properties[BoostCountKey];
            _materials = (EmPropertyCollectionList<EmModuleSaveData>)Properties[MaterialsKey];
        }

        public CyBioReactorSaveData(string preFabID) : this(GetDefinitions)
        {
            ID = preFabID;
        }

        public void SaveMaterialsProcessing(IList<BioEnergy> materialsInProcessor)
        {
            _materials.Values.Clear();

            for (int m = 0; m < materialsInProcessor.Count; m++)
            {
                BioEnergy item = materialsInProcessor[m];
                _materials.Add(new EmModuleSaveData
                {
                    ItemID = (int)item.Pickupable.GetTechType(),
                    RemainingCharge = item.RemainingEnergy
                });
            }
        }

        public List<BioEnergy> GetMaterialsInProcessing()
        {
            var list = new List<BioEnergy>();

            for (int m = 0; m < _materials.Values.Count; m++)
            {
                EmModuleSaveData savedItem = _materials.Values[m];

                if (savedItem.ItemID <= 0)
                    continue;

                GameObject prefab = CraftData.GetPrefabForTechType((TechType)savedItem.ItemID);

                if (prefab == null)
                    continue;

                var gameObject = GameObject.Instantiate(prefab);

                if (gameObject == null)
                    continue;

                Pickupable pickupable = gameObject.GetComponent<Pickupable>();

                if (pickupable == null)
                    continue;

                pickupable.Pickup(false);

                list.Add(new BioEnergy(pickupable, savedItem.RemainingCharge));
            }

            return list;
        }

        public float ReactorBatterCharge
        {
            get => _batteryCharge.HasValue ? _batteryCharge.Value : 0;
            set => _batteryCharge.Value = value;
        }

        public int BoosterCount
        {
            get => _boosterCount.HasValue ? _boosterCount.Value : 0;
            set => _boosterCount.Value = Mathf.Max(value, 0);
        }

        private string SaveDirectory => Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "CyBioReactor");
        private string SaveFile => Path.Combine(this.SaveDirectory, ID + ".txt");

        public void Save()
        {
            this.Save(this.SaveDirectory, this.SaveFile);
        }

        public bool Load()
        {
            return this.Load(this.SaveDirectory, this.SaveFile);
        }

        internal override EmProperty Copy()
        {
            return new CyBioReactorSaveData(this.CopyDefinitions);
        }
    }
}
