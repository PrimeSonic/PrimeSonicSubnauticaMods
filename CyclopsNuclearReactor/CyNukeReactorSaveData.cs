namespace CyclopsNuclearReactor
{
    using Common;
    using Common.EasyMarkup;
    using SMLHelper.V2.Utility;
    using System.Collections.Generic;
    using System.IO;

    internal class CyNukeReactorSaveData : EmPropertyCollectionList<CyNukeRodSaveData>
    {
        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "CyclopsNuclearReactor");

        private string SaveFile => Path.Combine(SaveDirectory, PreFabId + ".txt");

        private readonly string PreFabId;

        public CyNukeReactorSaveData(string prefabID) : base(prefabID)
        {
            PreFabId = prefabID;
        }

        public void ClearOldData()
        {
            this.Values.Clear();
        }

        public IEnumerable<CyNukeRodSaveData> Rods => this.Values;

        public void AddRodData(TechType techType, float remainingCharge)
        {
            this.Values.Add(new CyNukeRodSaveData
            {
                TechTypeID = techType,
                RemainingCharge = remainingCharge
            });
        }

        public void AddEmptySlot()
        {
            this.Values.Add(new CyNukeRodSaveData
            {
                TechTypeID = TechType.None,
                RemainingCharge = 0f
            });
        }

        public void SaveData()
        {
            this.Save(SaveDirectory, this.SaveFile);
        }

        public bool LoadData()
        {
            return this.Load(SaveDirectory, this.SaveFile);
        }

        internal override EmProperty Copy()
        {
            var cyNukeReactorSaveData = new CyNukeReactorSaveData(PreFabId);

            foreach (CyNukeRodSaveData item in this.Values)
                cyNukeReactorSaveData.Values.Add(item.Copy() as CyNukeRodSaveData);

            return cyNukeReactorSaveData;
        }
    }
}
