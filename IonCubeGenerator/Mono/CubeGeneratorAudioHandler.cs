using UnityEngine;

namespace IonCubeGenerator.Mono
{
    internal class CubeGeneratorAudioHandler
    {

        #region Constructor
        /// <summary>
        /// Default Constructor 
        /// </summary>
        public CubeGeneratorAudioHandler()
        {
            LoadFmodAssets();

        }
        #endregion


        /// <summary>
        /// Sound Effect for the water filtration machine;
        /// </summary>
        public FMODAsset WATER_FILTER_LOOP { get; private set; }

        internal void LoadFmodAssets()
        {
            Resources.Load<GameObject>("Submarine/Build/FiltrationMachine");

            FMODAsset[] fmods = Resources.FindObjectsOfTypeAll<FMODAsset>();

            foreach (FMODAsset fmod in fmods)
            {
                switch (fmod.name.ToLower())
                {
                    case "water_filter_loop":
                        WATER_FILTER_LOOP = fmod;
                        break;
                }
            }

        }
    }
}

