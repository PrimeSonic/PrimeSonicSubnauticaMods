namespace IonCubeGenerator.Mono
{
    using Common;
    using System.Collections;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private bool _safeToAnimate;
        private Animator _animator;
        private bool _AnimatorPausedState;

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

            }
        }

        private void AnimationWorkingState()
        {
            QuickLogger.Debug(@"Working State");
            StartCoroutine(PlayAnimationEnu(1));
        }

        private void AnimationIdleState()
        {
            QuickLogger.Debug(@"Idle State");
            StartCoroutine(IdleAnimationEnu());
        }

        private void PauseAnimation()
        {
            QuickLogger.Debug(@"Paused State");
            StartCoroutine(PauseAnimationEnu());
            _AnimatorPausedState = true;
        }

        private void ResumeAnimation()
        {
            QuickLogger.Debug(@"Resuming");
            StartCoroutine(ResumeAnimationEnu());
            _AnimatorPausedState = false;
        }

        private void ResetAnimatorProperties()
        {
            _animator.SetBool("Working", false);
        }

        #region IEnumerators

        private IEnumerator PlayAnimationEnu(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (_safeToAnimate)
            {
                _animator.SetBool("Working", true);
            }
        }

        private IEnumerator IdleAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                _animator.SetBool("Working", false);
            }
        }

        private IEnumerator PauseAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                _animator.speed = 0;
            }
        }

        private IEnumerator ResumeAnimationEnu()
        {
            yield return new WaitForEndOfFrame();
            if (_safeToAnimate)
            {
                _animator.speed = 1;
            }
        }
        #endregion
    }
}
