using Common;
using IonCubeGenerator.Enums;
using System;
using System.Collections;
using UnityEngine;

namespace IonCubeGenerator.Display
{
    public class IonGeneratorDisplay : MonoBehaviour
    {
        private GameObject _canvasGameObject;
        private GameObject _powerOffPage;
        private GameObject _operationPage;
        private GameObject _uI_Frame;
        private Action _storage;
        private Animator _animator;
        private Action<bool> _updateBreaker;
        private int _stateHash;

        /// <summary>
        /// The max distance to the screen
        /// </summary>

        #region Public Properties
        //public DeepDrillerController Controller { get; set; }
        #endregion

        private void Awake()
        {
            _stateHash = Animator.StringToHash("State");
        }

        public void Setup(Action storage, Action<bool> updateBreaker)
        {
            _storage = storage;
            _updateBreaker = updateBreaker;


            if (FindAllComponents() == false)
            {
                QuickLogger.Error("// ============== Error getting all Components ============== //");
                ShutDownDisplay();
                return;
            }

        }

        public void OnButtonClick(string btnName, object tag)
        {
            switch (btnName)
            {
                case "StorageBTN":
                    _storage?.Invoke();
                    break;

                case "UIFramePowerBTN":
                    _updateBreaker?.Invoke(true);
                    break;

                case "PowerOffPagePowerBTN":
                    _updateBreaker?.Invoke(false);
                    break;
            }
        }

        public bool FindAllComponents()
        {
            #region Canvas
            _canvasGameObject = gameObject.GetComponentInChildren<Canvas>()?.gameObject;
            if (_canvasGameObject == null)
            {
                QuickLogger.Error("Canvas not found.");
                return false;
            }
            #endregion

            #region PowerOffPage

            _powerOffPage = _canvasGameObject.FindChild("PowerOffPage")?.gameObject;

            if (_powerOffPage == null)
            {
                QuickLogger.Error("PowerOffPage not found.");
                return false;
            }
            #endregion

            #region OperationPage

            _operationPage = _canvasGameObject.FindChild("OperationPage")?.gameObject;

            if (_operationPage == null)
            {
                QuickLogger.Error("OperationPage not found.");
                return false;
            }
            #endregion

            #region UI_Frame

            _uI_Frame = _operationPage.FindChild("UI_Frame")?.gameObject;

            if (_uI_Frame == null)
            {
                QuickLogger.Error("UI_Frame not found.");
                return false;
            }
            #endregion

            #region UI_Frame Power BTN

            var uIFramePowerBtn = _uI_Frame.FindChild("Power_BTN")?.gameObject;

            if (uIFramePowerBtn == null)
            {
                QuickLogger.Error("UI_Frame Power Button not found.");
                return false;
            }

            var powerBTN = uIFramePowerBtn.AddComponent<InterfaceButton>();
            powerBTN.OnButtonClick = OnButtonClick;
            powerBTN.BtnName = "UIFramePowerBTN";
            powerBTN.ButtonMode = InterfaceButtonMode.Background;
            powerBTN.TextLineOne = $"Toggle Ion Cube Generator Power";
            powerBTN.Tag = this;
            #endregion

            #region PowerOffPage Power BTN

            var powerOffPagePowerBtn = _powerOffPage.FindChild("Power_BTN")?.gameObject;

            if (powerOffPagePowerBtn == null)
            {
                QuickLogger.Error("PowerOffPage Power Button not found.");
                return false;
            }

            var _powerOffPagePowerBTN = powerOffPagePowerBtn.AddComponent<InterfaceButton>();
            _powerOffPagePowerBTN.OnButtonClick = OnButtonClick;
            _powerOffPagePowerBTN.BtnName = "PowerOffPagePowerBTN";
            _powerOffPagePowerBTN.ButtonMode = InterfaceButtonMode.Background;
            _powerOffPagePowerBTN.TextLineOne = $"Toggle Ion Cube Generator Power";
            _powerOffPagePowerBTN.Tag = this;
            #endregion

            #region PowerOffPage Power BTN

            var storage_BTN = _uI_Frame.FindChild("Storage_BTN")?.gameObject;

            if (storage_BTN == null)
            {
                QuickLogger.Error("PowerOffPage Power Button not found.");
                return false;
            }

            var _storage_BTN = storage_BTN.AddComponent<InterfaceButton>();
            _storage_BTN.OnButtonClick = OnButtonClick;
            _storage_BTN.BtnName = "StorageBTN";
            _storage_BTN.ButtonMode = InterfaceButtonMode.Background;
            _storage_BTN.TextLineOne = $"Open Storage";
            _storage_BTN.Tag = this;
            #endregion

            #region Animator

            _animator = gameObject.GetComponent<Animator>();
            if (_animator == null)
            {
                QuickLogger.Error("Animator not found.");
                return false;
            }

            _animator.enabled = true;
            #endregion
            return true;
        }

        public void ShutDownDisplay()
        {
            StartCoroutine(ShutDown());
        }

        public void PowerOnDisplay()
        {
            StartCoroutine(PowerOn());
        }

        public void PowerOffDisplay()
        {
            StartCoroutine(PowerOff());
        }

        public IEnumerator PowerOff()
        {
            yield return new WaitForEndOfFrame();
            _animator.enabled = true;
            _animator.SetFloat(_stateHash, 0.5f);
        }

        public IEnumerator PowerOn()
        {
            yield return new WaitForEndOfFrame();
            _animator.enabled = true;
            _animator.SetFloat(_stateHash, 1.0f);
        }

        public IEnumerator ShutDown()
        {
            yield return new WaitForEndOfFrame();
            _animator.enabled = true;
            _animator.SetFloat(_stateHash, 0.0f);

        }
    }
}
