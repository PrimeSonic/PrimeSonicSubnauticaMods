using System;

namespace IonCubeGenerator.Mono
{
    using Common;
    using System.Collections;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        #region Private Members
        private bool _safeToAnimate;
        internal Animator Animator;
        private bool _animatorPausedState;
        private bool _isWorking;
        private int _speedHash;
        private int _workingHash;

        private const float MinValue = 0.087f;
        private const float MaxValue = 0.409f;
        private const float ArmAnimationStart = 0.146606f; //0.1415817f;
        private const float ArmAnimationEnd = 0.6554104f; //0.6440131f;
        private const float MaxBar = 100f;

        private bool _coolDownPeriod;
        private float _currentNormilzedTime;
        private AnimatorStateInfo _animationState;
        #endregion

        #region Unity Methods
        private void LateUpdate()
        {
            UpdatePercentageBar();
        } 
        #endregion
        
        #region Private Methods
        private void RetrieveAnimator()
        {
            if (!IsConstructed) return;

            if (Animator == null)
            {
                Animator = this.transform.GetComponent<Animator>();
            }
            else
            {
                QuickLogger.Error("Animator component not found on the prefab trying again.");
                _safeToAnimate = false;
                return;
            }
            
            _safeToAnimate = true;
            _speedHash = Animator.StringToHash("speed");
            _workingHash = Animator.StringToHash("Working");
            SetBar();
        }

        private void UpdateCoolDown()
        {
            _animationState = Animator.GetCurrentAnimatorStateInfo(0);
            _currentNormilzedTime = _animationState.normalizedTime;

            if (Math.Round(_currentNormilzedTime, 2) < Math.Round(ArmAnimationStart, 2) && NextCubePercentage != 100)
            {
                _coolDownPeriod = true;

            }
            else if (Math.Round(_currentNormilzedTime, 2) > Math.Round(ArmAnimationEnd, 2) && NextCubePercentage != 100)
            {
                _coolDownPeriod = true;
            }
            else
            {
                _coolDownPeriod = false;
            }

            if (Math.Round(_animationState.normalizedTime, 2) >= 1)
            {
                SetAnimationState(0);
                if (this.CurrentCubeCount == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y))
                {
                    //Pause the animator
                    PauseAnimation();
                }
            }

        }

        private void AnimationWorkingState()
        {
            if (_isWorking) return;
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
            if (_animatorPausedState) return;

            StartCoroutine(PauseAnimationEnu());
            _animatorPausedState = true;
            _isWorking = false;
        }

        private void ResumeAnimation()
        {
            if (Mathf.Approximately(Animator.GetFloat(_speedHash), 1f)) return;
            StartCoroutine(ResumeAnimationEnu());
            _animatorPausedState = false;
        }

        private void UpdatePercentageBar()
        {
            if (Animator != null)
            {
                UpdateCoolDown();

                if (NextCubePercentage < 100)
                {
                    float calcBar = NextCubePercentage / MaxBar;
                    float outputBar = calcBar * (ArmAnimationEnd - ArmAnimationStart) + ArmAnimationStart;

                    if (!_coolDownPeriod)
                    {
                        UpdateArmPosition(outputBar);
                    }
                }

                //Update Percentage
                if (_display != null)
                {
                    _display.UpdatePercentageText(NextCubePercentage);
                }


                SetBar();
                SetStorageBar();
            }
        }

        private void UpdateArmPosition(float percent)
        {
            SetAnimationState(percent);
        }

        private void SetAnimationState(float percent)
        {
            QuickLogger.Debug($"Arm position changed to {percent}"); 
            Animator.Play("Main", 0, percent);
        }

        private void SetBar()
        {
            float calcBar = NextCubePercentage / MaxBar;
            float outputBar = calcBar * (MaxValue - MinValue) + MinValue;

            if (_display != null)
            {
                _display.UpdatePercentageBar(outputBar, MinValue, MaxValue);
            }
        }

        private void SetStorageBar()
        {
            float calcBar = (float)((CurrentCubeCount * 1.0) / (MaxAvailableSpaces * 1.0));
            float outputBar = calcBar * (MaxValue - MinValue) + MinValue;

            if (_display == null) return;

            _display.UpdateStoragePercentBar(outputBar, MinValue, MaxValue);

            _display.UpdateStorageAmount(CurrentCubeCount, MaxAvailableSpaces);
        }
        #endregion

        #region IEnumerators

        private IEnumerator PlayAnimationEnu(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (_safeToAnimate)
            {
                Animator.SetBool(_workingHash, true);
                _isWorking = true;
            }
        }

        private IEnumerator IdleAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                Animator.SetBool(_workingHash, false);
            }
        }

        private IEnumerator PauseAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                QuickLogger.Debug(@"Paused State");
                Animator.SetFloat(_speedHash, 0);
            }
        }

        private IEnumerator ResumeAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                QuickLogger.Debug(@"Resuming");
                Animator.SetFloat(_speedHash, 1);
            }
        }
        #endregion
    }
}
