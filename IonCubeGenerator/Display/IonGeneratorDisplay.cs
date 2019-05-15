namespace IonCubeGenerator.Display
{
    using Common;
    using IonCubeGenerator.Display.Patching;
    using IonCubeGenerator.Enums;
    using IonCubeGenerator.Mono;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A component that controls the screens UI input functions
    /// </summary>
    internal class IonGeneratorDisplay : MonoBehaviour
    {
        #region Private Members
        private const float MaxBar = CubeGeneratorMono.ProgressComplete;
        private const float PowerOn = 1.0f;
        private const float Boot = 0.0f;
        private GameObject _canvasGameObject;
        private GameObject _powerOffPage;
        private GameObject _operationPage;
        private GameObject _uIFrame;
        private int _stateHash;
        private Text _speedMode;
        private GameObject _lButton;
        private GameObject _clocking;
        private GameObject _rButton;
        private Text _percentDisplay;
        private Image _percentageBar;
        private Image _storageBar;
        private Text _storageAmount;
        private readonly Color _fireBrick = new Color(0.6953125f, 0.1328125f, 0.1328125f);
        private readonly Color _cyan = new Color(0.13671875f, 0.7421875f, 0.8046875f);
        private readonly Color _green = new Color(0.0703125f, 0.92578125f, 0.08203125f);
        private readonly Color _orange = new Color(0.95703125f, 0.4609375f, 0f);
        private bool _coroutineStarted;
        private const float DelayedStartTime = 0.5f;
        private const float RepeatingUpdateInterval = 1f;
        private CubeGeneratorMono _mono;
        private CubeGeneratorAnimator _animatorController;
        private bool _initialized;
        private const float BarMinValue = 0.087f;
        private const float BarMaxValue = 0.409f;
        private const int MaxContainerSpaces = CubeGeneratorContainer.MaxAvailableSpaces;

        #endregion

        #region Public Properties
        public bool ShowBootScreen { get; set; } = true;
        public int BootTime { get; set; } = 3;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _stateHash = UnityEngine.Animator.StringToHash("State");
        }

        private void Start()
        {
            if (!_coroutineStarted)
                base.InvokeRepeating(nameof(UpdateDisplay), DelayedStartTime * 3f, RepeatingUpdateInterval);

            DisplayLanguagePatching.AdditionPatching();

            if (FindAllComponents() == false)
            {
                QuickLogger.Error("// ============== Error getting all Components ============== //");
                return;
            }

            _animatorController = this.transform.GetComponent<CubeGeneratorAnimator>();

            if (_animatorController == null)
            {
                QuickLogger.Error("Animator component not found on the GameObject.");
            }

            _mono = this.transform.GetComponent<CubeGeneratorMono>();

            if (_mono == null)
            {
                QuickLogger.Error("CubeGeneratorMono component not found on the GameObject.");
            }

            _initialized = true;

            UpdateSpeedModeText();

            BootScreen();

        }

        #endregion

        #region Internal Methods
        internal void OnButtonClick(string btnName, object additionalObject)
        {
            switch (btnName)
            {
                case "StorageBTN":
                    _mono.OpenStorage();
                    break;

                case "LButton":
                    switch (_mono.CurrentSpeedMode)
                    {
                        case SpeedModes.Max:
                            _mono.CurrentSpeedMode = SpeedModes.High;
                            break;
                        case SpeedModes.High:
                            _mono.CurrentSpeedMode = SpeedModes.Low;
                            break;
                        case SpeedModes.Low:
                            _mono.CurrentSpeedMode = SpeedModes.Min;
                            break;
                        case SpeedModes.Min:
                            _mono.CurrentSpeedMode = SpeedModes.Off;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    UpdateSpeedModeText();
                    break;

                case "RButton":
                    switch (_mono.CurrentSpeedMode)
                    {
                        case SpeedModes.High:
                            _mono.CurrentSpeedMode = SpeedModes.Max;
                            break;
                        case SpeedModes.Low:
                            _mono.CurrentSpeedMode = SpeedModes.High;
                            break;
                        case SpeedModes.Min:
                            _mono.CurrentSpeedMode = SpeedModes.Low;
                            break;
                        case SpeedModes.Off:
                            _mono.CurrentSpeedMode = SpeedModes.Min;
                            break;
                    }
                    UpdateSpeedModeText();
                    break;
            }
        }

        #endregion

        #region Private Methods
        private void UpdateSpeedModeText()
        {
            switch (_mono.CurrentSpeedMode)
            {
                case SpeedModes.Off:
                    _speedMode.text = GetLanguage(DisplayLanguagePatching.OffKey);
                    break;
                case SpeedModes.Max:
                    _speedMode.text = GetLanguage(DisplayLanguagePatching.MaxKey);
                    break;
                case SpeedModes.High:
                    _speedMode.text = GetLanguage(DisplayLanguagePatching.HighKey);
                    break;
                case SpeedModes.Low:
                    _speedMode.text = GetLanguage(DisplayLanguagePatching.LowKey);
                    break;
                case SpeedModes.Min:
                    _speedMode.text = GetLanguage(DisplayLanguagePatching.MinKey);
                    break;
            }
        }

        private bool FindAllComponents()
        {
            #region Canvas
            _canvasGameObject = this.gameObject.GetComponentInChildren<Canvas>()?.gameObject;
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

            #region Clocking

            _clocking = _uIFrame.FindChild("Clocking")?.gameObject;

            if (_clocking == null)
            {
                QuickLogger.Error("Clocking not found.");
                return false;
            }
            #endregion

            #region SpeedMode

            GameObject speedMode = _clocking.FindChild("SpeedMode")?.gameObject;

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

            InterfaceButton lButton = _lButton.AddComponent<InterfaceButton>();
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

            InterfaceButton rButton = _rButton.AddComponent<InterfaceButton>();
            rButton.OnButtonClick = OnButtonClick;
            rButton.BtnName = "RButton";
            rButton.ButtonMode = InterfaceButtonMode.Background;
            rButton.TextLineOne = GetLanguage(DisplayLanguagePatching.NextSpeedKey);
            rButton.Tag = this;
            #endregion

            #region Storage BTN

            GameObject storage_BTN = _uIFrame.FindChild("Storage_BTN")?.gameObject;

            if (storage_BTN == null)
            {
                QuickLogger.Error("PowerOffPage Power Button not found.");
                return false;
            }

            InterfaceButton _storage_BTN = storage_BTN.AddComponent<InterfaceButton>();
            _storage_BTN.OnButtonClick = OnButtonClick;
            _storage_BTN.BtnName = "StorageBTN";
            _storage_BTN.ButtonMode = InterfaceButtonMode.Background;
            _storage_BTN.TextLineOne = GetLanguage(DisplayLanguagePatching.OpenStorageKey);
            _storage_BTN.Tag = this;
            #endregion

            #region Complete

            GameObject complete = _uIFrame.FindChild("Complete")?.gameObject;

            if (complete == null)
            {
                QuickLogger.Error("Complete not found.");
                return false;
            }

            _percentDisplay = complete.GetComponent<Text>();
            #endregion

            #region Mask

            GameObject mask = _uIFrame.FindChild("Mask")?.gameObject;

            if (mask == null)
            {
                QuickLogger.Error("Mask not found.");
                return false;
            }
            #endregion

            #region Mask2

            GameObject mask2 = _uIFrame.FindChild("Mask_2")?.gameObject;

            if (mask2 == null)
            {
                QuickLogger.Error("Mask_2 not found.");
                return false;
            }
            #endregion

            #region Full_Bar
            GameObject fullBar = mask.FindChild("Full_Bar")?.gameObject;

            if (fullBar == null)
            {
                QuickLogger.Error("Full_Bar not found.");
                return false;
            }

            _percentageBar = fullBar.GetComponent<Image>();
            #endregion

            #region StorageBar
            GameObject statusFullBar = mask2.FindChild("Status_Full_Bar")?.gameObject;

            if (statusFullBar == null)
            {
                QuickLogger.Error("Status_Full_Bar not found.");
                return false;
            }

            _storageBar = statusFullBar.GetComponent<Image>();
            #endregion

            #region Storage

            GameObject storage = _uIFrame.FindChild("Storage")?.gameObject;

            if (storage == null)
            {
                QuickLogger.Error("Storage UI was not found.");
                return false;
            }

            _storageAmount = storage.GetComponent<Text>();
            #endregion

            #region CompletedTXT

            GameObject completedTxt = _uIFrame.FindChild("Completed_Txt")?.gameObject;

            if (completedTxt == null)
            {
                QuickLogger.Error("Completed_Txt was not found.");
                return false;
            }

            completedTxt.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.CompletedKey);
            #endregion

            #region Storage_LBL

            GameObject storageLbl = _uIFrame.FindChild("Storage_LBL")?.gameObject;

            if (storageLbl == null)
            {
                QuickLogger.Error("Storage_LBL was not found.");
                return false;
            }

            storageLbl.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.StorageKey);
            #endregion

            #region Overclock_Txt

            GameObject overClocking = _clocking.FindChild("Overclock_Txt")?.gameObject;

            if (overClocking == null)
            {
                QuickLogger.Error("Overclock_Txt not found.");
                return false;
            }

            overClocking.GetComponent<Text>().text = GetLanguage(DisplayLanguagePatching.OverClockKey);
            #endregion


            return true;
        }

        private string GetLanguage(string key)
        {
            return Language.main.Get(key);
        }

        private void UpdateDisplay()
        {
            if (!_initialized)
                return;

            _coroutineStarted = true;

            UpdatePercentageBar();

            UpdateStoragePercentBar();

            UpdateStorageAmount();

            UpdatePercentageText();
        }

        private void BootScreen()
        {
            StartCoroutine(BootScreenEnu());
        }

        private void PowerOnDisplay()
        {
            StartCoroutine(PowerOnDisplayEnu());
        }

        private void UpdatePercentageText()
        {
            _percentDisplay.text = $"{Mathf.RoundToInt(_mono.GenerationPercent * 100)}%";
        }

        private void UpdatePercentageBar()
        {
            if (_mono == null)
            {
                QuickLogger.Error("Mono is null");
                return;
            }

            //float calcBar = _mono.GenerationPercent / MaxBar;

            float outputBar = _mono.GenerationPercent * (BarMaxValue - BarMinValue) + BarMinValue;

            _percentageBar.fillAmount = Mathf.Clamp(outputBar, BarMinValue, BarMaxValue);

        }

        private void UpdateStoragePercentBar()
        {
            float calcBar = (float)((_mono.NumberOfCubes * 1.0) / (MaxContainerSpaces * 1.0));
            float outputBar = calcBar * (BarMaxValue - BarMinValue) + BarMinValue;
            _storageBar.fillAmount = Mathf.Clamp(outputBar, BarMinValue, BarMaxValue);

        }

        private void UpdateStorageAmount()
        {
            _storageAmount.text = $"{_mono.NumberOfCubes}/{MaxContainerSpaces}";

            float percent = (float)(_mono.NumberOfCubes * 1.0 / MaxContainerSpaces * 1.0) * 100.0f;

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

        #region IEnumerators

        private IEnumerator PowerOnDisplayEnu()
        {
            yield return new WaitForEndOfFrame();
            _animatorController.SetFloatHash(_stateHash, PowerOn);
        }

        private IEnumerator BootScreenEnu()
        {
            yield return new WaitForEndOfFrame();

            if (ShowBootScreen)
            {
                _animatorController.SetFloatHash(_stateHash, Boot);
                yield return new WaitForSeconds(BootTime);
            }

            PowerOnDisplay();
        }

        #endregion
    }
}
