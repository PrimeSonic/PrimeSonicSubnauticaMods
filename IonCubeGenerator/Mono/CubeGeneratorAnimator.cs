using UnityEngine.UI;

namespace IonCubeGenerator.Mono
{
    using Common;
    using System.Collections;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private bool _safeToAnimate;
        private Animator _animator;
        private bool _animatorPausedState;
        private bool _isWorking;
        private int _speedHash;
        private int _workingHash;

        private const float MinValue = 0.087f;
        private const float MaxValue = 0.409f;
        private const float ArmAnimationStart = 0.1415817f;
        private const float ArmAnimationEnd = 0.6440131f;

        private const float MaxBar = 100f;
        private readonly Color _endColor = new Color(0.13671875f, 0.7421875f, 0.8046875f, 0.99609375f);
        private readonly Color _startColor = new Color(0.99609375f, 0.0f, 0.0f, 0.99609375f);

        private Image _healthBar;
        private Image _statusBar;
        private Text _percentDisplay;


        private void RetrieveAnimator()
        {
            Animator animator = this.transform.GetComponent<Animator>();

            if (animator == null)
            {
                QuickLogger.Error("Animator component not found on the prefab");
                _safeToAnimate = false;
            }
            else
            {
                _safeToAnimate = true;
                _animator = animator;
                _animator.enabled = true;
                _speedHash = Animator.StringToHash("speed");
                _workingHash = Animator.StringToHash("Working");

            }

            if (!GetVisualBars())
            {
                QuickLogger.Error("Failed getting all visual bars and components");
            }
            SetBar();
            InvokeRepeating("UpdateStatusBar", 1f, 0.5f);

        }

        private bool GetVisualBars()
        {
            #region Canvas
            var canvasGameObject = this.transform.GetComponentInChildren<Canvas>()?.gameObject;
            if (canvasGameObject == null)
            {
                QuickLogger.Error("Canvas not found.");
                return false;
            }
            #endregion

            #region OperationPage

            var operationPage = canvasGameObject.FindChild("OperationPage")?.gameObject;

            if (operationPage == null)
            {
                QuickLogger.Error("OperationPage not found.");
                return false;
            }
            #endregion

            #region UI_Frame

            var uI_Frame = operationPage.FindChild("UI_Frame")?.gameObject;

            if (uI_Frame == null)
            {
                QuickLogger.Error("UI_Frame not found.");
                return false;
            }
            #endregion

            #region Mask

            var mask = uI_Frame.FindChild("Mask")?.gameObject;

            if (mask == null)
            {
                QuickLogger.Error("Mask not found.");
                return false;
            }
            #endregion

            #region Mask2

            var mask2 = uI_Frame.FindChild("Mask_2")?.gameObject;

            if (mask2 == null)
            {
                QuickLogger.Error("Mask_2 not found.");
                return false;
            }
            #endregion

            #region Complete

            var complete = uI_Frame.FindChild("Complete")?.gameObject;

            if (complete == null)
            {
                QuickLogger.Error("Complete not found.");
                return false;
            }

            _percentDisplay = complete.GetComponent<Text>();
            #endregion

            #region Full_Bar
            var fullBar = mask.FindChild("Full_Bar")?.gameObject;

            if (fullBar == null)
            {
                QuickLogger.Error("Full_Bar not found.");
                return false;
            }

            _healthBar = fullBar.GetComponent<Image>();
            #endregion

            #region Status_Full_Bar
            var statusFullBar = mask2.FindChild("Status_Full_Bar")?.gameObject;

            if (statusFullBar == null)
            {
                QuickLogger.Error("Status_Full_Bar not found.");
                return false;
            }

            _statusBar = statusFullBar.GetComponent<Image>();
            #endregion
            return true;
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
            if (Mathf.Approximately(_animator.GetFloat(_speedHash), 0f)) return;
            StartCoroutine(ResumeAnimationEnu());
            _animatorPausedState = false;
        }

        void UpdateStatusBar()
        {
            if (NextCubePercentage >= 100)
            {
                _statusBar.fillAmount = 0;
                _statusBar.color = _endColor;
            }
            else
            {
                float rand = Random.Range(0.0f, 1.0f);
                _statusBar.fillAmount = rand;
                _statusBar.color = Color.Lerp(_endColor, _startColor, rand);
            }

            if (_percentDisplay != null)
            {
                _percentDisplay.text = NextCubePercentage + "%";
            }

        }

        void UpdatePercentageBar()
        {
            if (NextCubePercentage < 100)
            {
                float calcBar = NextCubePercentage / MaxBar;
                float outputBar = calcBar * (ArmAnimationEnd - ArmAnimationStart) + ArmAnimationStart;
                _animator.Play("Main", 0, outputBar);
            }

            SetBar();
        }

        private void SetBar()
        {
            float calcBar = NextCubePercentage / MaxBar;
            float outputBar = calcBar * (MaxValue - MinValue) + MinValue;

            _healthBar.fillAmount = Mathf.Clamp(outputBar, MinValue, MaxValue);
        }

        #region IEnumerators

        private IEnumerator PlayAnimationEnu(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (_safeToAnimate)
            {
                _animator.SetBool(_workingHash, true);
                _isWorking = true;
            }
        }

        private IEnumerator IdleAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                _animator.SetBool(_workingHash, false);
            }
        }

        private IEnumerator PauseAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                QuickLogger.Debug(@"Paused State");
                _animator.SetFloat(_speedHash, 0);
            }
        }

        private IEnumerator ResumeAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                QuickLogger.Debug(@"Resuming");
                _animator.SetFloat(_speedHash, 1);
            }
        }
        #endregion
    }
}
