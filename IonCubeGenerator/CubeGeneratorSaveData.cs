namespace IonCubeGenerator
{
    using Common;
    using Common.EasyMarkup;
    using IonCubeGenerator.Enums;
    using IonCubeGenerator.Interfaces;
    using SMLHelper.V2.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class CubeGeneratorSaveData : EmPropertyCollection, ICubeGeneratorSaveHandler
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

        internal SpeedModes CurrentSpeedMode
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
            _speedMode = (EmProperty<SpeedModes>)base.Properties[SpeedModesKey];
        }

        internal override EmProperty Copy()
        {
            return new CubeGeneratorSaveData(this.Key, this.CopyDefinitions);
        }

        public void SaveData(ICubeGeneratorSaveData cubeGenerator)
        {
            this.NumberOfCubes = cubeGenerator.NumberOfCubes;
            this.RemainingTimeToNextCube = cubeGenerator.RemainingTimeToNextCube;
            this.CurrentSpeedMode = cubeGenerator.CurrentSpeedMode;

            this.Save(SaveDirectory, this.SaveFile);
        }

        public void LoadData(ICubeGeneratorSaveData cubeGenerator)
        {
            if (this.Load(SaveDirectory, this.SaveFile))
            {
                cubeGenerator.IsLoadingSaveData = true;

                cubeGenerator.NumberOfCubes = this.NumberOfCubes;
                cubeGenerator.RemainingTimeToNextCube = this.RemainingTimeToNextCube;
                cubeGenerator.CurrentSpeedMode = this.CurrentSpeedMode;

                cubeGenerator.IsLoadingSaveData = false;
            }
        }
    }
}
