namespace CyclopsBioReactor.Management
{
    using CyclopsBioReactor.Items;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CyBioReactorDisplayHandler
    {
        internal static CyBioConfig Config;

        private readonly CyBioReactorMono _mono;
        private readonly GameObject _gameobject;
        private Text _status;

        internal CyBioReactorDisplayHandler(CyBioReactorMono mono)
        {
            _mono = mono;
            _gameobject = mono.gameObject;
            FindAllComponents();
        }

        internal void TurnOnDisplay()
        {
            _mono.AnimationHandler.SetBoolHash(_mono.ScreenStateHash, true);
        }

        private void FindAllComponents()
        {
            if (_gameobject == null)
            {
                MCUServices.Logger.Error("GameObject is null canceling screen setup.");
                return;
            }

            GameObject canvas = _gameobject.GetComponentInChildren<Canvas>()?.gameObject;

            if (canvas == null)
            {
                MCUServices.Logger.Error("Canvas is null canceling screen setup.");
                return;
            }

            _status = canvas.FindChild("Home").FindChild("Status").GetComponent<Text>();

            if (_status == null)
            {
                MCUServices.Logger.Error("Status label is null canceling screen setup.");
                return;
            }
        }

        public void SetActive(int charge, int capacity, bool draining)
        {
            if (Config?.EnergyOnDisplay == false)
            {
                _status.text = Language.main.Get("BaseBioReactorActive");
            }
            else if (draining)
            {
                if (charge == 0)
                {
                    _status.text = $"{Language.main.Get("BaseBioReactorActive")}\n{CyBioReactor.ChargingCyclopsText}";
                }
                else
                {
                    _status.text = $"{Language.main.Get("BaseBioReactorActive")}\n {charge}/{capacity}+\n{CyBioReactor.ChargingCyclopsText}";
                }
            }
            else
            {
                _status.text = $"{Language.main.Get("BaseBioReactorActive")}\n {charge}/{capacity}+";
            }

            _status.color = Color.green;
        }

        public void SetInActivating(int charge, int capacity, bool draining)
        {
            if (Config?.EnergyOnDisplay == false)
            {
                _status.text = Language.main.Get("BaseBioReactorInactive");
            }
            else if (draining)
            {
                _status.text = $"{Language.main.Get("BaseBioReactorInactive")}\n{charge}/{capacity}\n{CyBioReactor.ChargingCyclopsText}";
            }
            else
            {
                _status.text = $"{Language.main.Get("BaseBioReactorInactive")}\n{charge}/{capacity}";
            }

            _status.color = Color.yellow;
        }

        public void SetInactive()
        {
            _status.text = Language.main.Get("BaseBioReactorInactive");
            _status.color = Color.red;
        }
    }
}
