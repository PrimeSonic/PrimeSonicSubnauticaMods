namespace CyclopsBioReactor.Management
{
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal class CyBioReactorAudioHandler : MonoBehaviour
    {
        private FMODAsset _doorOpen;
        private FMODAsset _doorClose;
        private bool _allowedToPlaySounds;
        private readonly Transform _transform;

        /// <summary>
        /// Default Constructor 
        /// </summary>
        public CyBioReactorAudioHandler(Transform transform)
        {
            _transform = transform;
            LoadFModAssets();
        }

        private void LoadFModAssets()
        {
            FMODAsset[] fmods = Resources.FindObjectsOfTypeAll<FMODAsset>();

            foreach (FMODAsset fmod in fmods)
            {
                switch (fmod.name.ToLower())
                {
                    case "bioreactor_hatch_close":
                        MCUServices.Logger.Debug("bioreactor_hatch_close found!", true);
                        _doorClose = fmod;
                        break;

                    case "bioreactor_hatch_open":
                        MCUServices.Logger.Debug("bioreactor_hatch_open found!", true);
                        _doorOpen = fmod;
                        break;
                }
            }

            if (_doorClose == null)
            {
                MCUServices.Logger.Debug("bioreactor_hatch_close not found trying to search again...", true);
                Resources.Load<GameObject>("/sub/base/bioreactor_hatch_close");
                LoadFModAssets();
            }

            if (_doorOpen == null)
            {
                MCUServices.Logger.Debug("bioreactor_hatch_open not found trying to search again...", true);
                Resources.Load<GameObject>("/sub/base/bioreactor_hatch_open");
                LoadFModAssets();
            }
        }

        /// <summary>
        /// Sets the field that allows sound to be played or not.
        /// </summary>
        /// <param name="value">Value to be set for the player to play audio. Options (true/false)</param>
        internal void SetSoundActive(bool value)
        {
            _allowedToPlaySounds = value;
        }

        /// <summary>
        /// Plays the door open/close sound clip
        /// </summary>
        /// <param name="doorState"></param>
        internal void PlayDoorSoundClip(bool doorState)
        {
            if (!_allowedToPlaySounds)
                return;
            FMODAsset asset = !doorState ? _doorOpen : _doorClose;
            if (!(asset != null))
                return;
            FMODUWE.PlayOneShot(asset, _transform.position, 1f);
        }
    }
}
