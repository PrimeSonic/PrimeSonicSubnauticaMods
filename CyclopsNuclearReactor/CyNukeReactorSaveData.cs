namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using System.IO;
    using EasyMarkup;
    using SMLHelper.V2.Utility;

    internal class CyNukeReactorSaveData : EmPropertyCollectionList<CyNukeRodSaveData>
    {
        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "CyclopsNuclearReactor");

        private string SaveFile => Path.Combine(SaveDirectory, PreFabId + ".txt");

        private readonly string PreFabId;
        private readonly int MaxSlots;

        public CyNukeReactorSaveData(string prefabID, int maxSlots) : base("CNR")
        {
            PreFabId = prefabID;
            MaxSlots = maxSlots;
        }

        public void ClearOldData()
        {
            this.Values.Clear();
        }

        public IList<CyNukeRodSaveData> SlotData => this.Values;

        public void AddSlotData(IList<SlotData> slotDataCollection)
        {
            for (int r = 0; r < slotDataCollection.Count; r++)
                this.Values.Add(new CyNukeRodSaveData(slotDataCollection[r]));

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
            var cyNukeReactorSaveData = new CyNukeReactorSaveData(PreFabId, MaxSlots);

            for (int r = 0; r < this.Values.Count; r++)
                cyNukeReactorSaveData.Values.Add(this.Values[r].Copy() as CyNukeRodSaveData);

            return cyNukeReactorSaveData;
        }
    }
}
