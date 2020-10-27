namespace IonCubeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using EasyMarkup;
    using IonCubeGenerator.Enums;
    using IonCubeGenerator.Interfaces;
    using IonCubeGenerator.Mono;
    using SMLHelper.V2.Utility;
    // using Logger = QModManager.Utility.Logger;

    internal class CubeGeneratorSaveData : EmPropertyCollection, ICubeGeneratorSaveHandler, ICubeGeneratorSaveData
    {
        private const string MainKey = "IG";
        private const string CubeCountKey = "CC";
        private const string ProgressKey = "P";
        private const string SpeedModesKey = "SM";

        private const int MaxCubes = CubeGeneratorContainer.MaxAvailableSpaces;

        private static readonly ICollection<EmProperty> emProperties = new List<EmProperty>(2)
        {
            new EmProperty<int>(CubeCountKey),
            new EmPropertyList<float>(ProgressKey),
            new EmProperty<SpeedModes>(SpeedModesKey, SpeedModes.Low),
        };

        private readonly string _preFabID;
        private readonly EmProperty<int> _cubeCount;
        private readonly EmPropertyList<float> _progress;
        private readonly EmProperty<SpeedModes> _speedMode;

        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "IonCubeGenerator");
        private string SaveFile => Path.Combine(SaveDirectory, _preFabID + ".txt");

        public int NumberOfCubes
        {
            get => Math.Min(MaxCubes, _cubeCount.Value);
            set => _cubeCount.Value = Math.Min(MaxCubes, value);
        }

        public float StartUpProgress
        {
            get => _progress.Values[(int)CubePhases.StartUp];
            set => _progress.Values[(int)CubePhases.StartUp] = value;
        }

        public float GenerationProgress
        {
            get => _progress.Values[(int)CubePhases.Generating];
            set => _progress.Values[(int)CubePhases.Generating] = value;
        }

        public float CoolDownProgress
        {
            get => _progress.Values[(int)CubePhases.CoolDown];
            set => _progress.Values[(int)CubePhases.CoolDown] = value;
        }

        public SpeedModes CurrentSpeedMode
        {
            get => _speedMode.Value;
            set => _speedMode.Value = value;
        }

        public CubeGeneratorSaveData(string prefabId) : this(MainKey, emProperties)
        {
            _preFabID = prefabId;
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
            // Logger.Log(Logger.Level.Debug, "Saving data");

            this.NumberOfCubes = cubeGenerator.NumberOfCubes;

            _progress.Values.Clear();
            _progress.Add(-1f);
            _progress.Add(-1f);
            _progress.Add(-1f);
            this.StartUpProgress = cubeGenerator.StartUpProgress;
            this.GenerationProgress = cubeGenerator.GenerationProgress;
            this.CoolDownProgress = cubeGenerator.CoolDownProgress;

            this.CurrentSpeedMode = cubeGenerator.CurrentSpeedMode;

            this.Save(SaveDirectory, this.SaveFile);
            // Logger.Log(Logger.Level.Debug, "Data saved");
        }

        public void LoadData(ICubeGeneratorSaveData cubeGenerator)
        {
            // Logger.Log(Logger.Level.Debug, "Searching for save data");
            if (this.Load(SaveDirectory, this.SaveFile))
            {
                // Logger.Log(Logger.Level.Debug, "Save data found");

                cubeGenerator.NumberOfCubes = this.NumberOfCubes;
                // Logger.Log(Logger.Level.Debug, $"NumberOfCubes {cubeGenerator.NumberOfCubes} <-- {this.NumberOfCubes}");

                cubeGenerator.StartUpProgress = this.StartUpProgress;
                // Logger.Log(Logger.Level.Debug, $"StartUpProgress {cubeGenerator.StartUpProgress} <-- {this.StartUpProgress}");

                cubeGenerator.GenerationProgress = this.GenerationProgress;
                // Logger.Log(Logger.Level.Debug, $"GenerationProgress {cubeGenerator.GenerationProgress} <-- {this.GenerationProgress}");

                cubeGenerator.CoolDownProgress = this.CoolDownProgress;
                // Logger.Log(Logger.Level.Debug, $"CoolDownProgress {cubeGenerator.CoolDownProgress} <-- {this.CoolDownProgress}");

                cubeGenerator.CurrentSpeedMode = this.CurrentSpeedMode;
                // Logger.Log(Logger.Level.Debug, $"CoolDownProgress {cubeGenerator.CurrentSpeedMode} <-- {this.CurrentSpeedMode}");

                // Logger.Log(Logger.Level.Debug, "Save data loaded");
            }
        }
    }
}
