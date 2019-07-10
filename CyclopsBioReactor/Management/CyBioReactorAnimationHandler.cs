using Common;
using UnityEngine;

namespace CyclopsBioReactor.Management
{
    internal class CyBioReactorAnimationHandler
    {
        internal void Setup(CyBioReactorMono mono)
        {
            _mono = mono;
            this._animator = mono.transform.GetComponent<Animator>();

            if (this._animator == null)
            {
                QuickLogger.Error("Animator component not found on the GameObject.");
            }

            if (this._animator != null && this._animator.enabled == false)
            {
                QuickLogger.Debug("Animator was disabled and now has been enabled");
                this._animator.enabled = true;
            }
        }

        #region Private Members

        private Animator _animator;
        private CyBioReactorMono _mono;

        #endregion

        #region Internal Methods
        /// <summary>
        /// Sets the an animator boolean to a certain value
        /// </summary>
        /// <param name="stateHash">The hash of the parameter</param>
        /// <param name="value">Float to set</param>
        internal void SetBoolHash(int stateHash, bool value)
        {
            if (_animator == null)
            {
                this._animator = _mono.transform.GetComponent<Animator>();
            }

            this._animator.SetBool(stateHash, value);
        }

        /// <summary>
        /// Sets the an animator integer to a certain value
        /// </summary>
        /// <param name="stateHash">The hash of the parameter</param>
        /// <param name="value">Float to set</param>
        internal void SetIntHash(int stateHash, int value)
        {
            if (_animator == null)
            {
                this._animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null) return;

            this._animator.SetInteger(stateHash, value);
        }

        internal int GetIntHash(int hash)
        {
            if (_animator == null)
            {
                this._animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null) return 0;

            return this._animator.GetInteger(hash);
        }

        internal bool GetBoolHash(int hash)
        {
            if (_animator == null)
            {
                this._animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null) return false;

            return this._animator.GetBool(hash);
        }
        #endregion

    }
}
