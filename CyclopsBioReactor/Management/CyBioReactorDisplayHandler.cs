namespace CyclopsBioReactor.Management
{
    using Common;
    using UnityEngine;
    using UnityEngine.UI;

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

            GameObject canvas = _gameobject.GetComponentInChildren<Canvas>()?.gameObject;

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

        public void SetInactive()
        {
            _status.text = Language.main.Get("BaseBioReactorInactive");
            _status.color = Color.red;
        }

        public void SetActive(int charge, int capacity, bool stillProcessing)
        {
            _status.text = $"{Language.main.Get("BaseBioReactorActive")}\n{charge}/{capacity}{(stillProcessing ? "+" : "")}";
            _status.color = Color.green;
        }
    }
}
