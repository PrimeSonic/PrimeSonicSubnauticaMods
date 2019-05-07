namespace IonCubeGenerator
{
    using Common;
    using Common.EasyMarkup;
    using SMLHelper.V2.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class CubeGeneratorSaveData : EmPropertyCollection
    {
        private const string CubeCountKey = "CC";
        private const string TimeNextCubeKey = "TNC";
        private const string SpeedModesKey = "SM";
        private const string MainKey = "IG";

        private static readonly ICollection<EmProperty> emProperties = new List<EmProperty>(2)
        {
            new EmProperty<int>(CubeCountKey),
            new EmProperty<float>(TimeNextCubeKey),
            new EmProperty<SpeedModes>(SpeedModesKey, SpeedModes.Low),
        };

        private readonly string _preFabID;
        private readonly int _maxCubes;
        private readonly EmProperty<int> _cubeCount;
        private readonly EmProperty<float> _timeToCube;
        private readonly EmProperty<SpeedModes> _speedMode;

        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "IonCubeGenerator");
        private string SaveFile => Path.Combine(SaveDirectory, _preFabID + ".txt");

        internal int NumberOfCubes
        {
            get => Math.Min(_maxCubes, _cubeCount.Value);
            set => _cubeCount.Value = Math.Min(_maxCubes, value);
        }

        internal float RemainingTimeToNextCube
        {
            get => _timeToCube.Value;
            set => _timeToCube.Value = value;
        }

        internal SpeedModes Mode
        {
            get => _speedMode.Value;
            set => _speedMode.Value = value;
        }

        public CubeGeneratorSaveData(string prefabId, int maxCubes) : this(MainKey, emProperties)
        {
            _preFabID = prefabId;
            _maxCubes = maxCubes;
        }

        private CubeGeneratorSaveData(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            _cubeCount = (EmProperty<int>)base.Properties[CubeCountKey];
            _timeToCube = (EmProperty<float>)base.Properties[TimeNextCubeKey];
        }

        internal override EmProperty Copy()
        {
            return new CubeGeneratorSaveData(this.Key, this.CopyDefinitions);
        }

        public void SaveData()
        {
            this.Save(SaveDirectory, this.SaveFile);
        }

        public bool LoadData()
        {
            return this.Load(SaveDirectory, this.SaveFile);
        }
    }
}
