using Common;
using UnityEngine;

namespace IonCubeGenerator.Mono
{
    internal class CubeGeneratorAudioHandler
    {
        #region Private Members
        private readonly FMOD_CustomLoopingEmitter _loopingEmitter;
        private bool _soundPlaying;
        private FMODAsset _waterFilterLoop;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor 
        /// </summary>
        public CubeGeneratorAudioHandler(FMOD_CustomLoopingEmitter emitter)
        {
            LoadFModAssets();

            _loopingEmitter = emitter;

            _loopingEmitter.asset = _waterFilterLoop;
        }
        #endregion

        #region Private Methods
        private void LoadFModAssets()
        {
            FMODAsset[] fmods = Resources.FindObjectsOfTypeAll<FMODAsset>();

            foreach (FMODAsset fmod in fmods)
            {
                switch (fmod.name.ToLower())
                {
                    case "water_filter_loop":
                        QuickLogger.Debug("WATER_FILTER_LOOP found!", true);
                        this._waterFilterLoop = fmod;
                        break;
                }
            }

            if (_waterFilterLoop == null)
            {
                QuickLogger.Debug("WATER_FILTER_LOOP not found trying to search again...", true);
                Resources.Load<GameObject>("Submarine/Build/FiltrationMachine");
                LoadFModAssets();
            }
        }
        #endregion

        #region Internal Methods    
        /// <summary>
        /// Plays the filtration machine audio.
        /// </summary>
        internal void PlayFilterMachineAudio()
        {
            if (!_soundPlaying)
            {
                _loopingEmitter.Play();
                _soundPlaying = true;
            }
        }

        /// <summary>
        /// Stops the filtration machine audio.
        /// </summary>
        internal void StopFilterMachineAudio()
        {
            if (_soundPlaying)
            {
                _loopingEmitter.Stop();
                _soundPlaying = false;
            }
        }
        #endregion
    }
}

