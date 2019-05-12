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
        private const string ProgressKey = "P";
        private const string SpeedModesKey = "SM";
        private const string MainKey = "IG";

        private static readonly ICollection<EmProperty> emProperties = new List<EmProperty>(2)
        {
            new EmProperty<int>(CubeCountKey),
            new EmPropertyList<float>(ProgressKey, new []{ -1f, -1f, -1f }),
            new EmProperty<SpeedModes>(SpeedModesKey, SpeedModes.Low),
        };

        private readonly string _preFabID;
        private readonly int _maxCubes;
        private readonly EmProperty<int> _cubeCount;
        private readonly EmPropertyList<float> _progress;
        private readonly EmProperty<SpeedModes> _speedMode;

        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "IonCubeGenerator");
        private string SaveFile => Path.Combine(SaveDirectory, _preFabID + ".txt");

        internal int NumberOfCubes
        {
            get => Math.Min(_maxCubes, _cubeCount.Value);
            set => _cubeCount.Value = Math.Min(_maxCubes, value);
        }

        internal IList<float> Progress
        {
            get => _progress.Values;
            set
            {
                _progress.Values[(int)CubePhases.StartUp] = value[(int)CubePhases.StartUp];
                _progress.Values[(int)CubePhases.Generating] = value[(int)CubePhases.Generating];
                _progress.Values[(int)CubePhases.CoolDown] = value[(int)CubePhases.CoolDown];
            }
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
            _progress = (EmPropertyList<float>)base.Properties[ProgressKey];
            _speedMode = (EmProperty<SpeedModes>)base.Properties[SpeedModesKey];
        }

        internal override EmProperty Copy()
        {
            return new CubeGeneratorSaveData(this.Key, this.CopyDefinitions);
        }

        public void SaveData(ICubeGeneratorSaveData cubeGenerator)
        {
            this.NumberOfCubes = cubeGenerator.NumberOfCubes;
            this.Progress = cubeGenerator.Progress;
            this.CurrentSpeedMode = cubeGenerator.CurrentSpeedMode;

            this.Save(SaveDirectory, this.SaveFile);
        }

        public void LoadData(ICubeGeneratorSaveData cubeGenerator)
        {
            if (this.Load(SaveDirectory, this.SaveFile))
            {
                cubeGenerator.IsLoadingSaveData = true;

                cubeGenerator.NumberOfCubes = this.NumberOfCubes;
                cubeGenerator.Progress = this.Progress;
                cubeGenerator.CurrentSpeedMode = this.CurrentSpeedMode;

                cubeGenerator.IsLoadingSaveData = false;
            }
        }
    }
}
