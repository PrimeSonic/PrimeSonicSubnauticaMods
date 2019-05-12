namespace IonCubeGenerator.Mono
{
    using Common;
    using System;
    using System.Collections;
    using UnityEngine;

    internal class CubeGeneratorAnimator : MonoBehaviour
    {
        #region Private Members
        private const float MaxProgress = CubeGeneratorMono.ProgressComplete;

        private bool _animatorPausedState;
        private bool _isWorking;
        private int _speedHash;
        private int _workingHash;
        private CubeGeneratorMono _mono;

        private const float ArmAnimationStart = 0.146606f; //0.1415817f;
        private const float ArmAnimationEnd = 0.6554104f; //0.6440131f;

        private float _currentNormilzedTime;
        private AnimatorStateInfo _animationState;
        private bool _loaded;
        private bool _soundPlaying;
        private CubeGeneratorAudioHandler _audioHandler;
        private FMOD_CustomLoopingEmitter loopingEmitter;
        private const float ANIMATION_START_BUFFER = 0.1f;
        private const float ANIMATION_START = 0.0f;
        private const float ANIMATION_END = 1.0f;
        private const float ANIMATION_SPEED_MAX = 1.0f;
        private const float ANIMATION_SPEED_MIN = 0.0f;
        private const int MAIN_ANIMATION_LAYER = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// The animator component from the gameObject.
        /// </summary>
        public Animator Animator { get; private set; }
        public bool InCoolDown { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _audioHandler = new CubeGeneratorAudioHandler();
        }

        private void Start()
        {
            this.Animator = this.transform.GetComponent<Animator>();
            loopingEmitter = this.transform.GetComponent<FMOD_CustomLoopingEmitter>();
            loopingEmitter.asset = _audioHandler.WATER_FILTER_LOOP;

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

            _speedHash = Animator.StringToHash("speed");
            _workingHash = Animator.StringToHash("Working");

            if (this.Animator != null && this.Animator.enabled == false)
            {
                QuickLogger.Debug("Animator was disabled and now has been enabled");
                this.Animator.enabled = true;
            }

            _loaded = true;

            AnimationWorkingState();
        }

        private void Update()
        {
            if (!_loaded)
                return;

            UpdateCoolDown();

            //UpdatePauseOrResumeToggle();

            //UpdateArm();

        }
        #endregion

        #region Private Methods

        private void UpdateCoolDown()
        {
            if (_mono.NotAllowToGenerate)
                return;

            _animationState = this.Animator.GetCurrentAnimatorStateInfo(0);
            _currentNormilzedTime = _animationState.normalizedTime;

            //QuickLogger.Debug($"Startup Percent = {_mono.StartUpPercent}");
            //QuickLogger.Debug($"Generation Percent = {_mono.GenerationPercent}");
            //QuickLogger.Debug($"Cool DOwn Percent = {_mono.CoolDownPercent}");

            //if (Math.Round(_currentNormilzedTime, 2) < Math.Round(ArmAnimationStart, 2) && _mono.GenerationPercent != 100)
            //{
            //    this.InCoolDown = true;
            //}
            //else if (Math.Round(_currentNormilzedTime, 2) > Math.Round(ArmAnimationEnd, 2) && _mono.GenerationPercent != 100)
            //{
            //    this.InCoolDown = true;
            //}
            //else
            //{
            //    this.InCoolDown = false;
            //}
        }
        #endregion

        #region Private Methods

        private void UpdatePauseOrResumeToggle()
        {
            if (_mono.IsFull && Math.Round(_animationState.normalizedTime, 2) <= ANIMATION_START_BUFFER)
            {
                //Pause the animator
                PauseAnimation();
            }
            else
            {
                //Resume the animator
                ResumeAnimation();
            }
        }

        private void UpdateArm()
        {
            if (_mono.NotAllowToGenerate)
                return;

            if (Math.Round(_animationState.normalizedTime, 2) >= ANIMATION_END)
            {
                ChangeAnimationPointer(ANIMATION_START);
                return;
            }

            if (_mono.GenerationPercent < MaxProgress)
            {
                float outputBar = _mono.GenerationPercent / MaxProgress * (ArmAnimationEnd - ArmAnimationStart) + ArmAnimationStart;

                if (!this.InCoolDown)
                {
                    ChangeAnimationPointer(outputBar);

                }
            }
        }

        private void AnimationWorkingState()
        {
            if (_isWorking)
                return;
            QuickLogger.Debug(@"Working State");
            StartCoroutine(PlayAnimationEnu(1));
            _animatorPausedState = false;
        }

        private void AnimationIdleState()
        {
            QuickLogger.Debug(@"Idle State");
            StartCoroutine(IdleAnimationEnu());
        }

        private void PauseAnimation()
        {
            if (_animatorPausedState)
                return;
            StopAudio();

            StartCoroutine(PauseAnimationEnu());
            _animatorPausedState = true;
            _isWorking = false;
        }

        private void ResumeAnimation()
        {
            if (Mathf.Approximately(this.Animator.GetFloat(_speedHash), ANIMATION_SPEED_MAX))
                return;

            PlayAudio();

            StartCoroutine(ResumeAnimationEnu());
            _animatorPausedState = false;
        }

        private void ChangeAnimationPointer(float percent)
        {
            this.Animator.Play("Main", MAIN_ANIMATION_LAYER, percent);
        }

        #endregion

        #region IEnumerators

        private IEnumerator PlayAnimationEnu(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            this.Animator.SetBool(_workingHash, true);
            _isWorking = true;
        }

        private IEnumerator IdleAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            this.Animator.SetBool(_workingHash, false);
        }

        private IEnumerator PauseAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            QuickLogger.Debug(@"Paused State");
            this.Animator.SetFloat(_speedHash, ANIMATION_SPEED_MIN);
        }

        private IEnumerator ResumeAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            QuickLogger.Debug(@"Resuming");
            this.Animator.SetFloat(_speedHash, ANIMATION_SPEED_MAX);
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

        internal void PlayAudio()
        {
            if (!_soundPlaying)
            {
                loopingEmitter.Play();
                _soundPlaying = true;
            }
        }

        internal void StopAudio()
        {
            if (_soundPlaying)
            {
                loopingEmitter.Stop();
                _soundPlaying = false;
            }
        }

        #endregion
    }
}
