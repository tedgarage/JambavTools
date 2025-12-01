using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Jambav.Settings
{
    public class EventCodePage : BaseSettingsPage
{

    [SerializeField] private TMP_InputField eventCodeInputField;
    [SerializeField] private Transform textHolder;
    [SerializeField] private TextMeshProUGUI eventCodeText;
    [SerializeField] private Transform EditButton;
    [SerializeField] private TextMeshProUGUI expiredDateText;
    [SerializeField] private Transform expireDataHolders;


    public void EditButtonAction()
    {
        expireDataHolders.gameObject.SetActive(false);
        eventCodeInputField.gameObject.SetActive(true);
        textHolder.gameObject.SetActive(false);
        ShowActionButtons.Invoke(true);
    }
    public override void RevertActionDone()
    {
        SetData();
        HideActionButtons?.Invoke();
    }

    public override void SaveActionDone()
    {
        string code = eventCodeInputField.text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(code))
        {
            ToastPanelHandler.sharedInstance.ShowToastMessage("Event code cannot be empty.", true);
            return;
        }
        if (!Regex.IsMatch(code, @"^[A-Za-z0-9_]+$"))
        {
            ToastPanelHandler.sharedInstance.ShowToastMessage("Only letters, numbers and underscore (_) are allowed.", true);
            return;
        }
        LoaderPanelHandler.sharedInstance.ShowLoadingPanel(LoadingType.Sending);
        SettingsFirebaseController.sharedInstance.GetDateTimeFromEventCode(code, (expireDate) =>
        {
            LoaderPanelHandler.sharedInstance.HideLoadingPanel();
            if (expireDate > DateTime.Now)
            {
                ToastPanelHandler.sharedInstance.ShowToastMessage("Event code is Updated.");

                eventCodeText.text = code;
                expiredDateText.text = expireDate.ToShortDateString();
                bool itsFistTime = !PlayerPrefs.HasKey(CommonConstants.EventCodeKey);
                PlayerPrefs.SetString(CommonConstants.EventCodeKey, code);
                PlayerPrefs.SetString(CommonConstants.ExpireDateKey, expireDate.Date.ToString());
                PlayerPrefs.Save();
                textHolder.gameObject.SetActive(true);
                eventCodeInputField.gameObject.SetActive(false);
                expireDataHolders.gameObject.SetActive(true);
                HideActionButtons.Invoke();
                if(itsFistTime)
                    SettingsManager.sharedInstance.EventCodeFirstUpdated();
            }
            else
            {
                ToastPanelHandler.sharedInstance.ShowToastMessage("Event code is already Expired.", true);
            }

        },()=>
        {
            LoaderPanelHandler.sharedInstance.HideLoadingPanel();
            ToastPanelHandler.sharedInstance.ShowToastMessage("Event code is invalid.", true);
        });

       
        
    }

    public override void SetUpPage()
    {

        string code = SettingsManager.sharedInstance.GetEventCodePreference();
        if(code == "")
        {
            EditButtonAction();
            ShowActionButtons?.Invoke(false);
            print("Called");
        }
       else
        {
            textHolder.gameObject.SetActive(true);
            eventCodeInputField.gameObject.SetActive(false);
            expireDataHolders.gameObject.SetActive(true);
            eventCodeText.text = code;
            expiredDateText.text = SettingsManager.sharedInstance.GetExpireDatePreference().ToShortDateString();
        }
        
    }
    private void SetData()
    {
        string code = SettingsManager.sharedInstance.GetEventCodePreference();
        textHolder.gameObject.SetActive(true);
        eventCodeInputField.gameObject.SetActive(false);
        expireDataHolders.gameObject.SetActive(true);
        eventCodeText.text = code;
        expiredDateText.text = SettingsManager.sharedInstance.GetExpireDatePreference().ToShortDateString();
    }
}
}