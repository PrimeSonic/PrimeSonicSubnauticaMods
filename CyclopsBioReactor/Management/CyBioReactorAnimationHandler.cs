namespace CyclopsBioReactor.Management
{
    using UnityEngine;
    using MoreCyclopsUpgrades.API;

    internal class CyBioReactorAnimationHandler
    {
        private Animator _animator;
        private CyBioReactorMono _mono;

        internal CyBioReactorAnimationHandler(CyBioReactorMono mono)
        {
            _mono = mono;
            _animator = mono.transform.GetComponent<Animator>();

            if (_animator == null)
            {
                MCUServices.Logger.Error("Animator component not found on the GameObject.");
            }

            if (_animator != null && _animator.enabled == false)
            {
                MCUServices.Logger.Debug("Animator was disabled and now has been enabled");
                _animator.enabled = true;
            }
        }

        /// <summary>
        /// Sets the an animator boolean to a certain value
        /// </summary>
        /// <param name="stateHash">The hash of the parameter</param>
        /// <param name="value">Float to set</param>
        internal void SetBoolHash(int stateHash, bool value)
        {
            if (_animator == null)
            {
                _animator = _mono.transform.GetComponent<Animator>();
            }

            _animator.SetBool(stateHash, value);
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
                _animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null)
                return;

            _animator.SetInteger(stateHash, value);
        }

        internal int GetIntHash(int hash)
        {
            if (_animator == null)
            {
                _animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null)
                return 0;

            return _animator.GetInteger(hash);
        }

        internal bool GetBoolHash(int hash)
        {
            if (_animator == null)
            {
                _animator = _mono.transform.GetComponent<Animator>();
            }

            if (_animator == null)
                return false;

            return _animator.GetBool(hash);
        }
    }
}
