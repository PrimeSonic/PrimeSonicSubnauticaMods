using System;
using UnityEngine;

namespace CyclopsBioReactor
{
    internal class CyBioreactorTrigger : MonoBehaviour
    {
        internal Action OnPlayerEnter;
        internal Action OnPlayerExit;
        internal Action OnPlayerStay;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;
            //MCUServices.Logger.Debug("In OnTriggerEnter");
            OnPlayerEnter?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;
            //MCUServices.Logger.Debug($"In OnTriggerStay {other.gameObject.name}", true);
            OnPlayerStay?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;

            //MCUServices.Logger.Debug("In OnTriggerExit");
            OnPlayerExit?.Invoke();
        }
    }
}
