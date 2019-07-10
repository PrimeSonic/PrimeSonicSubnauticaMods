using Common;
using UnityEngine;
using UnityEngine.UI;

namespace CyclopsBioReactor.Management
{
    internal class CyBioReactorDisplayHandler
    {
        private CyBioReactorMono _mono;
        private GameObject _gameobject;
        private Text _status;

        internal void Setup(CyBioReactorMono mono)
        {
            _mono = mono;
            _gameobject = mono.gameObject;
            FindAllComponents();
        }

        internal void TurnOnDisplay()
        {
            _mono.AnimationHandler.SetBoolHash(_mono.ScreenStateHash, true);
        }

        private bool FindAllComponents()
        {
            if (_gameobject == null)
            {
                QuickLogger.Error("GameObject is null canceling screen setup.");
                return false;
            }

            var canvas = _gameobject.GetComponentInChildren<Canvas>()?.gameObject;

            if (canvas == null)
            {
                QuickLogger.Error("Canvas is null canceling screen setup.");
                return false;
            }

            _status = canvas.FindChild("Home").FindChild("Status").GetComponent<Text>();

            if (_status == null)
            {
                QuickLogger.Error("Status label is null canceling screen setup.");
                return false;
            }
            return true;
        }

        public void UpdateScreen(bool isOperating)
        {
            _status.text = isOperating ? $"<color=#00ff00>{Language.main.Get("BaseBioReactorActive")}</color>" : $"<color=#ff0000>{Language.main.Get("BaseBioReactorInactive")}</color>"; ;
        }
    }
}
