using System;
using UnityEngine;
using Jambav.Utilities;
using Jambav.Localization;

namespace Jambav.Settings
{
    public class SettingsManager : SingletonPersistent<SettingsManager>
    {

        public LocalizationSelector LocalizationSelector;
        [SerializeField] string defaultBundleIdentifier = "com.jambav.vrzoho";
        [HideInInspector] public bool canViewSettings = true;

        public Action GameReadyToPlay;
        public Action OnRestartStart;
        
        
        private MessagePanelHandler messagePanelHandler;
        private LoaderPanelHandler loaderPanelHandler;
        private ToastPanelHandler toastPanelHandler;
        private GameObject mainCanvasHolder;


        private bool codeValid = false;
        private bool deviceNameValid = false;
        private bool internetConnectionValid = false;
        private string bundleName = "";

        override protected void Awake()
        {
            base.Awake();
            Init();
            SetGameplayDataPath();


        }
        private void Start()
        {
            SettingsInputController.sharedInstance.OnSettingToggled += SettingToggledAction;
            CheckInternetConnectionSettingInit();
        }
        private void Init()
        {
            mainCanvasHolder = GameObject.Find("MainCanvasHolder");
            if (mainCanvasHolder == null)
            {
                throw new Exception("MainCanvasHolder not found");
            }
            var handlerScripts = GameObject.Find("HandlerScripts");
            if (handlerScripts == null)
            {
                throw new Exception("HandlersScripts not found");
            }
            messagePanelHandler = handlerScripts.GetComponentInChildren<MessagePanelHandler>();
            loaderPanelHandler = handlerScripts.GetComponentInChildren<LoaderPanelHandler>();
            toastPanelHandler = handlerScripts.GetComponentInChildren<ToastPanelHandler>();

        }
        public bool DoseLocalizationAvailable()
        {
            return (LocalizationSelector != null);

        }
        private void SettingToggledAction()
        {
            if (internetConnectionValid && codeValid && deviceNameValid)
            {

                mainCanvasHolder.SetActive(!mainCanvasHolder.activeSelf);
                if (mainCanvasHolder.activeSelf)
                {
                    SettingsUIController.sharedInstance.OpenSettings();
                }
            }
        }
        private void GameReadyAction(bool _firstTime = false)
        {
            if (_firstTime)
                SettingsUIController.sharedInstance.OpenAllSettings();
            else
                mainCanvasHolder.SetActive(false);
            GameReadyToPlay?.Invoke();
        }
        private void CheckInternetConnectionSettingInit()
        {
            loaderPanelHandler.ShowLoadingPanel(LoadingType.Checking);
            InternetConnectivityChecker.CheckInternetConnection((bool result) =>
            {

                if (result)
                {
                    internetConnectionValid = true;
                    SettingsFirebaseController.sharedInstance.FirebaseLogin((result) =>
                    {
                        if (result)
                        {
                            CheckEventCode();

                        }
                        else
                        {
                            loaderPanelHandler.HideLoadingPanel();
                            messagePanelHandler.SetErrorMessage("Internal Error", "Check your network connection and retry. If the problem persists, please contact your admin.", actionButtonText: "Retry", action: CheckInternetConnectionSettingInit);
                        }
                    });


                }
                else
                {
                    loaderPanelHandler.HideLoadingPanel();
                    messagePanelHandler.SetErrorMessage("No Internet", "Please check your internet connection and try again.", actionButtonText: "Retry", action: CheckInternetConnectionSettingInit);
                }

            });
        }

        private void CheckEventCode()
        {
            string eventCode = GetStringPreference(CommonConstants.EventCodeKey, "");
            if (!string.IsNullOrEmpty(eventCode))
            {
                SettingsFirebaseController.sharedInstance.GetDateTimeFromEventCode(eventCode, (expireDate) =>
                {
                    if (expireDate > DateTime.Now)
                    {
                        SaveStringPreference(CommonConstants.ExpireDateKey, expireDate.Date.ToString());
                        loaderPanelHandler.HideLoadingPanel();
                        codeValid = true;
                        if (CheckDeviceName())
                        {
                            GameReadyAction();
                        }
                    }
                    else
                    {
                        loaderPanelHandler.HideLoadingPanel();
                        messagePanelHandler.SetErrorMessage("Event Code Expired", "Your event code has expired.", actionButtonText: "Enter New Code", action: OpenSettingsForEventCode);
                    }
                }, () =>
                {
                    loaderPanelHandler.HideLoadingPanel();
                    messagePanelHandler.SetErrorMessage("Invalid Event Code", "The event code stored is invalid.", actionButtonText: "Enter New Code", action: OpenSettingsForEventCode);
                });
            }
            else
            {
                OpenSettingsForEventCode();
            }
        }
        private bool CheckDeviceName()
        {
            string deviceName = GetDeviceName();
            if (!string.IsNullOrEmpty(deviceName))
            {
                deviceNameValid = true;
                return true;
            }
            else
            {
                OpenSettingsForDeviceName();
                return false;
            }
        }

        private void OpenSettingsForEventCode()
        {
            loaderPanelHandler.HideLoadingPanel();
            SettingsUIController.sharedInstance.OpenEventCodePage();
        }
        private void OpenSettingsForDeviceName()
        {
            SettingsUIController.sharedInstance.OpenDeviceNamePage();
        }

        private void ShowSettingsPopUp()
        {
            throw new NotImplementedException();
        }
        #region firebase related methods
        private void GetEventCodeExpireDate()
        {

        }
        #endregion

        #region PlayerPrefs Related Methods

        internal string GetEventCodePreference()
        {
            return GetStringPreference(CommonConstants.EventCodeKey, "");
        }

        internal DateTime GetExpireDatePreference()
        {
            string expireDateString = GetStringPreference(CommonConstants.ExpireDateKey, "");
            if (DateTime.TryParse(expireDateString, out DateTime expireDate))
            {
                return expireDate;
            }
            return DateTime.MinValue;
        }

        internal string GetDeviceName()
        {
            return GetStringPreference(CommonConstants.DeviceNameKey, "");
        }

        private void SaveStringPreference(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
        private string GetStringPreference(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key);
        }
        private void SetGameplayDataPath()
        {
            if (Application.identifier == defaultBundleIdentifier)
            {
                bundleName = "JAMBAV";
            }
            else if (Application.identifier.Contains(defaultBundleIdentifier))
            {
                bundleName = Application.identifier.Split('.')[3].ToUpper();
            }

            PlayerPrefs.SetString(CommonConstants.gameplayDataPathKey, string.Format(CommonConstants.GameplayDataPathFormat, bundleName));
            PlayerPrefs.Save();
        }

        internal void DeviceNameFirstUpdated()
        {
            SettingsUIController.sharedInstance.OpenAllSettings();
            deviceNameValid = true;
            GameReadyAction(true);
        }
        internal void EventCodeFirstUpdated()
        {
            OpenSettingsForDeviceName();
            codeValid = true;
        }
        public bool DoseGameReadyToPlay()
        {
            return codeValid && deviceNameValid && internetConnectionValid;
        }

        #endregion
    }
}
