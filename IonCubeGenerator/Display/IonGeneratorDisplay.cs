using Common;
using IonCubeGenerator.Display.Patching;
using IonCubeGenerator.Enums;
using IonCubeGenerator.Extensions;
using IonCubeGenerator.Mono;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IonCubeGenerator.Display
{
    /// <summary>
    /// A component that controls the screens UI input functions
    /// </summary>
    internal class IonGeneratorDisplay : MonoBehaviour
    {
        #region Private Members
        private GameObject _canvasGameObject;
        private GameObject _powerOffPage;
        private GameObject _operationPage;
        private GameObject _uIFrame;
        private Action _storageAction;
        private Animator _animator;
        private Action<bool> _updateBreakerAction;
        private int _stateHash;
        private Text _speedMode;
        private GameObject _lButton;
        private GameObject _clocking;
        private GameObject _rButton;
        private CubeGeneratorMono _mono;
        private Text _percentDisplay;
        private Image _percentageBar;
        private Image _storageBar;
        private Text _storageAmount;
        private readonly Color _fireBrick = new Color(0.6953125f, 0.1328125f, 0.1328125f);
        private readonly Color _cyan = new Color(0.13671875f, 0.7421875f, 0.8046875f);
        private readonly Color _green = new Color(0.0703125f, 0.92578125f, 0.08203125f);
        private readonly Color _orange = new Color(0.95703125f, 0.4609375f, 0f);

        #endregion

        #region Public Properties
        public bool DisplayCreated { get; private set; }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _stateHash = Animator.StringToHash("State");
        }
        #endregion

        #region Internal Methods

        internal void Setup(CubeGeneratorMono mono, Action storage, Action<bool> updateBreaker)
        {
            DisplayLanguagePatching.AdditionPatching();
            _storageAction = storage;
            _updateBreakerAction = updateBreaker;
            _mono = mono;
            _animator = mono.Animator;
            if (FindAllComponents() == false)
            {
                QuickLogger.Error("// ============== Error getting all Components ============== //");
                ShutDownDisplay();
                return;
            }

            UpdateSpeedModeText();

            DisplayCreated = true;
        }

        internal void OnButtonClick(string btnName, object tag)
        {
            switch (btnName)
            {
                case "StorageBTN":
                    _storageAction?.Invoke();
                    break;

                case "StorageBTNPO":
                    _storageAction?.Invoke();
                    break;

                case "UIFramePowerBTN":
                    _updateBreakerAction?.Invoke(true);
                    break;

                case "PowerOffPagePowerBTN":
                    _updateBreakerAction?.Invoke(false);
                    break;

                case "LButton":
                    _mono.CurrentSpeedMode = _mono.CurrentSpeedMode.Prev();
                    UpdateSpeedModeText();
                    break;

                case "RButton":
                    _mono.CurrentSpeedMode = _mono.CurrentSpeedMode.Next();
                    UpdateSpeedModeText();
                    break;
            }
        }

        internal void ShutDownDisplay()
        {
            StartCoroutine(ShutDown());
        }

        internal void PowerOnDisplay()
        {
            StartCoroutine(PowerOn());
        }

        internal void PowerOffDisplay()
        {
            StartCoroutine(PowerOff());
        }

        internal void UpdatePercentageText(int percent)
        {
            _percentDisplay.text = $"{percent}%";
        }

        internal void UpdatePercentageBar(float percentage, float minBarValue, float maxBarValue)
        {
            _percentageBar.fillAmount = Mathf.Clamp(percentage, minBarValue, maxBarValue);
        }

        internal void UpdateStoragePercentBar(float percentage, float minBarValue, float maxBarValue)
        {
            _storageBar.fillAmount = Mathf.Clamp(percentage, minBarValue, maxBarValue);
        }

        internal void UpdateStorageAmount(int currentAmount, int maxAmount)
        {
            _storageAmount.text = $"{currentAmount}/{maxAmount}";

            float percent = (float)(currentAmount * 1.0 / maxAmount * 1.0) * 100.0f;

            if (Math.Round(percent) <= 25)
            {
                _storageBar.color = _cyan;
            }

            if (Math.Round(percent) > 25 && percent <= 50)
            {
                _storageBar.color = _green;
            }

            if (Math.Round(percent) > 50 && percent <= 75)
            {
                _storageBar.color = _orange;
            }

            if (Math.Round(percent) > 75 && percent <= 100)
            {
                _storageBar.color = _fireBrick;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateSpeedModeText()
        {
            _speedMode.text = _mono.CurrentSpeedMode.ToString();
        }

        private bool FindAllComponents()
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

            _uIFrame = _operationPage.FindChild("UI_Frame")?.gameObject;

            if (_uIFrame == null)
            {
                QuickLogger.Error("UI_Frame not found.");
                return false;
            }
            #endregion

            #region UI_Frame Power BTN

            var uIFramePowerBtn = _uIFrame.FindChild("Power_BTN")?.gameObject;

            if (uIFramePowerBtn == null)
            {
                QuickLogger.Error("UI_Frame Power Button not found.");
                return false;
            }

            var powerBTN = uIFramePowerBtn.AddComponent<InterfaceButton>();
            powerBTN.OnButtonClick = OnButtonClick;
            powerBTN.BtnName = "UIFramePowerBTN";
            powerBTN.ButtonMode = InterfaceButtonMode.Background;
            powerBTN.TextLineOne = GetLanguage(DisplayLanguagePatching.ToggleIonPowerKey);
            powerBTN.Tag = this;
            #endregion

            #region Clocking

            _clocking = _uIFrame.FindChild("Clocking")?.gameObject;

            if (_clocking == null)
            {
                QuickLogger.Error("Clocking not found.");
                return false;
            }
            #endregion

            #region SpeedMode

            var speedMode = _clocking.FindChild("SpeedMode")?.gameObject;

            if (speedMode == null)
            {
                QuickLogger.Error("SpeedMode not found.");
                return false;
            }

            _speedMode = speedMode.GetComponent<Text>();
            #endregion

            #region LButton BTN

            _lButton = _clocking.FindChild("LButton")?.gameObject;

            if (_lButton == null)
            {
                QuickLogger.Error("LButton not found.");
                return false;
            }

            var lButton = _lButton.AddComponent<InterfaceButton>();
            lButton.OnButtonClick = OnButtonClick;
            lButton.BtnName = "LButton";
            lButton.ButtonMode = InterfaceButtonMode.Background;
            lButton.TextLineOne = GetLanguage(DisplayLanguagePatching.PrevSpeedKey);
            lButton.Tag = this;
            #endregion

            #region RButton BTN

            _rButton = _clocking.FindChild("RButton")?.gameObject;

            if (_rButton == null)
            {
                QuickLogger.Error("RButton not found.");
                return false;
            }

            var rButton = _rButton.AddComponent<InterfaceButton>();
            rButton.OnButtonClick = OnButtonClick;
            rButton.BtnName = "RButton";
            rButton.ButtonMode = InterfaceButtonMode.Background;
            rButton.TextLineOne = GetLanguage(DisplayLanguagePatching.NextSpeedKey);
            rButton.Tag = this;
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
            _powerOffPagePowerBTN.TextLineOne = GetLanguage(DisplayLanguagePatching.ToggleIonPowerKey);
            _powerOffPagePowerBTN.Tag = this;
            #endregion

            #region Storage BTN

            var storage_BTN = _uIFrame.FindChild("Storage_BTN")?.gameObject;

            if (storage_BTN == null)
            {
                QuickLogger.Error("PowerOffPage Power Button not found.");
                return false;
            }

            var _storage_BTN = storage_BTN.AddComponent<InterfaceButton>();
            _storage_BTN.OnButtonClick = OnButtonClick;
            _storage_BTN.BtnName = "StorageBTN";
            _storage_BTN.ButtonMode = InterfaceButtonMode.Background;
            _storage_BTN.TextLineOne = GetLanguage(DisplayLanguagePatching.OpenStorageKey);
            _storage_BTN.Tag = this;
            #endregion

            #region Storage BTN PO

            var storageBtnPo = _powerOffPage.FindChild("Storage_BTN_PO")?.gameObject;

            if (storageBtnPo == null)
            {
                QuickLogger.Error("Storage_BTN_PO not found.");
                return false;
            }

            var poStorageBtn = storageBtnPo.AddComponent<InterfaceButton>();
            poStorageBtn.OnButtonClick = OnButtonClick;
            poStorageBtn.BtnName = "StorageBTNPO";
            poStorageBtn.ButtonMode = InterfaceButtonMode.Background;
            poStorageBtn.TextLineOne = GetLanguage(DisplayLanguagePatching.OpenStorageKey);
            poStorageBtn.Tag = this;
            #endregion

            #region Complete

            var complete = _uIFrame.FindChild("Complete")?.gameObject;

            if (complete == null)
            {
                QuickLogger.Error("Complete not found.");
                return false;
            }

            _percentDisplay = complete.GetComponent<Text>();
            #endregion

            #region Mask

            var mask = _uIFrame.FindChild("Mask")?.gameObject;

            if (mask == null)
            {
                QuickLogger.Error("Mask not found.");
                return false;
            }
            #endregion

            #region Mask2

            var mask2 = _uIFrame.FindChild("Mask_2")?.gameObject;

            if (mask2 == null)
            {
                QuickLogger.Error("Mask_2 not found.");
                return false;
            }
            #endregion

            #region Full_Bar
            var fullBar = mask.FindChild("Full_Bar")?.gameObject;

            if (fullBar == null)
            {
                QuickLogger.Error("Full_Bar not found.");
                return false;
            }

            _percentageBar = fullBar.GetComponent<Image>();
            #endregion

            #region StorageBar
            var statusFullBar = mask2.FindChild("Status_Full_Bar")?.gameObject;

            if (statusFullBar == null)
            {
                QuickLogger.Error("Status_Full_Bar not found.");
                return false;
            }

            _storageBar = statusFullBar.GetComponent<Image>();
            #endregion

            #region Storage

            var storage = _uIFrame.FindChild("Storage")?.gameObject;

            if (storage == null)
            {
                QuickLogger.Error("Storage UI was not found.");
                return false;
            }

            _storageAmount = storage.GetComponent<Text>();
            #endregion

            #region CompletedTXT

            var completedTxt = _uIFrame.FindChild("Completed_Txt")?.gameObject;

            if (completedTxt == null)
            {
                QuickLogger.Error("Completed_Txt was not found.");
                return false;
            }

            completedTxt.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.CompletedKey);
            #endregion

            #region Storage_LBL

            var storageLbl = _uIFrame.FindChild("Storage_LBL")?.gameObject;

            if (storageLbl == null)
            {
                QuickLogger.Error("Storage_LBL was not found.");
                return false;
            }

            storageLbl.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.StorageKey);
            #endregion

            #region Overclock_Txt

            var overClocking = _clocking.FindChild("Overclock_Txt")?.gameObject;

            if (overClocking == null)
            {
                QuickLogger.Error("Overclock_Txt not found.");
                return false;
            }

            overClocking.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.OverClockKey);
            #endregion

            #region PowerOffPage Power BTN

            var powerOff = _powerOffPage.FindChild("PoweredOff")?.gameObject;

            if (powerOff == null)
            {
                QuickLogger.Error("PoweredOff not found.");
                return false;
            }

            powerOff.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.PoweredOffKey);

            #endregion

            #region PowerOffPage Power BTN

            var ready = _powerOffPage.FindChild("Ready")?.gameObject;

            if (ready == null)
            {
                QuickLogger.Error("Ready not found.");
                return false;
            }

            ready.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.ReadyKey);

            #endregion

            return true;
        }

        private string GetLanguage(string key)
        {
            return Language.main.Get(key);
        }
        #endregion

        #region IEnumerators
        private IEnumerator PowerOff()
        {
            yield return new WaitForEndOfFrame();
            _animator.SetFloat(_stateHash, 0.5f);
        }

        private IEnumerator PowerOn()
        {
            yield return new WaitForEndOfFrame();
            _animator.SetFloat(_stateHash, 1.0f);
        }

        private IEnumerator ShutDown()
        {
            yield return new WaitForEndOfFrame();
            _animator.SetFloat(_stateHash, 0.0f);

        }
        #endregion
    }
}
