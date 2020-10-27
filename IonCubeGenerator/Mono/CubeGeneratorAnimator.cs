namespace IonCubeGenerator.Mono
{
    using Common;
    using UnityEngine;
    using IonCubeGenerator.Configuration;
    using IonCubeGenerator.Enums;
    // using Logger = QModManager.Utility.Logger;

    internal class CubeGeneratorAnimator : MonoBehaviour
    {
        #region Private Members
        private CubeGeneratorMono _mono;
        private const float ArmAnimationStart = 0.146606f; //0.1415817f;
        private const float ArmAnimationEnd = 0.6554104f; //0.6440131f;
        private bool _loaded;
        private CubeGeneratorAudioHandler _audioHandler;

        private const int MAIN_ANIMATION_LAYER = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// The animator component from the gameObject.
        /// </summary>
        public Animator Animator { get; private set; }

        #endregion

        #region Unity Methods

        private void Start()
        {
            this.Animator = this.transform.GetComponent<Animator>();

            _audioHandler = new CubeGeneratorAudioHandler(gameObject.GetComponent<FMOD_CustomLoopingEmitter>());

            if (this.Animator == null)
            {
                QuickLogger.Error("Animator component not found on the GameObject.");
                _loaded = false;
            }

            _mono = this.transform.GetComponent<CubeGeneratorMono>();

            if (_mono == null)
            {
                QuickLogger.Error("CubeGeneratorMono component not found on the GameObject.");
                _loaded = false;
            }

            if (this.Animator != null && this.Animator.enabled == false)
            {
                // Logger.Log(Logger.Level.Debug, "Animator was disabled and now has been enabled");
                this.Animator.enabled = true;
            }

            _loaded = true;
        }

        private void Update()
        {
            if (!_loaded)
                return;

            UpdateAudioState();

            UpdateArm();
        }

        private void UpdateAudioState()
        {
            if (!_mono.IsConstructed || _audioHandler == null)
                return;

            if (_mono.GenerationPercent > 0.01 &&
                _mono.GenerationPercent < 1 &&
                _mono.CurrentSpeedMode != SpeedModes.Off &&
                ModConfiguration.Singleton.AllowSFX)
            {
                _audioHandler.PlayFilterMachineAudio();
            }
            else
            {
                _audioHandler.StopFilterMachineAudio();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateArm()
        {
            float _outputBar = 0f;

            if (_mono.StartUpPercent < 1)
            {
                _outputBar = _mono.StartUpPercent * ArmAnimationStart;
            }
            else if (_mono.GenerationPercent < 1)
            {
                _outputBar = _mono.GenerationPercent * (ArmAnimationEnd - ArmAnimationStart) + ArmAnimationStart;
            }
            else if (_mono.CoolDownPercent < 1)
            {
                _outputBar = _mono.CoolDownPercent * (1 - ArmAnimationEnd) + ArmAnimationEnd;
            }

            ChangeAnimationPointer(_outputBar);
        }

        private void ChangeAnimationPointer(float percent)
        {
            this.Animator.Play("Main_BK", MAIN_ANIMATION_LAYER, percent);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Sets the an animator float to a certain value (For use with setting the page on the screen)
        /// </summary>
        /// <param name="stateHash">The hash of the parameter</param>
        /// <param name="value">Float to set</param>
        public void SetFloatHash(int stateHash, float value)
        {
            this.Animator.SetFloat(stateHash, value);
        }

        #endregion
    }
}
